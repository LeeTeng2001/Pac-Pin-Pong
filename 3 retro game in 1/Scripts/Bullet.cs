using Godot;
using System;
using System.Runtime.Serialization;

public class Bullet : Area2D
{
    public enum BulletType {Enemy, Player}
    public World.Player playerBulletType;
    public BulletType curBulletType;
    public int bulletDirection = 1, bulletDamage;
    public float bulletSpeed = 100f;

    private int deadFxCount;
    private bool isDead;

    public override void _Ready()
    {
        if (curBulletType == BulletType.Enemy) GetNode<AnimatedSprite>("AnimatedSprite").Play("enemy bullet");
        else if (playerBulletType == World.Player.Left) GetNode<AnimatedSprite>("AnimatedSprite").Play("player1 bullet");
        else GetNode<AnimatedSprite>("AnimatedSprite").Play("player2 bullet");
        
        GetNode<AudioStreamPlayer>("BulletShot").Play();
    }

    public override void _Process(float delta)
    {
        if (isDead) return;
        Position = new Vector2(Position.x, Position.y + delta * bulletSpeed * bulletDirection);
    }

    private void _on_Bullet_area_entered(Area2D obj)
    {
        if (obj.IsInGroup("bulletDeleteRegion"))  // not playing animation to save resource
            QueueFree();
    }

    private void _on_DeadFX_finished()
    {
        if (deadFxCount != 1) deadFxCount++;
        else QueueFree();
    }

    public void BulletExplosion()
    {
        isDead = true;
        GetNode<AudioStreamPlayer>("CollisionHit").Play();
        GetNode<AnimatedSprite>("AnimatedSprite").Visible = false;
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
        GetNode<AnimatedSprite>("Explosion").Visible = true;
        GetNode<AnimatedSprite>("Explosion").Play("explosion");
    }
}
