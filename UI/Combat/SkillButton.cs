using Godot;
using System;

public partial class SkillButton : Control
{
    public Skill skill;

    public Button button;
    Label label;

    [Signal] public delegate void SkillButtonDownEventHandler(SkillButton self);


    public override void _Ready()
    {
        button = GetNode<Button>("%Button");
        label = GetNode<Label>("%Label");

        button.ButtonDown += OnButtonDown;
    }

    public void LoadSkill(Skill s)
    {
        skill = s;
        label.Text = s.name;

    }

    void OnButtonDown()
    {
        EmitSignal(SignalName.SkillButtonDown, this);
    }

}
