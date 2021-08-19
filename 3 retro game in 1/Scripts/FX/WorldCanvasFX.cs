using Godot;

public class WorldCanvasFX : CanvasLayer
{
    private void _on_AudioManager_LevelIncreased()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("DistortionShaderFX");
    }   
}
