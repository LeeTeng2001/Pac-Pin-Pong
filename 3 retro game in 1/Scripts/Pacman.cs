using Godot;
using System;

public class Pacman : Area2D
{
    private enum PacmanState {Still, FlyingToGrid, InGrid, FlyingOutGrid, Dead};
    private enum GridDirect {Up, Down, Left, Right};
    private static readonly Godot.Collections.Dictionary<GridDirect, Vector2> Dir2Vec = new Godot.Collections.Dictionary<GridDirect, Vector2>
    {
        { GridDirect.Up, new Vector2(0, -1) },
        { GridDirect.Down, new Vector2(0, 1) },
        { GridDirect.Left, new Vector2(-1, 0) },
        { GridDirect.Right, new Vector2(1, 0) },
    };

    public World.Player curPlayer = World.Player.Left;
    public static float InGridMaxTime = 6f, PinballMaxTime = 5f;
    
    [Signal] public delegate void BallLeave();
    [Signal] public delegate void CollectedPoint(bool isPowerUp);
    [Signal] public delegate void PacmanDieSignal(Vector2 globalLoc);
    [Signal] public delegate void StartTimeLimitCountdown(float duration, World.Player playerType);

    private AnimatedSprite pacmanSprite;
    private Position2D spritePivot;
    private AnimationPlayer _animationPlayer;
    private AudioStreamPlayer _collisionSoundFX;
    
    private PacmanState curState = PacmanState.Still;
    private GridDirect curDirection, desireDirection;
    private Vector2 nextGrid;

    public Area2D parentPin;
    private Vector2 parentOffset;
    private bool forwardAnimation = false;

    private const int CellSize = 64, OutOfBoundDamage = 40, CatchRecoverHealth = 20, CatchRecoverBullet = 5;
    private float accumGridTime = 0, movPixelTime;
    private string playerStrPrefix;

