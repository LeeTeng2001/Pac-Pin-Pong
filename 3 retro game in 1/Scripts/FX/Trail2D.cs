using Godot;

public class Trail2D : Line2D
{
    [Export] private int trailLength = 30;

    private bool isStopped = false;
    private Vector2 point;

    private Tween trailTween;

    public override void _Ready()
    {
        trailTween = GetNode<Tween>("Tween");
    }

    public override void _Process(float delta)
    {
        GlobalPosition = Vector2.Zero;
        GlobalRotation = 0;

        if (isStopped) return;
        point = GetParent<Node2D>().GlobalPosition + 32 * Vector2.One;  // Offset to center
        AddPoint(point);
        while (GetPointCount() > trailLength)  RemovePoint(0);
    }

    public void PauseTrail()
    {
        isStopped = true;
        trailTween.InterpolateProperty(this, "modulate:a", 1f, 0f, 0.3f);
        trailTween.Start();
    }

    public void ResumeTrail()
    {
        ClearPoints();
        isStopped = false;
        trailTween.InterpolateProperty(this, "modulate:a", 0f, 1f, 0.3f);
        trailTween.Start();
    }
}
