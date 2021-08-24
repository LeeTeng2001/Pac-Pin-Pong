using Godot;
using System;
using System.Collections.Generic;

public class Ghost : Area2D
{
    private enum GridDirect {Up, Down, Left, Right};
    public enum GhostState {Still, NormalMoving, Fear, Invisible, Dead}

    [Signal] public delegate void GhostAllDead();
    
    private static readonly Godot.Collections.Dictionary<GridDirect, Vector2> Dir2Vec = new Godot.Collections.Dictionary<GridDirect, Vector2>
    {
        { GridDirect.Up, new Vector2(0, -1) },
        { GridDirect.Down, new Vector2(0, 1) },
        { GridDirect.Left, new Vector2(-1, 0) },
        { GridDirect.Right, new Vector2(1, 0) },
    };

    public static int contactDamage = 30, movSpeed = 210, totalGhost = 0;
    public GhostState enemyState;
    
    private GridDirect curDirection;
    private float movPixelTime, accumGridTime = 0;
    private Vector2 nextGrid;
    private AnimatedSprite ghostSprite;
    private Timer effectTimer;
    private AnimationPlayer animPlayer;
    
    private const int CellSize = 64;
    private readonly Random random = new Random();
    private static readonly Array directionValue = Enum.GetValues(typeof(GridDirect));

    public override void _Ready()
    {
        effectTimer = GetNode<Timer>("EffectTimer");
        animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        ghostSprite = GetNode<AnimatedSprite>("RotationPivot/GhostSprite");

        Start();
    }

    public override void _Process(float delta)
    {
        if (enemyState == GhostState.Dead || enemyState == GhostState.Still) return;
        
        movPixelTime = 1 / (float) movSpeed;  // Update speed because speed might gradually vary
        accumGridTime += delta;
        var movStep = 0;
        while (accumGridTime > movPixelTime)
        {
            accumGridTime -= movPixelTime;
            movStep++;
        }
        MoveGridStep(movStep);
    }

    private void Start()
    {
        animPlayer.Play("Spawn");
        // Choose a random direction
        curDirection = (GridDirect)directionValue.GetValue(random.Next(directionValue.Length));
        CheckNextPath();
        GainInvisibility(2.5f);  // Spawn invisibility
    }

    public void GainInvisibility(float duration = 4f)
    {
        if (enemyState == GhostState.Dead) return;  // Guard
        enemyState = GhostState.Invisible;
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
        ghostSprite.Play("Normal");
        Modulate = new Color(1, 1, 1, 0.5f);
        effectTimer.Start(duration);
    }

    public void GainFear(float duration = 4f)
    {
        if (enemyState == GhostState.Dead) return;  // Guard
        // To restore status first 
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", false);
        Modulate = new Color(1, 1, 1, 1);
        
        enemyState = GhostState.Fear;
        ghostSprite.Play("Fear");
        effectTimer.Start(duration);
    }

    public void KillGhost()
    {
        if (enemyState == GhostState.Dead) return;  // Guard

        GetNode<AudioStreamPlayer>("Dead").Play();
        totalGhost--;
        if (totalGhost <= 0) EmitSignal(nameof(GhostAllDead)); // tell world to respawn
        
        if (enemyState == GhostState.Fear) ghostSprite.Play("Fear Die");
        else ghostSprite.Play("Normal Die");
        enemyState = GhostState.Dead;
        animPlayer.PlayBackwards("Spawn");
    }

    private void CheckNextPath()
    {
        var nextDesireGrid = Position + CellSize * Dir2Vec[curDirection];
        if (PacmanMap1.ValidPath.Contains(nextDesireGrid))  // else if curDirection is valid
        {
            nextGrid = nextDesireGrid;
            return;
        }
        
        // If there's no desire path, pick a random tile and move on
        var oppositeDir = curDirection == GridDirect.Up ? GridDirect.Down :
                                    curDirection == GridDirect.Down ? GridDirect.Up :
                                    curDirection == GridDirect.Left ? GridDirect.Right : GridDirect.Left;
        var possiblePath = new List<(GridDirect, Vector2)>();
        foreach (GridDirect direction in directionValue)
        {
            if (direction == oppositeDir) continue;  // Doesn't prefer opposite position if possible 
            nextDesireGrid = Position + CellSize * Dir2Vec[direction];
            if (!PacmanMap1.ValidPath.Contains(nextDesireGrid)) continue;
            possiblePath.Add((direction, nextDesireGrid));
        }
        
        // Pick random and return
        if (possiblePath.Count > 0)
        {
            var randomDirection = possiblePath[random.Next(possiblePath.Count)];
            nextGrid = randomDirection.Item2;
            curDirection = randomDirection.Item1;
        }
        else
        {
            curDirection = oppositeDir;
            nextGrid = Position + CellSize * Dir2Vec[curDirection];
        }
    }

    private void MoveGridStep(int steps)
    {
        while (steps > 0)
        {
            if (Position == nextGrid)
            {
                CheckNextPath();
                // SpriteFaceDirection(curDirection);// Update ghost rotation
            }
            Position += Dir2Vec[curDirection];  // update 1 pixel every loop
            steps--;
        }
    }

    private void _on_AnimationPlayer_animation_finished(string aniName)
    {
        if (enemyState == GhostState.Dead) QueueFree();  // Die then clenaup
        else if (enemyState == GhostState.Still) enemyState = GhostState.NormalMoving;  // Spawn then continue moving
    }

    private void _on_EffectTimer_timeout()  // Restore normal status
    {
        enemyState = GhostState.NormalMoving;
        Modulate = new Color(1, 1, 1, 1f);
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", false);
        ghostSprite.Play("Normal");
    }
    
    public static void IncreaseDifficulty()
    {
        // GD.Print("Ghost difficulty increase");
        contactDamage += 20;
        movSpeed += 20;
    }
}
