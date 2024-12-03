using Godot;
using System;

[GlobalClass]
public partial class SkillEffect : Resource
{
    [Export] public Color color = new Color(0, 0, 0);
    [Export] public Curve fadeCurve;
}
