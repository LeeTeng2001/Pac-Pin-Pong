using Godot;
using System;

public class PacmanPoint : Area2D
{
    public int healthRestorePoint = 2, bulletRestorePoint = 1, speedRestorePoint = 1;
    private bool _isSpawn = true;

    public override void _Ready()
    {
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
        GetNode<AnimationPlayer>("AnimationPlayer").Play("Spawn");
    }
    
    public void Destroy()
    {
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
        GetNode<AnimationPlayer>("AnimationPlayer").PlayBackwards("Spawn");
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
