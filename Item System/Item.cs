using Godot;
using System;

[GlobalClass]
public partial class Item : Resource
{
    [Export] public string name;
    [Export(PropertyHint.MultilineText)] public string description;
    [Export] int maxStack;

    public enum UseTarget{
        any,
        self,
    }

    [Export] UseTarget useTarget;
}
