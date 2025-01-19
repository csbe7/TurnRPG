using Godot;
using System;

[GlobalClass]
public partial class EquippableItem : Item
{
    public enum Slot{
        armor,
        weapon,
        accessory,
    }
    [Export] public Slot slot;

    [Export] public Godot.Collections.Dictionary<string, StatModifier> statModifiers = new Godot.Collections.Dictionary<string, StatModifier>();
}
