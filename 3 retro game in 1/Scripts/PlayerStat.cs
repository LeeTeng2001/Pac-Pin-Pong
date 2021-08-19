using Godot;
using System;

public class PlayerStat : Node2D
{
    [Signal] public delegate void HealthChange(World.Player playerType, int changeTo);
    [Signal] public delegate void BulletCountChange(World.Player playerType, int changeTo);
    [Signal] public delegate void PlayerDied(World.Player playerType);
    
    [Export] public World.Player playerType;
    public int health {
        get => _health;
        set
        {
            if (value <= 0)
            {
                value = 0;
                EmitSignal(nameof(PlayerDied), playerType);
            }
            EmitSignal(nameof(HealthChange), playerType, value);
            _health = value;
        }
    }
    public int bulletCount {
        get => _bulletCount;
        set
        {
            if (value < 0) value = 0;
            EmitSignal(nameof(BulletCountChange), playerType, value);
            _bulletCount = value;
        }
    }
    public int fireRateInMS {
        get => _fireRateInMS;
        set
        {
            // TODO: Emit change fire rate signal
            _fireRateInMS = value;
        }
    }

    public int pinBallSpeed, pacmanSpeed;

    public const int InitHealth = 110, InitBulletCount = 14, InitPlayerFireRateInMS = 800, InitPinBallSpeed = 400;
    public const int InitPacmanSpeed = 400;

    private int _health, _bulletCount, _fireRateInMS;

    public void Init()
    {
        _bulletCount = InitBulletCount;
        _health = InitHealth;
        _fireRateInMS = InitPlayerFireRateInMS;
        pacmanSpeed = InitPacmanSpeed;
        pinBallSpeed = InitPinBallSpeed;
    }

    public void IncreaseDifficulty()
    {
        GD.Print("Player difficulty increase");
        pinBallSpeed += 150;
        pacmanSpeed += 80;
        fireRateInMS -= 200;
    }
}
