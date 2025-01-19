using Godot;
using System;

public partial class SkillButton : Button
{
    public Skill skill;

    Label label;

    [Signal] public delegate void SkillButtonDownEventHandler(SkillButton self);

    [Export] float showDescTime = 0.3f;
    float showDescTimer;

    [Export] PackedScene descDisplay;
    SkillDescriptionDisplay descDisplayInstance;

    public override void _Ready()
    {
        label = GetNodeOrNull<Label>("%Label");

        ButtonDown += () => EmitSignal(SignalName.SkillButtonDown, this);;
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;

        if (!IsInstanceValid(descDisplay)) GD.Print("invalid packed scene");

        SetProcess(false);
    }

    public override void _Process(double delta)
    {  
        showDescTimer += (float)delta;
        if (showDescTimer >= showDescTime)
        {
            /*descDisplayInstance = descDisplay.Instantiate<SkillDescriptionDisplay>();
            AddChild(descDisplayInstance);
            descDisplayInstance.SetText(skill.GetDescription());*/
            SetProcess(false);
        }
    }

    private void OnMouseEntered()
    {
        showDescTimer = 0;
        SetProcess(true);
    }

    private void OnMouseExited()
    {
        if (IsInstanceValid(descDisplayInstance)) descDisplayInstance.QueueFree();
        SetProcess(false);
    }


    public void LoadSkill(Skill s)
    {
        skill = s;
        if (IsInstanceValid(label)) label.Text = s.name;

    }

}