    public override void _Ready()
    {
        pacmanSprite = GetNode<AnimatedSprite>("RotationPivot/PacmanSprite");
        spritePivot = GetNode<Position2D>("RotationPivot");
        _animationPlayer = GetNode<AnimationPlayer>("RotationPivot/AnimationPlayer");
        _collisionSoundFX = GetNode<AudioStreamPlayer>("CollisionHit");
        
        // Setup
        playerStrPrefix = curPlayer == World.Player.Left ? "player1" : "player2";
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true); // cause start in pinball
    }

    public override void _Process(float delta)
    {
        switch (curState)
        {
            case PacmanState.InGrid:
                // Update user input
                if (Input.IsActionPressed(playerStrPrefix + "PacRight")) desireDirection = GridDirect.Right;
                else if (Input.IsActionPressed(playerStrPrefix + "PacLeft")) desireDirection = GridDirect.Left;
                else if (Input.IsActionPressed(playerStrPrefix + "PacUp")) desireDirection = GridDirect.Up;
                else if (Input.IsActionPressed(playerStrPrefix + "PacDown")) desireDirection = GridDirect.Down;
                
                // Update Timer
                

                // Update grid movement
                movPixelTime = 1 / (float) World.Player2Stat[curPlayer].pacmanSpeed;  // Update speed because speed might gradually vary
                accumGridTime += delta;
                var movStep = 0;
                while (accumGridTime > movPixelTime)
                {
                    accumGridTime -= movPixelTime;
                    movStep++;
                }
                MoveGridStep(movStep);
                break;
            case PacmanState.FlyingOutGrid:
                if (Input.IsActionPressed(playerStrPrefix + "PacUp")) desireDirection = GridDirect.Up;
                else if (Input.IsActionPressed(playerStrPrefix + "PacDown")) desireDirection = GridDirect.Down;

                Position += World.Player2Stat[curPlayer].pacmanSpeed * delta * Dir2Vec[curDirection];
                break;
            case PacmanState.Still:
                if (forwardAnimation) return; // don't update when playing death animation
                if (Input.IsActionPressed(playerStrPrefix + "ShootBall"))
                {
                    curState = PacmanState.FlyingToGrid;
                    EmitSignal(nameof(StartTimeLimitCountdown), InGridMaxTime, curPlayer);
                    EmitSignal(nameof(BallLeave));
                    accumGridTime = 0;
                }
                else
                {
                    GlobalPosition = parentPin.GlobalPosition + parentOffset;
                }
                break;
            case PacmanState.FlyingToGrid:
                movPixelTime = 1 / (float) World.Player2Stat[curPlayer].pacmanSpeed;  // Update speed because speed might gradually vary
                accumGridTime += delta;
                var moveSteps = 0;
                while (accumGridTime > movPixelTime)
                {
                    accumGridTime -= movPixelTime;
                    moveSteps++;
                }

                var direction = (nextGrid - Position).Normalized();
                while (Position != nextGrid && moveSteps > 0)
                {
                    if (Position.DistanceTo(nextGrid) < 1.5)  // TODO: Possible bug location
                    {
                        Position = nextGrid;
                        GridMovInit();
                        break;
                    }
                    Position += direction;
                    moveSteps--;
                }

                break;
        }
    }

    public void GridMovInit()
    {
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", false);
        curDirection = curPlayer == World.Player.Right ? GridDirect.Left : GridDirect.Right;
        desireDirection = curDirection;
        curState = PacmanState.InGrid;
        nextGrid = Position + Dir2Vec[curDirection] * CellSize;
    }

    public void UpdateDesireLoc(Vector2 localPos)
    {
        nextGrid = localPos;
    }

    private bool CheckSpecialPath()
    {
        var oppositePlayer = curPlayer == World.Player.Left ? World.Player.Right : World.Player.Left;
        
        if (PacmanMap1.PlayerGridStart[oppositePlayer].Contains(Position))
        {
            curState = PacmanState.FlyingOutGrid;
            return true;
        }
        if (PacmanMap1.PlayerGridStart[curPlayer].Contains(Position))
        {
            curState = PacmanState.FlyingOutGrid;
            return true;
        }

        return false;
    }
    
    private void CheckNextPath()
    {
        var nextDesireGrid = Position + CellSize * Dir2Vec[desireDirection];
        if (PacmanMap1.ValidPath.Contains(nextDesireGrid))  // If next desire position is valid
        {
            nextGrid = nextDesireGrid;
            curDirection = desireDirection;
            return;
        }
        
        nextDesireGrid = Position + CellSize * Dir2Vec[curDirection];
        if (PacmanMap1.ValidPath.Contains(nextDesireGrid))  // else if curDirection is valid
        {
            nextGrid = nextDesireGrid;
            return;            
        }
        
        // If there's no desire path and valid path, pick a 'random' tile and move on
        var oppositeDir = -Dir2Vec[curDirection]; 
        foreach (GridDirect direction in Enum.GetValues(typeof(GridDirect)))
        {
            if (Dir2Vec[direction] == oppositeDir) continue;  // prevent going back to the coming direction 
            nextDesireGrid = Position + CellSize * Dir2Vec[direction];
            if (!PacmanMap1.ValidPath.Contains(nextDesireGrid)) continue;
            nextGrid = nextDesireGrid;
            curDirection = direction;
            return;
        }
    }

    private void MoveGridStep(int steps)
    {
        while (steps > 0)
        {
            if (Position == nextGrid)
            {
                if (CheckSpecialPath()) return;  // if we're at the exit, stop
                CheckNextPath();
                SpriteFaceDirection(curDirection);// Update pacman rotation
            }
            Position += Dir2Vec[curDirection];  // update 1 pixel every loop
            steps--;
        }
    }

    public void SetUpPin()
    {
        GlobalPosition = parentPin.GetNode<Position2D>("PacmanInitPos").GlobalPosition;
        parentOffset = GlobalPosition - parentPin.GlobalPosition;

        curPlayer = parentPin.GetParent<PinballMap>().playerType; 
        SpriteFaceDirection(curPlayer == World.Player.Left ? GridDirect.Right : GridDirect.Left);
        parentPin.GetParent<PinballMap>().curPacmanStill = this; // call this before setting still ball to trigger setter
        parentPin.GetParent<PinballMap>().hasStillBall = true;

        pacmanSprite.Play(curPlayer == World.Player.Left ? "Player1 Move" : "Player2 Move");  // Change sprite colour
        
        playerStrPrefix = curPlayer == World.Player.Left ? "player1" : "player2";
    }

    private void SpriteFaceDirection(GridDirect face)
    {
        switch (face)
        {
            case GridDirect.Up:
                spritePivot.RotationDegrees = -90;
                break;
            case GridDirect.Down:
                spritePivot.RotationDegrees = 90;
                break;
            case GridDirect.Right:
                spritePivot.RotationDegrees = 0;
                break;
            case GridDirect.Left:
                spritePivot.RotationDegrees = 180;
                break;
        }
    }

    private void _on_Pacman_area_entered(Area2D node)
    {
        if (node.IsInGroup("pacmanPoint"))
        {
            var point = ((PacmanPoint) node);
            World.Player2Stat[curPlayer].health += point.healthRestorePoint;
            World.Player2Stat[curPlayer].bulletCount += point.bulletRestorePoint;
            World.Player2Stat[curPlayer].pacmanSpeed += point.speedRestorePoint;
            point.Destroy();
            EmitSignal(nameof(CollectedPoint), false);
        }
        else if (node.IsInGroup("pacmanEnemy"))
        {
            switch (((Ghost)node).enemyState)
            {
                case Ghost.GhostState.NormalMoving:
                    World.Player2Stat[curPlayer].health -= Ghost.contactDamage;
                    EmitSignal(nameof(StartTimeLimitCountdown), PinballMaxTime, curPlayer);
                    PacmanDie();
                    break;
                case Ghost.GhostState.Fear:
                    World.Player2Stat[curPlayer].health += Ghost.contactDamage;
                    ((Ghost)node).KillGhost();
                    break;
            }
        }
        else if (node.IsInGroup("pacmanOutBound"))
        {
            if (curState == PacmanState.Still) return;

            // check if it's same person or another person
            var boundPlayer = node.GetParent<PinballMap>().playerType; 
            if (boundPlayer == curPlayer)  // if it's the same person deal more damage
            {
                World.Player2Stat[boundPlayer].health -= CatchRecoverHealth;
                World.Player2Stat[boundPlayer].bulletCount -= CatchRecoverBullet;
            }
            else World.Player2Stat[boundPlayer].health -= OutOfBoundDamage;

            parentPin = node.GetNode<Area2D>("../player");
            EmitSignal(nameof(StartTimeLimitCountdown), PinballMaxTime, boundPlayer);
            PacmanDie();
        }
        else if (node.IsInGroup("pinPlayer"))
        {
            if (curState == PacmanState.Still) return;
            var boundPlayer = node.GetParent<PinballMap>().playerType;
            if (boundPlayer == curPlayer)
            {
                World.Player2Stat[boundPlayer].health -= CatchRecoverHealth;
                World.Player2Stat[boundPlayer].bulletCount -= CatchRecoverBullet;
            }
            else
            {
                World.Player2Stat[boundPlayer].health += CatchRecoverHealth;
                World.Player2Stat[boundPlayer].bulletCount += CatchRecoverBullet;
            }
            
            parentPin = node;
            EmitSignal(nameof(StartTimeLimitCountdown), PinballMaxTime, boundPlayer);
            PacmanDie();
        }
        else if (node.IsInGroup("pacmanPowerup"))
        {
            EmitSignal(nameof(CollectedPoint), true);
            ((PacmanPowerup) node).InteractPowerUp();
            ((PacmanPowerup) node).Destroy();
        }
    }

    private void PacmanDie()
    {
        curState = PacmanState.Still;

        // disable area2d collision and move
        _collisionSoundFX.Play();
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
        forwardAnimation = true;
        _animationPlayer.Play("Pacman Die");
        EmitSignal(nameof(PacmanDieSignal), GlobalPosition);
        GetNode<Trail2D>("Trail2D").PauseTrail();
    }

    private void _on_AnimationPlayer_animation_finished(string animName)
    {
        if (forwardAnimation)
        {
            forwardAnimation = false;
            SetUpPin();
            _animationPlayer.PlayBackwards("Pacman Die");
            GetNode<Trail2D>("Trail2D").ResumeTrail();
        }
    }

    private void _linger_time_out()
    {
        switch (curState)
        {
            case PacmanState.Still:
                curState = PacmanState.FlyingToGrid;
                EmitSignal(nameof(StartTimeLimitCountdown), InGridMaxTime, curPlayer);
                World.Player2Stat[curPlayer].health -= 20;
                World.Player2Stat[curPlayer].bulletCount -= 10;
                EmitSignal(nameof(BallLeave));
                accumGridTime = 0;
                break;
            case PacmanState.InGrid:
                EmitSignal(nameof(StartTimeLimitCountdown), PinballMaxTime, curPlayer);
                World.Player2Stat[curPlayer].health -= 60;
                World.Player2Stat[curPlayer].bulletCount -= 20;
                PacmanDie();
                break;
        }
    }
}
