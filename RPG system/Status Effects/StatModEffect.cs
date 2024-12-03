using Godot;
using System;

public partial class StatModEffect : StatusEffect
{
    [Export] string targetStat;
    [Export] StatModifier mod;

    public override void _Ready()
    {
        base._Ready();
        
        Stat s = (Stat)receiver.statBlock.Get(targetStat);
        s.AddModifier(mod);
    }

    public override void EndEffect()
    {
        base.EndEffect();
        Stat s = (Stat)receiver.statBlock.Get(targetStat);
        s.RemoveModifier(mod);
    }
}
