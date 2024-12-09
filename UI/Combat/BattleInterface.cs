using Godot;
using System;

public partial class BattleInterface : Control
{
    CombatManager cm;
    public GridContainer partyGrid, enemyGrid;
    public ActionDrawer actionDrawer;
    public SelectedSkillDisplay selectedSkillDisplay;
    public AiSkillDisplay aiSkillDisplay;
    public Label roundCounter;

    public override void _Ready()
    {
        cm = GetTree().Root.GetNode<CombatManager>("CombatManager");
        partyGrid = GetNode<GridContainer>("%Party Grid");
        enemyGrid = GetNode<GridContainer>("%Enemy Grid");
        roundCounter = GetNode<Label>("Round Counter");
        

        actionDrawer = GetNode<ActionDrawer>("%Action Drawer");
        actionDrawer.Hide();

        selectedSkillDisplay = GetNode<SelectedSkillDisplay>("%Selected Skill Display");
        selectedSkillDisplay.Hide();
        aiSkillDisplay = GetNode<AiSkillDisplay>("%AI Skill Display");
        aiSkillDisplay.Hide();

        cm.BattleStart();

    }

    public override void _PhysicsProcess(double delta)
    {
        PositionUI();

    }

    void PositionUI()
    {
        int count = partyGrid.GetChildCount();
        Vector2 screenSize =  GetViewport().GetVisibleRect().Size;

        int space = 146 * count;

        partyGrid.Position = new Vector2(((screenSize.X - space)/2), partyGrid.Position.Y);

        count = enemyGrid.GetChildCount();

        space = 146 * count;

        enemyGrid.Position = new Vector2(((screenSize.X - space)/2), enemyGrid.Position.Y);
    }


}
