using Godot;
using System;
using Godot.Collections;

public class World : Node2D
{
    public enum Player {Left, Right};
    
    public static Dictionary<Player, PlayerStat> Player2Stat = new Dictionary<Player, PlayerStat>();
    
    private InvaderSpawner spawner1, spawner2;
    private GameStartUI mainMenu;
    
    public override void _Ready()
    {
        mainMenu = GetNode<GameStartUI>("GameStartUI");
        GetNode<Pacman>("PacmanMap1/Pacman").parentPin = GetNode<Area2D>("PinballMap/player");
        GetNode<Pacman>("PacmanMap1/Pacman").SetUpPin();

        spawner1 = GetNode<InvaderSpawner>("InvaderSpawner1");
        spawner2 = GetNode<InvaderSpawner>("InvaderSpawner2");
        
        // Initialize player stat reference
        GetNode<PlayerStat>("PlayerStat1").Init();
        GetNode<PlayerStat>("PlayerStat2").Init();
        Player2Stat[Player.Left] = GetNode<PlayerStat>("PlayerStat1");
        Player2Stat[Player.Right] = GetNode<PlayerStat>("PlayerStat2");

        GetTree().Paused = true;  // pause the game
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), 0);  // Reset volume
    }

    private void _on_PlayerStat_PlayerDied(Player playerDied)
    {
        var whoWin = playerDied == Player.Left ? "Player2 win!" : "Player1 win!";
        mainMenu.ShowOverlay(whoWin);
    }

    private void _on_AudioManager_LevelIncreased()
    {
        spawner1.IncreaseDifficulty();
        spawner2.IncreaseDifficulty();
        Player2Stat[Player.Left].IncreaseDifficulty();
        Player2Stat[Player.Right].IncreaseDifficulty();
        GetNode<PacmanMap1>("PacmanMap1").IncreaseDifficulty();
        Ghost.IncreaseDifficulty();
        Pacman.InGridMaxTime -= 0.2f;
        Pacman.PinballMaxTime -= 0.4f;
    }
}
