using Godot;
using System;

public partial class ActionDrawer : Control
{
    public Sheet sheet;
    [Export] PackedScene skill_button;
    GridContainer grid;

    public SkillButton attackButton, defendButton, restButton;

    [Signal] public delegate void SkillSelectedEventHandler(Skill skill);

    public override void _Ready()
    {
        grid = GetNode<GridContainer>("%Skill Grid");

        attackButton = GetNode<SkillButton>("%Attack Button");
        defendButton = GetNode<SkillButton>("%Defend Button");
        restButton = GetNode<SkillButton>("%Rest Button");

        attackButton.SkillButtonDown += OnSkillButtonDown;
        defendButton.SkillButtonDown += OnSkillButtonDown;
        restButton.SkillButtonDown += OnSkillButtonDown;
    }


    public void LoadSheet(Sheet s)
    {
        ClearSheet();
        sheet = s;
        foreach(Skill sk in sheet.skills)
        {
            if (sk is Attack) attackButton.LoadSkill(sk);
            else if (sk is Defend) defendButton.LoadSkill(sk);
            else if (sk is Rest) restButton.LoadSkill(sk);
            if (sk.hidden) continue; //attack, defend, and rest are hidden
            Control skillButton = skill_button.Instantiate<Control>();
            SkillButton button = skillButton.GetNode<SkillButton>("Skill Button");
            grid.AddChild(skillButton);
    
            button.LoadSkill(sk);

            if (sheet.statBlock.CurrEnergy.Value < sk.cost)
            {
                button.Disabled = true;
            } 
            else button.SkillButtonDown += OnSkillButtonDown;
        }
    }

    void OnSkillButtonDown(SkillButton button)
    {
        EmitSignal(SignalName.SkillSelected, button.skill);
    }


    public void ClearSheet()
    {
        foreach(Node child in grid.GetChildren())
        {
            SkillButton sb = child.GetNodeOrNull<SkillButton>("Skill Button");
            if (IsInstanceValid(sb)) child.QueueFree();
        }
    }

    
}
