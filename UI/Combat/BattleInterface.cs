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

    [Export] PackedScene turnDisplay;
    [Export] Texture2D blueTurnContainer, redTurnContainer;
    TurnOrderDisplay turnOrderDisplay;

    public override void _Ready()
    {
        cm = GetTree().Root.GetNode<CombatManager>("CombatManager");
        partyGrid = GetNode<GridContainer>("%Party Grid");
        enemyGrid = GetNode<GridContainer>("%Enemy Grid");
        roundCounter = GetNode<Label>("Round Counter");
        turnOrderDisplay = GetNode<TurnOrderDisplay>("Turn Order Display");
        

        actionDrawer = GetNode<ActionDrawer>("%Action Drawer");
        actionDrawer.Hide();

        selectedSkillDisplay = GetNode<SelectedSkillDisplay>("%Selected Skill Display");
        selectedSkillDisplay.Hide();
        aiSkillDisplay = GetNode<AiSkillDisplay>("%AI Skill Display");
        aiSkillDisplay.Hide();

        cm.TurnEnded += OnTurnEnded;

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


    public void LoadTurnOrder(Godot.Collections.Array<int> turnOrder, bool red)
    {
        foreach(var child in turnOrderDisplay.grid.GetChildren())
        {
            child.QueueFree();
        }

        foreach(int t in turnOrder)
        {
            Control td = turnDisplay.Instantiate<Control>();
            Sprite2D s = td.GetNode<Sprite2D>("Sprite2D");
            if (red) s.Texture = redTurnContainer;
            else s.Texture = blueTurnContainer;

            Label l = td.GetNode<Label>("%Turns");
            l.Text = t.ToString(); 
            l = td.GetNode<Label>("%Turns Left");
            l.Text = t.ToString();
            l.Hide();
            red = !red;
            turnOrderDisplay.grid.AddChild(td);
        }
    }

    Label turnsLeftLabel;
    void OnTurnEnded()
    {
        if (IsInstanceValid(turnsLeftLabel)) turnsLeftLabel.Hide();
        turnOrderDisplay.PositionDisplay(cm.turnIndex);
        Control td = (Control)turnOrderDisplay.grid.GetChild(cm.turnIndex);
        turnsLeftLabel = td.GetNode<Label>("%Turns Left");
        turnsLeftLabel.Text = cm.turnsLeft.ToString();
        turnsLeftLabel.Show();
    }

}
