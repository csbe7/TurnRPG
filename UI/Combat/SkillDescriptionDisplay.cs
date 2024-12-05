using Godot;
using System;

public partial class SkillDescriptionDisplay : Control
{
    //Skill skill;
    Label description;
    ColorRect corner, bg;
    [Export] float cornerWidth;

    public override void _Ready()
    {
        description = GetNode<Label>("Description");
        corner = GetNode<ColorRect>("Corner");
        bg = GetNode<ColorRect>("Background");
        SetText("gwsjklnsrjkgsrgk\nwnherjogrjgb");
    }

    public void SetText(string s)
    {
        description.Text = s;
        bg.Size = description.Size;
        corner.Size = new Vector2(bg.Size.X + cornerWidth*2, bg.Size.Y + cornerWidth*2);
    } 
}
