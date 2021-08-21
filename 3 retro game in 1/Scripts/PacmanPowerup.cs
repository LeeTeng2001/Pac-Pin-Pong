using Godot;
using System;
using System.Threading;

public class PacmanPowerup : Area2D
{
    public enum PowerUpType {Eat, Invisible, Kill, QuestionMark}

    private static readonly Random MyRandom = new Random();
    private static readonly Array PowValues = Enum.GetValues(typeof(PowerUpType));
    
    private PowerUpType type;
    private bool _isSpawn = true;

    public override void _Ready()
    {
        GetRandomPower();
        switch (type)
        {
            case PowerUpType.Eat:
                GetNode<AnimatedSprite>("Pivot/Sprite").Play("Eat");
                break;
            case PowerUpType.Invisible:
                GetNode<AnimatedSprite>("Pivot/Sprite").Play("Invisible");
                break;
            case PowerUpType.Kill:
                GetNode<AnimatedSprite>("Pivot/Sprite").Play("Kill");
                break;
            case PowerUpType.QuestionMark:
                GetNode<AnimatedSprite>("Pivot/Sprite").Play("Random");
                break;
        }
        
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
        GetNode<AnimationPlayer>("AnimationPlayer").PlayBackwards("Pickup");
    }

    private void GetRandomPower()
    {
        type = (PowerUpType)PowValues.GetValue(MyRandom.Next(PowValues.Length));
    }

    public void InteractPowerUp()
    {
        // Select random powerUp
        while (type == PowerUpType.QuestionMark) GetRandomPower();

        switch (type)
        {
            case PowerUpType.Eat:
                foreach (Ghost ghostInstance in GetTree().GetNodesInGroup("pacmanEnemy"))
                {
                    ghostInstance.GainFear();
                }
                break;
            case PowerUpType.Invisible:
                foreach (Ghost ghostInstance in GetTree().GetNodesInGroup("pacmanEnemy"))
                {
                    ghostInstance.GainInvisibility();
                }
                break;
            case PowerUpType.Kill:
                foreach (Ghost ghostInstance in GetTree().GetNodesInGroup("pacmanEnemy"))
                {
                    ghostInstance.KillGhost();
                    break;
                }
                break;
        }
    }

    public void Destroy()
    {
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
        GetNode<AnimationPlayer>("AnimationPlayer").Play("Pickup");
    }

    private void _on_AnimationPlayer_animation_finished(string name)
    {
        if (!_isSpawn)
        {
            QueueFree();
            return;
        }
        _isSpawn = false;
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", false);
    }
}
