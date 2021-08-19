using Godot;
using System;
using Godot.Collections;

public class AudioManager : Node2D
{
    [Signal] public delegate void LevelIncreased();
    private int levelCount = 1;
    
    private static readonly Dictionary<int, int> LevelLoopCount = new Dictionary<int, int>
    {
        {1 , 5},
        {2 , 8},
        {3 , 1000000},  // The last one need to be very long
    };

    private int _curLoopCount = 0;
    private float _pitchScale = 1;
    private AudioStreamPlayer _pitchPlayer;
    private Timer _pitchTimer;

    public override void _Ready()
    {
        GetNode<AudioStreamPlayer>("MainMusicLevel1").Play();
        _pitchPlayer = GetNode<AudioStreamPlayer>("CollectPoint");
        _pitchTimer = GetNode<Timer>("PointPitchTimer");
    }

    private void _on_MainMusicLevel_finished()
    {
        if (GetTree().Paused)  // We're in the menu
        {
            GetNode<AudioStreamPlayer>("MainMusicLevel" + levelCount.ToString()).Play();
            return;
        }
        
        _curLoopCount++;
        if (_curLoopCount == LevelLoopCount[levelCount])
        {
            levelCount++;
            _curLoopCount = 0;
            EmitSignal(nameof(LevelIncreased));
        }
        
        GetNode<AudioStreamPlayer>("MainMusicLevel" + levelCount.ToString()).Play();
    }

    private void _on_PointPitchTimer_timeout()
    {
        _pitchScale = 1;
    }

    private void _on_Pacman_CollectedPoint()
    {
        _pitchScale += 0.02f;
        _pitchPlayer.PitchScale = _pitchScale;
        _pitchPlayer.Play();
        _pitchTimer.Start();
    }
}
