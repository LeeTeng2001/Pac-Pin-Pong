using Godot;

public class GameStartUI : Control
{
    private enum GlobalGameState {ShowMenu, InGame}
    
    private Label infoLabel;
    private bool endGame;
    private GlobalGameState gameState = GlobalGameState.ShowMenu;
    private Camera2D mainCam;
    private int configLimitTop, configLimitLeft;
    
    public override void _Ready()
    {
        infoLabel = GetNode<Label>("InfoLabel");
        mainCam = GetParent().GetNode<Camera2D>("PacmanMap1/Pacman/Camera2D");
        configLimitLeft = mainCam.LimitLeft;
        configLimitTop = mainCam.LimitTop;
        mainCam.Offset = new Vector2(-configLimitLeft, -configLimitTop);
    }

    // Debug
    public override void _Process(float delta)
    {
        if (gameState != GlobalGameState.ShowMenu) return;
        
        if (Input.IsActionJustPressed("gamestartTrigger"))
        {
            if (endGame) RestartGame();  // Restart Game
            HideOverlay();
        }
    }

    public void ShowOverlay(string winner)
    {
        GetTree().Paused = true;
        infoLabel.Text = winner;
        GetNode<Label>("StartLabel").Text = "Press Space To Restart";
        var tween = GetNode<Tween>("TweenWhole");
        tween.InterpolateProperty(this, "modulate:a", 0f, 1f, 0.5f);
        tween.Start();
        var tween1 = new Tween();
        AddChild(tween1);
        tween1.InterpolateProperty(mainCam, "offset", mainCam.Offset, new Vector2(-configLimitLeft, -configLimitTop), .5f);
        tween1.Start();
        GetNode<AnimationPlayer>("Transition").Play();
        gameState = GlobalGameState.ShowMenu;
        endGame = true;
    }

    private void HideOverlay()
    {
        var tween1 = new Tween();
        AddChild(tween1);
        tween1.InterpolateProperty(mainCam, "offset", mainCam.Offset, Vector2.Zero, .5f);
        tween1.Start();
        
        gameState = GlobalGameState.InGame;
        var tween = GetNode<Tween>("TweenWhole");
        tween.InterpolateProperty(this, "modulate:a", 1f, 0f, 0.7f);
        tween.Start();
    }

    private void _on_TweenWhole_tween_all_completed()
    {
        if (!endGame) GetTree().Paused = false;  // start game when transition is finished
    }

    private void _on_Transition_animation_finished(string name)
    {
        GetNode<AnimationPlayer>("InfoTextAnimation").Play("GlowAnimation");
    }

    private void RestartGame()
    {
        var tween = new Tween();
        AddChild(tween);
        tween.InterpolateMethod(this, nameof(GameOverFadeOutToReload), 1f, 0f, 1.5f);
        tween.Start();
    }

    private void GameOverFadeOutToReload(float percentage)
    {
        if ((1 - percentage) > 0.95) GetTree().ReloadCurrentScene();

        Modulate = new Color(percentage, percentage, percentage, 1);
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), - (1 - percentage) * 60);
    }
}
