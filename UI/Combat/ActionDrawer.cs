using Godot;
using System;

public partial class ActionDrawer : Control
{
    public Sheet sheet;
    [Export] PackedScene skill_button;
    public Skill attackSkill;
    public Skill defendSkill;

    GridContainer grid;

    public Button attackButton, defendButton, fleeButton;

    [Signal] public delegate void SkillSelectedEventHandler(Skill skill);

    public override void _Ready()
    {
        grid = GetNode<GridContainer>("%Skill Grid");

        attackButton = GetNode<Button>("%Attack Button");
        defendButton = GetNode<Button>("%Defend Button");
        fleeButton = GetNode<Button>("%Flee Button");

        attackButton.ButtonDown += OnAttactButtonDown;
        defendButton.ButtonDown += OnDefendButtonDown;
    }


    public void LoadSheet(Sheet s)
    {
        sheet = s;
        foreach(Skill sk in sheet.skills)
        {
            if (sk is Attack) attackSkill = sk;
            else if (sk is Defend) defendSkill = sk;
            if (sk.hidden) continue;
            SkillButton button = skill_button.Instantiate<SkillButton>();
            grid.AddChild(button);
            Skill skill = sk;
            button.LoadSkill(skill);

            if (sheet.statBlock.CurrEnergy.Value < skill.cost)
            {
                button.button.Disabled = true;
            } 
            else button.SkillButtonDown += OnSkillButtonDown;
        }
    }

    void OnSkillButtonDown(SkillButton button)
    {
        EmitSignal(SignalName.SkillSelected, button.skill);
    }

    void OnAttactButtonDown()
    {
        EmitSignal(SignalName.SkillSelected, attackSkill);
    }

    void OnDefendButtonDown()
    {
        EmitSignal(SignalName.SkillSelected, defendSkill);
    }


    public void ClearSheet()
    {
        foreach(Node child in grid.GetChildren())
        {
            if (child is SkillButton sb)
            {
                if (IsInstanceValid(sb)) sb.QueueFree();
            }
        }
    }

    
}
