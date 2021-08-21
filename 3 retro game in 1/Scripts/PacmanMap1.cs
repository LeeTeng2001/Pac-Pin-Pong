using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class PacmanMap1 : Node2D
{
    private static readonly PackedScene PointPackedScene = GD.Load<PackedScene>("res://Scenes/PacmanPoint.tscn");
    private static readonly PackedScene GhostPackedScene = GD.Load<PackedScene>("res://Scenes/Ghost.tscn");
    private static readonly PackedScene PowerupPackedScene = GD.Load<PackedScene>("res://Scenes/PacmanPowerup.tscn");
    private static readonly PackedScene DeathParticleScene = GD.Load<PackedScene>("res://Scenes/FX/DeathParticle.tscn");

    public static readonly HashSet<Vector2> ValidPath = new HashSet<Vector2>
    {
        new Vector2(0, 64),
        new Vector2(64, 64),
        new Vector2(128, 64),
        new Vector2(320, 64),
        new Vector2(384, 64),
        new Vector2(448, 64),
        new Vector2(512, 64),
        new Vector2(576, 64),
        new Vector2(640, 64),
        new Vector2(704, 64),
        new Vector2(768, 64),
        new Vector2(896, 64),
        new Vector2(960, 64),
        new Vector2(1024, 64),
        new Vector2(1088, 64),
        new Vector2(128, 128),
        new Vector2(192, 128),
        new Vector2(256, 128),
        new Vector2(320, 128),
        new Vector2(576, 128),
        new Vector2(768, 128),
        new Vector2(896, 128),
        new Vector2(1024, 128),
        new Vector2(0, 192),
        new Vector2(64, 192),
        new Vector2(128, 192),
        new Vector2(256, 192),
        new Vector2(320, 192),
        new Vector2(384, 192),
        new Vector2(512, 192),
        new Vector2(576, 192),
        new Vector2(640, 192),
        new Vector2(704, 192),
        new Vector2(768, 192),
        new Vector2(832, 192),
        new Vector2(896, 192),
        new Vector2(960, 192),
        new Vector2(1024, 192),
        new Vector2(1088, 192),
        new Vector2(128, 256),
        new Vector2(256, 256),
        new Vector2(384, 256),
        new Vector2(512, 256),
        new Vector2(640, 256),
        new Vector2(832, 256),
        new Vector2(960, 256),
        new Vector2(0, 320),
        new Vector2(64, 320),
        new Vector2(128, 320),
        new Vector2(192, 320),
        new Vector2(256, 320),
        new Vector2(384, 320),
        new Vector2(448, 320),
        new Vector2(512, 320),
        new Vector2(576, 320),
        new Vector2(640, 320),
        new Vector2(704, 320),
        new Vector2(832, 320),
        new Vector2(896, 320),
        new Vector2(960, 320),
        new Vector2(1024, 320),
        new Vector2(1088, 320),
        new Vector2(256, 384),
        new Vector2(512, 384),
        new Vector2(704, 384),
        new Vector2(832, 384),
        new Vector2(960, 384),
        new Vector2(0, 448),
        new Vector2(64, 448),
        new Vector2(128, 448),
        new Vector2(192, 448),
        new Vector2(256, 448),
        new Vector2(384, 448),
        new Vector2(448, 448),
        new Vector2(512, 448),
        new Vector2(704, 448),
        new Vector2(768, 448),
        new Vector2(832, 448),
        new Vector2(960, 448),
        new Vector2(1024, 448),
        new Vector2(1088, 448),
        new Vector2(192, 512),
        new Vector2(256, 512),
        new Vector2(320, 512),
        new Vector2(384, 512),
        new Vector2(512, 512),
        new Vector2(576, 512),
        new Vector2(640, 512),
        new Vector2(704, 512),
        new Vector2(832, 512),
        new Vector2(960, 512),
        new Vector2(1024, 512),
        new Vector2(0, 576),
        new Vector2(64, 576),
        new Vector2(128, 576),
        new Vector2(192, 576),
        new Vector2(384, 576),
        new Vector2(512, 576),
        new Vector2(704, 576),
        new Vector2(768, 576),
        new Vector2(832, 576),
        new Vector2(896, 576),
        new Vector2(960, 576),
        new Vector2(1024, 576),
        new Vector2(1088, 576),
        new Vector2(64, 640),
        new Vector2(192, 640),
        new Vector2(384, 640),
        new Vector2(512, 640),
        new Vector2(704, 640),
        new Vector2(832, 640),
        new Vector2(960, 640),
        new Vector2(0, 704),
        new Vector2(64, 704),
        new Vector2(128, 704),
        new Vector2(192, 704),
        new Vector2(256, 704),
        new Vector2(320, 704),
        new Vector2(384, 704),
        new Vector2(512, 704),
        new Vector2(576, 704),
        new Vector2(640, 704),
        new Vector2(704, 704),
        new Vector2(832, 704),
        new Vector2(896, 704),
        new Vector2(960, 704),
        new Vector2(1024, 704),
        new Vector2(1088, 704),
    };

    public static readonly Dictionary<World.Player, List<Vector2>> PlayerGridStart =
                            new Dictionary<World.Player, List<Vector2>>
        {
            { World.Player.Left, new List<Vector2>
                {
                    new Vector2(0, 64),
                    new Vector2(0, 192),
                    new Vector2(0, 320),
                    new Vector2(0, 448),
                    new Vector2(0, 576),
                    new Vector2(0, 704),
                }
            },
            { World.Player.Right, new List<Vector2>
                {
                    new Vector2(1088, 64),
                    new Vector2(1088, 192),
                    new Vector2(1088, 320),
                    new Vector2(1088, 448),
                    new Vector2(1088, 576),
                    new Vector2(1088, 704),
                }
            }
        };

    private static readonly Random random = new Random();
    private static readonly Vector2[] validPathArray = ValidPath.ToArray();
    private const int AmountOfBackupParticles = 6;
    private const float ProgressRecoverTime = 0.2f;

    private Sprite hintSprite;
    private AnimatedSprite timeBarSprite;
    private Vector2 nearestPos;
    private List<Particles2D> deathParticles;

    [Signal] public delegate void TimeLimitReached();
    private float accumTime, timeOutDuration, lastXScale = 1;
    private bool isPlaying;
    
    public override void _Ready()
    {
        hintSprite = GetNode<Sprite>("HintSprite");
        timeBarSprite = GetNode<AnimatedSprite>("PacmanTimeSprite");
        deathParticles = new List<Particles2D>();
        
        // WHEN USER RESET THEIR PROGRESS
        Ghost.contactDamage = 30; Ghost.movSpeed = 210; Ghost.totalGhost = 0;

        GeneratePointsAndPU();
        GenerateEnemy(2);

        // Create death particles for pacman
        var particlesInstance = AmountOfBackupParticles;        
        while (particlesInstance > 0)
        {
            particlesInstance--;
            var deathParticle = DeathParticleScene.Instance<Particles2D>();
            deathParticles.Add(deathParticle);
            AddChild(deathParticle);
        }
    }

    public override void _Process(float delta)
    {
        if (!isPlaying) return;
        
        // Process timer
        accumTime += delta;
        
        if (accumTime < ProgressRecoverTime)  // Recover progress
        {
            var deltaLength = (1 - lastXScale) * (accumTime / ProgressRecoverTime);
            timeBarSprite.Scale = new Vector2(lastXScale + deltaLength, 1);
        }
        else if (accumTime < timeOutDuration)
        {
            var percentage = 1 - (accumTime - ProgressRecoverTime) / (timeOutDuration - ProgressRecoverTime);
            timeBarSprite.Scale = new Vector2(percentage, 1);
        }
        else
        {
            timeBarSprite.Scale = new Vector2(0, 1);
            isPlaying = false;
            EmitSignal(nameof(TimeLimitReached));            
        }
    }

    private void _on_Pacman_StartTimeLimitCountdown(float duration, World.Player playerType)
    {
        timeBarSprite.Play(playerType == World.Player.Left ? "player1" : "player2");
        accumTime = 0;
        lastXScale = timeBarSprite.Scale.x;
        timeOutDuration = duration;
        isPlaying = true;
    }

    private void GenerateEnemy(int enemyCount)
    {
        // Create Random enemy pos
        var enemyPos = new List<Vector2>();
        while (enemyPos.Count < enemyCount)
        {
            var newPos = validPathArray[random.Next(validPathArray.Length)];
            if (enemyPos.Contains(newPos)) continue;
            enemyPos.Add(newPos);
        }

        foreach (var gridPos in enemyPos)
        {
            var enemyInstance = GhostPackedScene.Instance<Ghost>();
            enemyInstance.Position = gridPos;
            enemyInstance.Connect("GhostAllDead", this, nameof(AllDeadSpawnDelay));
            AddChild(enemyInstance);
            Ghost.totalGhost++;
        }
    }

    private void GeneratePointsAndPU(double powerUpThreshold = 0.02)
    {
        foreach (var gridPos in ValidPath)
        {
            if (random.NextDouble() < powerUpThreshold)  // Spawn powerUp for this percentage
            {
                var powerUpInstance = PowerupPackedScene.Instance<PacmanPowerup>();
                powerUpInstance.Position = gridPos;
                AddChild(powerUpInstance);
            }
            else 
            {
                var pointInstance = PointPackedScene.Instance<Area2D>();
                pointInstance.Position = gridPos;
                AddChild(pointInstance);
            }
        }
    }

    private void _on_PinballMap_PlayerMoved(Vector2 localPlayerPos, Pacman player)
    {
        hintSprite.Visible = true;
        var nearestVal = float.MaxValue;
        nearestPos = Vector2.Zero;
        foreach (var pos in PacmanMap1.PlayerGridStart[player.curPlayer])
        {
            if ((pos - localPlayerPos).LengthSquared() < nearestVal)
            {
                nearestVal = (pos - localPlayerPos).LengthSquared();
                nearestPos = pos;
            }
        }

        player.UpdateDesireLoc(nearestPos);
        hintSprite.Position = new Vector2(nearestPos.x + (player.curPlayer == World.Player.Left ? -64 : 64), nearestPos.y);
        hintSprite.FlipH = player.curPlayer == World.Player.Right;
    }

    private void _on_Pacman_BallLeave()
    {
        hintSprite.Visible = false;
    }

    private void _on_Pacman_PacmanDieSignal(Vector2 globPos)
    {
        foreach (var particle in deathParticles)
        {
            if (particle.Emitting) continue;
            particle.GlobalPosition = globPos;
            particle.Emitting = true;
            return;
        }
    }

    public void IncreaseDifficulty()
    {
        // Clear the remaining points and powerup
        foreach (PacmanPoint pointInstance in GetTree().GetNodesInGroup("pacmanPoint")) pointInstance.Destroy();
        foreach (PacmanPowerup powerInstance in GetTree().GetNodesInGroup("pacmanPowerup")) powerInstance.Destroy();
        
        // generate new stuff
        GenerateEnemy(2);
        GeneratePointsAndPU();
    }

    private void AllDeadSpawnDelay()
    {
        GetNode<Timer>("AddDeadTimer").Start(3);
    }

    private void _on_AddDeadTimer_timeout()
    {
        GenerateEnemy(2);
    }

    private void _on_PlayerStat_PlayerDied(World.Player playerDied)
    {
        SetProcess(false);  // Stop countdown timer when one of the player died
    }
}
