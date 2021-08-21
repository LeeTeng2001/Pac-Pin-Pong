using Godot;

public class GameStartUI : Control
{
    private enum GlobalGameState {ShowMenu, InGame}
    
    private Label infoLabel;
    private bool endGame;
    private GlobalGameState gameState = GlobalGameState.ShowMenu;
    private MainCamera mainCam;
    
    public override void _Ready()
    {
        infoLabel = GetNode<Label>("InfoLabel");
        mainCam = GetParent().GetNode<MainCamera>("PacmanMap1/Pacman/MainCamera");
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
        
        mainCam.ChangeCameraFollow(true);  // turn back to fixed camera
        
        GetNode<AnimationPlayer>("Transition").Play();
        gameState = GlobalGameState.ShowMenu;
        endGame = true;
    }

    private void HideOverlay()
    {
        mainCam.ChangeCameraFollow(false);  // turn camera to follow mode
        
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
