using Godot;
using System;

public partial class DotEffect : StatusEffect
{
    [Export] AttackInfo dot;
    [Export] bool affectedByUserStats;

    public override void _Ready()
    {
        if (affectedByUserStats) dot = giver.Attack(dot, null, true);
    }

    public override void ApplyEffect()
    {
        receiver.HandleAttack(dot);
    }
}
