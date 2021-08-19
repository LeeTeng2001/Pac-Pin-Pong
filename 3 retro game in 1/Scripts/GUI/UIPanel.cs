using Godot;
using System;

public class UIPanel : Control
{
    private BtmCounterUI p1HealthUI, p1BulletUI, p2BulletUI, p2HealthUI;
    
    public override void _Ready()
    {
        p1HealthUI = GetNode<BtmCounterUI>("BtmContainer/Player1/HealthUI");
        p2HealthUI = GetNode<BtmCounterUI>("BtmContainer/Player2/HealthUI");
        p1BulletUI = GetNode<BtmCounterUI>("BtmContainer/Player1/BulletUI");
        p2BulletUI = GetNode<BtmCounterUI>("BtmContainer/Player2/BulletUI");
    }

    private void _on_PlayerStat_HealthChange(World.Player playerType, int changeTo)
    {
        if (playerType == World.Player.Left) p1HealthUI.ChangeValue(changeTo);
        else p2HealthUI.ChangeValue(changeTo);
    }

    private void _on_PlayerStat_BulletCountChange(World.Player playerType, int changeTo)
    {
        if (playerType == World.Player.Left) p1BulletUI.ChangeValue(changeTo);
        else p2BulletUI.ChangeValue(changeTo);
    }
}
