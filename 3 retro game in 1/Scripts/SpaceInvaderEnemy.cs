using Godot;
using System;
using System.Xml;

public class SpaceInvaderEnemy : Area2D
{
    private static readonly PackedScene BulletScene = GD.Load<PackedScene>("res://Scenes/Bullet.tscn");

    public float movDownSpeed, movHorizonSpeed;
    public int health, bulletDamage;
    public float shootingRate, bulletSpeed;
    
    private Sprite enemySprite;
    private AnimatedSprite explosionSprite;
    private Position2D bulletSpawnPos;
    private Timer bulletTimer;
    private TextureProgress healthProgress;
    private Tween healthTween;

    private Random random = new Random();
    private int maxHealth;
    
    private int maxLocalX = 64 * 3, horizontalDirection = 1;

    public override void _Ready()
    {
        explosionSprite = GetNode<AnimatedSprite>("ExplosionAnimation");
        enemySprite = GetNode<Sprite>("Sprite");
        bulletTimer = GetNode<Timer>("BulletTimer");
        bulletSpawnPos = GetNode<Position2D>("BulletSpawnPos");
        healthProgress = GetNode<TextureProgress>("ProgressBG");
        healthTween = GetNode<Tween>("ProgressBG/Tween");

        maxHealth = health;
        healthProgress.Visible = false;
        bulletTimer.Start(1 / shootingRate);
    }

    public override void _Process(float delta)
    {
        if (explosionSprite.Playing) return;
        var newPosX = Position.x + delta * movHorizonSpeed * horizontalDirection;
        var newPosY = Position.y + delta * movDownSpeed;
        if (newPosX > maxLocalX)
        {
            newPosX = maxLocalX;
            horizontalDirection = -1;
        }
        else if (newPosX < 0)
        {
            newPosX = 0;
            horizontalDirection = 1;
        }

        Position = new Vector2(newPosX, newPosY);
    }

    public void RandomizeStat()
    {
        horizontalDirection = random.NextDouble() < 0.5 ? -1 : 1;  // randomize direction
        enemySprite.Frame = random.Next(enemySprite.Hframes * enemySprite.Vframes);
        Position = new Vector2(random.Next(maxLocalX), Position.y);
    }

    private void _on_BulletTimer_timeout()
    {
        if (explosionSprite.Playing) return;  // Dead
        bulletTimer.Start(1 / shootingRate);
        
        var bulletInstance = BulletScene.Instance<Bullet>();
        bulletInstance.GlobalPosition = bulletSpawnPos.GlobalPosition;
        bulletInstance.bulletSpeed = bulletSpeed;
        bulletInstance.bulletDamage = bulletDamage;
        bulletInstance.curBulletType = Bullet.BulletType.Enemy;
        GetParent().AddChild(bulletInstance);
    }

    private void _on_SpaceInvaderEnemy_area_entered(Area2D other)
    {
        if (other.IsInGroup("bulletDeleteRegion")) QueueFree();
        else if (other.IsInGroup("Bullet") && ((Bullet)other).curBulletType == Bullet.BulletType.Player)
        {
            var playerBullet = (Bullet) other;
            var beforeHealth100 = (int)(100 * (float)health / maxHealth);
            health -= playerBullet.bulletDamage;
            health = health < 0 ? 0 : health;
            healthProgress.Visible = true;
            var newHealth1000 = (int)(100 * (float)health / maxHealth);
            healthTween.InterpolateProperty(healthProgress, "value", beforeHealth100, newHealth1000, 0.5f);
            healthTween.Start();
            playerBullet.BulletExplosion();
            if (health <= 0) DieAnimation();
        }
    }

    public void DieAnimation()
    {
        GetNode<AudioStreamPlayer>("DeadAudioFX").Play();
        GetNode<AudioStreamPlayer>("CollisionHit").Play();
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
        explosionSprite.Visible = true;
        explosionSprite.Play();
    }

    private void _on_ExplosionAnimation_animation_finished()
    {
        Modulate = new Color(1, 1, 1, 0);
    }

    private void _on_DeadAudioFX_finished()
    {
        QueueFree();
    }
}
