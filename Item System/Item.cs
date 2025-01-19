using Godot;
using System;

[GlobalClass]
public partial class Item : Resource
{
    [Export] public string name;
    [Export(PropertyHint.MultilineText)] public string description;
    [Export] public int maxStack = 1;
    [Export] public int value;
    [Export] public float shopProbability = 1;

    public enum UseTarget{
        any,
        self,
    }

    [Export] UseTarget useTarget;
}
