using Godot;
using System;

public class InvaderSpawner : Node2D
{
    private static readonly PackedScene InvaderScene = GD.Load<PackedScene>("res://Scenes/SpaceInvaderEnemy.tscn");
    private static readonly Random random = new Random();

    [Export] public int maxLocalY = 64 * 11, maxLocalX = 64 * 3;

    private Timer spawnTimer;
    private Position2D invaderSpawnLoc;
    
    // Difficulty Setting -----------------------------------
    private const float rndTimeMax = 1.5f, shootingRangeRandom = 0.15f;
    private float bulletSpeed = 100f;
    private float enemyMovDownSpeed = 50f, enemyMovHoriSpeed = 80f, shootingRate = 0.35f;
    private float spawnTimeInterval = 5f;
    private int enemyHealth = 10, bulletDamage = 10;

    public override void _Ready()
    {
        spawnTimer = GetNode<Timer>("SpawnTimer");
        invaderSpawnLoc = GetNode<Position2D>("InvaderStartPoint");
        
        spawnTimer.Start(8);  // the first spawn should be longer
    }

    public void IncreaseDifficulty()
    {
        // GD.Print("Invader Difficulty increase!");
        enemyHealth += 5;
        enemyMovDownSpeed += 20f;
        enemyMovHoriSpeed += 15f;
        shootingRate += 0.15f;
        bulletSpeed += 50f;
        bulletDamage += 5;

        spawnTimeInterval = spawnTimeInterval > 0.6f ? spawnTimeInterval - 0.4f : 0.6f;
    }

    private void _on_SpawnTimer_timeout()
    {
        SpawnWave();
        spawnTimer.Start(spawnTimeInterval + (float) random.NextDouble() * rndTimeMax);
    }

    private void SpawnWave()
    {
        var enemyInstance = InvaderScene.Instance<SpaceInvaderEnemy>();
        enemyInstance.Position = invaderSpawnLoc.Position;
        enemyInstance.movDownSpeed = enemyMovDownSpeed;
        enemyInstance.movHorizonSpeed = enemyMovHoriSpeed;
        enemyInstance.health = enemyHealth;
        enemyInstance.shootingRate = shootingRate + (random.NextDouble() < 0.5 ? -1 : 1) * (float) random.NextDouble() * shootingRangeRandom;
        enemyInstance.bulletSpeed = bulletSpeed;
        enemyInstance.bulletDamage = bulletDamage;
        AddChild(enemyInstance);
        enemyInstance.RandomizeStat();
    }
}
