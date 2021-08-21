using Godot;
using Godot.Collections;

public class AudioManager : Node2D
{
    [Signal] public delegate void LevelIncreased();
    private int levelCount = 1;
    
    private static readonly Dictionary<int, int> LevelLoopCount = new Dictionary<int, int>
    {
        {1 , 4},
        {2 , 5},
        {3 , 5},
        {4 , 6},
        {5 , 100000},  // The last one need to be very long
    };

    private int _curLoopCount = 0;
    private float _pitchScale = 1;
    private AudioStreamPlayer _pitchPlayerSingle, _pitchPlayerPowerUp;
    private Timer _pitchTimer;

    public override void _Ready()
    {
        GetNode<AudioStreamPlayer>("MainMusicLevel1").Play();
        _pitchPlayerSingle = GetNode<AudioStreamPlayer>("CollectPoint");
        _pitchPlayerPowerUp = GetNode<AudioStreamPlayer>("CollectPointPowerup");
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
        if (_curLoopCount == LevelLoopCount[levelCount])  // go to next level
        {
            levelCount++;
            _curLoopCount = 0;
            EmitSignal(nameof(LevelIncreased));
            GetNode<AudioStreamPlayer>("MainMusicTransition").Play();  // Transition music
        }
        
        GetNode<AudioStreamPlayer>("MainMusicLevel" + levelCount.ToString()).Play();
    }

    private void _on_PointPitchTimer_timeout()
    {
        _pitchScale = 1;
        GetNode<AudioStreamPlayer>("CollectPointFinish").Play();
    }

    private void _on_Pacman_CollectedPoint(bool isPowerUp)
    {
        _pitchScale += 0.02f;
        if (isPowerUp)
        {
            _pitchPlayerPowerUp.PitchScale = _pitchScale;
            _pitchPlayerPowerUp.Play();
        }
        else
        {
            _pitchPlayerSingle.PitchScale = _pitchScale;
            _pitchPlayerSingle.Play();
        }
        _pitchTimer.Start();
    }
}
