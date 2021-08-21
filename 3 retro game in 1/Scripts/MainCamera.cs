using Godot;
using System;

public class MainCamera : Camera2D
{
    private static Random MyRandom = new Random();
    
    private Tween _tweenMenuHelper, _tweenShaker; 
    private Timer _shakeTimer;
    
    private int _initLimitTop, _initLimitLeft;
    private float shakeAmount;
    private Vector2 _defaultCamOffset;

    public override void _Ready()
    {
        _tweenShaker = GetNode<Tween>("TweenShake");
        _tweenMenuHelper = GetNode<Tween>("TweenMenuHelper");
        _shakeTimer = GetNode<Timer>("ShakeTimer");
        _defaultCamOffset = Offset;
        
        SetProcess(false);
        
        _initLimitLeft = LimitLeft;
        _initLimitTop = LimitTop;
        Offset = new Vector2(-_initLimitLeft, -_initLimitTop);  // Go back to original position
    }

    public override void _Process(float delta)
    {
        var randX = (float) MyRandom.NextDouble() * 2 * shakeAmount - shakeAmount;
        var randY = (float) MyRandom.NextDouble() * 2 * shakeAmount - shakeAmount;
        Offset = new Vector2(randX, randY) * delta + _defaultCamOffset;
    }

    private void Shake(float newShake, float shakeTime = 0.4f, float shakeLimit = 100f)
    {
        shakeAmount += newShake;
        shakeAmount = Mathf.Min(shakeAmount, shakeLimit);
        
        
        _tweenShaker.StopAll();
        SetProcess(true);
        _shakeTimer.Start(shakeTime);
    }

    private void _on_ShakeTimer_timeout()
    {
        shakeAmount = 0;
        SetProcess(false);

        _tweenShaker.InterpolateProperty(this, "offset", Offset, _defaultCamOffset, 0.15f, Tween.TransitionType.Quad);
        _tweenShaker.Start();
    }

    public void ChangeCameraFollow(bool isFixedCameraMovement)
    {
        if (!isFixedCameraMovement)
        {
            _tweenMenuHelper.InterpolateProperty(this, "offset", 
                Offset, Vector2.Zero, .5f);
            _tweenMenuHelper.Start();
        }
        else
        {
            _tweenMenuHelper.InterpolateProperty(this, "offset", 
                Offset, new Vector2(-_initLimitLeft, -_initLimitTop), .5f);
            _tweenMenuHelper.Start();
        }
    }

    private void _on_Pacman_PacmanDieSignal(Vector2 _)
    {
        Shake(300, 0.6f, 500);
    }

    private void _on_PinballMap_PlayerGotHit()
    {
        Shake(150, 0.5f, 500);
    }
}
