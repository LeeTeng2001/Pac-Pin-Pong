using Godot;
using System;
using System.Collections.Generic;

public class BtmCounterUI : HBoxContainer
{
    private static readonly Texture AddHealthTex = GD.Load<Texture>("res://Assets/Add Prompt.png");
    private static readonly Texture MinusHealthTex = GD.Load<Texture>("res://Assets/Minus Prompt.png");
    private static readonly PackedScene AddMinusParticleScene = GD.Load<PackedScene>("res://Scenes/GUI/AddMinusParticle.tscn");

    enum UItype {Health, Bullet}
    [Export] private UItype typeUI;

    private Label textLabel;
    private AnimationPlayer animationPlayer;
    private Tween textTween;
    private Position2D particlePos;
    private List<Particles2D> particleList;

    private int finalValue, curValue; 
    
    public override void _Ready()
    {
        textLabel = GetNode<Label>("Count");
        textTween = GetNode<Tween>("Count/Tween");
        particlePos = GetNode<Position2D>("EmissionPosition");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        CreateParticles(25);
        var val = typeUI == UItype.Bullet ? PlayerStat.InitBulletCount : PlayerStat.InitHealth;
        finalValue = val;
        curValue = val;
        ChangeValueTween(val);
    }

    // Debug Testing
    // public override void _Process(float delta)
    // {
    //     if (Input.IsActionJustPressed("player1PinUp")) ChangeValue(10);
    //     if (Input.IsActionJustPressed("player1PinDown")) ChangeValue(-10);
    // }

    private void CreateParticles(int counts)
    {
        particleList = new List<Particles2D>();
        while (counts > 0)
        {
            counts--;
            var newParticle = AddMinusParticleScene.Instance<Particles2D>();
            particlePos.AddChild(newParticle);
            newParticle.Position = Vector2.Zero;
            particleList.Add(newParticle);
        }
    }

    public void ChangeValue(int changeTo)
    {
        var amt = changeTo - finalValue;
        if (amt == 0) return;
        
        foreach (var particle in particleList)
        {
            if (particle.Emitting) continue;
            var posAmt = (int)(1.2 * amt);
            particle.Amount = posAmt < 0 ? -posAmt : posAmt;
            particle.Texture = amt > 0 ? AddHealthTex : MinusHealthTex;
            particle.Emitting = true;
            break;
        }

        finalValue += amt;
        textTween.InterpolateMethod(this, nameof(ChangeValueTween), curValue, finalValue, 0.7f);
        textTween.Start();
        animationPlayer.Play("ScaleHeart");
    }

    private void ChangeValueTween(int amount)
    {
        curValue = amount;
        var hlstr = amount.ToString();
        while (hlstr.Length < 4) hlstr = " " + hlstr;
        textLabel.Text = hlstr;
    }
}
