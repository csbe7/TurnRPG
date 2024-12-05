using Godot;
using System;

public partial class StatModEffect : StatusEffect
{
    [Export] Godot.Collections.Dictionary<string, StatModifier> statMods = new Godot.Collections.Dictionary<string, StatModifier>();

    public override void _Ready()
    {
        base._Ready();
        
        foreach(string targetStat in statMods.Keys)
        {
            Stat s = (Stat)receiver.statBlock.Get(targetStat);
            s.AddModifier(statMods[targetStat]);
        }
    }

    public override void EndEffect()
    {
        foreach(string targetStat in statMods.Keys)
        {
            Stat s = (Stat)receiver.statBlock.Get(targetStat);
            s.RemoveModifier(statMods[targetStat]);
        }
        base.EndEffect();
    }
}
