using Godot;
using System;

public class PinballMap : Node2D
{
    [Export] public World.Player playerType = World.Player.Left;
    [Export] public int maxLocalY = 64 * 11, maxLocalX = 64 * 3;

    [Signal] public delegate void PlayerMoved(Vector2 playerGlobPos, Pacman player);
    [Signal] public delegate void PlayerGotHit();
    
    public Pacman curPacmanStill;
    private bool _hasStillBall = false;
    public bool hasStillBall
    {
        get => _hasStillBall;
        set
        {
            if (value) EmitSignal(nameof(PlayerMoved), curPacmanStill.Position, curPacmanStill);
            _hasStillBall = value;
        }
    }
    
    private static readonly PackedScene BulletScene = GD.Load<PackedScene>("res://Scenes/Bullet.tscn");

    private float lastFireTime;
    private string playerStrPrefix;
    private Area2D playerBody;
    private Position2D bulletSpawnPoint;
    private AnimatedSprite pinballPlayer;
    private Timer flashTimer;
    private Tween flashTween;

    public override void _Ready()
    {
        playerStrPrefix = playerType == World.Player.Left ? "player1" : "player2";
        playerBody= GetNode<Area2D>("player");
        bulletSpawnPoint = GetNode<Position2D>("player/BulletSpawnPoint");
        flashTimer = GetNode<Timer>("player/FlashTimer");
        flashTween = GetNode<Tween>("player/playerSprite/Tween");

        pinballPlayer = GetNode<AnimatedSprite>("player/playerSprite"); 
        pinballPlayer.Play(playerType == World.Player.Right ? "player2 pinball" : "player1 pinball");
        var pacmanInitPos = GetNode<Position2D>("player/PacmanInitPos");
        pacmanInitPos.Position = World.Player.Right == playerType ? new Vector2(-84, pacmanInitPos.Position.y) : pacmanInitPos.Position;
        playerBody.Position = new Vector2(maxLocalX / 2, maxLocalY / 2);
        var collisionShape = GetNode<CollisionShape2D>("PacmanGameOver/CollisionShape2D");
        collisionShape.Position = World.Player.Right == playerType ? new Vector2(224, collisionShape.Position.y) : collisionShape.Position;
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionPressed(playerStrPrefix + "ShootBullet")) CheckShoot();

        var curInput = new Vector2();
        if (Input.IsActionPressed(playerStrPrefix + "PinLeft")) curInput += new Vector2(-1, 0);
        if (Input.IsActionPressed(playerStrPrefix + "PinRight")) curInput += new Vector2(1, 0);
        if (Input.IsActionPressed(playerStrPrefix + "PinUp")) curInput += new Vector2(0, -1);
        if (Input.IsActionPressed(playerStrPrefix + "PinDown")) curInput += new Vector2(0, 1);
        curInput = curInput.Normalized();

        playerBody.Position += World.Player2Stat[playerType].pinBallSpeed * delta * curInput;
        playerBody.Position = new Vector2(Mathf.Clamp(playerBody.Position.x, 0, maxLocalX),
                                            Mathf.Clamp(playerBody.Position.y, 0, maxLocalY));
        
        if (hasStillBall && curInput != Vector2.Zero) EmitSignal(nameof(PlayerMoved), curPacmanStill.Position, curPacmanStill);
        
    }

    // Debug to see the pinball movable area
    // public override void _Draw()
    // {
    //     DrawRect(new Rect2(0, 0, maxLocalX, maxLocalY), new Color(0, 0.5f, 0, .2f));
    // }

    private void CheckShoot()
    {
        if (OS.GetTicksMsec() - lastFireTime < World.Player2Stat[playerType].fireRateInMS || World.Player2Stat[playerType].bulletCount == 0) return;
        lastFireTime = OS.GetTicksMsec();

        World.Player2Stat[playerType].bulletCount--;
        var bulletInstance = BulletScene.Instance<Bullet>();
        bulletInstance.GlobalPosition = bulletSpawnPoint.GlobalPosition;
        bulletInstance.bulletSpeed = 200f;
        bulletInstance.bulletDamage = 10;
        bulletInstance.bulletDirection = -1;
        bulletInstance.curBulletType = Bullet.BulletType.Player;
        bulletInstance.playerBulletType = playerType;
        GetParent().AddChild(bulletInstance);
    }

    private void _on_Pacman_BallLeave()
    {
        hasStillBall = false;
        curPacmanStill = null;
    }

    private void _on_player_area_entered(Area2D other)
    {
        // Enemy bullet hurt player
        if (other.IsInGroup("Bullet") && ((Bullet)other).curBulletType == Bullet.BulletType.Enemy)
        {
            World.Player2Stat[playerType].health -= ((Bullet) other).bulletDamage;
            ((Bullet) other).BulletExplosion();
            FlashEffect();
        }
        else if (other.IsInGroup("spaceEnemy"))
        {
            World.Player2Stat[playerType].health -= ((SpaceInvaderEnemy) other).health;
            ((SpaceInvaderEnemy) other).DieAnimation();
            FlashEffect();
        }
    }

    private void FlashEffect()
    {
        EmitSignal(nameof(PlayerGotHit));
        flashTween.InterpolateMethod(this, nameof(SetFlashShader), 0f, 0.8f, 0.3f);
        flashTween.Start();
        flashTimer.Start(0.35f);
    }

    private void _on_FlashTimer_timeout()
    {
        var initVal = (float) ((ShaderMaterial)pinballPlayer.Material).GetShaderParam("flashModifier");
        flashTween.InterpolateMethod(this, nameof(SetFlashShader), 0.8f, 0, 0.3f);
        flashTween.Start();
    }

    private void SetFlashShader(float val)
    {
        ((ShaderMaterial)pinballPlayer.Material).SetShaderParam("flashModifier", val);
    }
}
