using Godot;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

public partial class BattleInterface : Control
{
    CombatManager cm;
    public GridContainer partyGrid, enemyGrid;
    public ActionDrawer actionDrawer;
    public SelectedSkillDisplay selectedSkillDisplay;
    public AiSkillDisplay aiSkillDisplay;
    public Label roundCounter;
    public Control battleEndScreen;

    [Export] PackedScene characterIcon;
    [Export] PackedScene turnDisplay;
    [Export] Texture2D blueTurnContainer, redTurnContainer;
    TurnOrderDisplay turnOrderDisplay;

    [Signal] public delegate void PCSelectedEventHandler(CharacterIcon pc);
    [Signal] public delegate void TargetSelectedEventHandler(CharacterIcon target);

    CharacterIcon selectedChar, selectedTarget;
    Skill selectedSkill;

    public partial class ActionLine : RefCounted{
        public CharacterIcon from;
        public CharacterIcon to;
        public Color color;
        public float progression;   

        public ActionLine(CharacterIcon from, CharacterIcon to, Color color)
        {
            this.from = from;
            this.to = to;
            this.color = color;
            progression = 0;
        } 
    }
    Godot.Collections.Array<ActionLine> lines = new Godot.Collections.Array<ActionLine>();

    public enum ActionStage{
        character_selection,
        skill_selection,
        target_selection,
        waiting,
    }

    ActionStage currStage;

    public async override void _Ready()
    {
        cm = GetTree().Root.GetNode<CombatManager>("CombatManager");

        partyGrid = GetNode<GridContainer>("%Party Grid");
        enemyGrid = GetNode<GridContainer>("%Enemy Grid");
        
    
        actionDrawer = GetNode<ActionDrawer>("%Action Drawer");
        actionDrawer.SkillSelected += OnSkillSelected;
        actionDrawer.Hide();

        selectedSkillDisplay = GetNode<SelectedSkillDisplay>("%Selected Skill Display");
        selectedSkillDisplay.Hide();
        aiSkillDisplay = GetNode<AiSkillDisplay>("%AI Skill Display");
        aiSkillDisplay.Hide();

        battleEndScreen = GetNode<Control>("Battle End Screen");
        battleEndScreen.Hide();

        PCSelected += OnCharacterSelected;
        TargetSelected += OnTargetSelected;

        foreach (Node child in partyGrid.GetChildren() + enemyGrid.GetChildren()) child.QueueFree();

        SetStageWaiting();

        /*foreach (Sheet s in Game.state.current_party) 
        {
            AddCharacterIcon(s, false);
            AddCharacterIcon(s, false);   
        }*/

        //DEBUG LOAD ENEMIES
        /*var dir = DirAccess.Open("res://Sheets/Enemies");
        if (dir == null) 
        {
            GD.Print("dir not found");
            return;
        }
        dir.ListDirBegin();
        string filename = dir.GetNext();
        while (filename != "")
        {
            if (dir.CurrentIsDir()) continue;
            Sheet s = (Sheet)ResourceLoader.Load("res://Sheets/Enemies/" + filename);
            AddCharacterIcon(s, true);
            AddCharacterIcon(s, true);
            AddCharacterIcon(s, true);
            AddCharacterIcon(s, true);
            filename = dir.GetNext();
        }

        currStage = ActionStage.character_selection;
        
        cm.LoadBattle(this);*/
        await Task.Delay(10);
    }

    void SetStageCharacterSelection()
    {
        currStage = ActionStage.character_selection;
        actionDrawer.Hide();
        selectedSkillDisplay.Hide();
        DisableOccupied();
        //GD.Print(currStage);
        //SetSelectable(Skill.TargetState.alive, Skill.Targeting.ally);
    }

    void SetStageSkillSelection()
    {
        currStage = ActionStage.skill_selection;
        actionDrawer.Show();
        selectedSkillDisplay.Hide();
        //GD.Print(currStage);
    }
    
    void SetStageTargetSelection()
    {
        currStage = ActionStage.target_selection;
        actionDrawer.Hide();
        selectedSkillDisplay.Show();
        //GD.Print(currStage);
    }

    async void SetStageWaiting()
    {
        currStage = ActionStage.waiting;
        actionDrawer.Hide();
        selectedSkillDisplay.Hide();
        //GD.Print(currStage);
        await ToSignal(cm, CombatManager.SignalName.PlayerTurnStarted);
        SetStageCharacterSelection();
    }

    public override void _PhysicsProcess(double delta)
    {
        PositionUI();
        //QueueRedraw();
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


    //DRAW ACTION LINES
    public void AddActionLine(CharacterIcon from, CharacterIcon to, Color color)
    {
        lines.Add(new ActionLine(from, to, color));
        QueueRedraw();
    }
    public override void _Draw()
    {
        foreach (ActionLine line in lines)
        {
            if (!line.from.isEnemy)
            {
                DrawLine(line.from.topFromAttachment.GlobalPosition, line.to.bottomToAttachment.GlobalPosition, line.color, 5, true);
            }
            else
            {
                DrawLine(line.from.bottomFromAttachment.GlobalPosition, line.to.topToAttachment.GlobalPosition, line.color, 5, true);
            }
        }
    }


    public CharacterIcon AddCharacterIcon(Sheet s, bool isEnemy)
    {
        CharacterIcon ci = characterIcon.Instantiate<CharacterIcon>();
        ci.sheet = s;
        ci.isEnemy = isEnemy;
        if (isEnemy)
        {
            enemyGrid.AddChild(ci);
            ci.Selected += (c) => {
                if (currStage == ActionStage.target_selection) EmitSignal(SignalName.TargetSelected, c);
            };
        } 
        else
        {
            partyGrid.AddChild(ci);
            ci.Selected += (c) => {
                if (selectedChar != c)
                {
                    if (currStage == ActionStage.character_selection || currStage == ActionStage.skill_selection) EmitSignal(SignalName.PCSelected, c);
                    else if (currStage == ActionStage.target_selection) EmitSignal(SignalName.TargetSelected, c);
                }
            };
        } 
        return ci;
    }

    public void OnCharacterSelected(CharacterIcon ci)
    {
        if (IsInstanceValid(selectedChar)) selectedChar.Deselect();
        selectedChar = ci;
        ci.Select();
        actionDrawer.LoadSheet(ci.sheet);
        SetStageSkillSelection();
    }

    public void OnCharacterDeselected(CharacterIcon ci)
    {
        selectedChar = null;
        SetStageCharacterSelection();
    }

    public void OnSkillSelected(Skill s)
    {
        selectedSkill = s;
        selectedSkillDisplay.SetText(s.name);
        if (s.playerTargeting == Skill.Targeting.self)
        {
            cm.UseSkill(selectedChar, selectedChar, selectedSkill);
            SetStageCharacterSelection();
            selectedChar.Deselect();
            selectedChar = null;
            return;
        }
        SetSelectable(s);
        SetStageTargetSelection();
    }

    public void OnTargetSelected(CharacterIcon ci)
    {
        selectedTarget = ci;
        cm.UseSkill(selectedChar, selectedTarget, selectedSkill);
        selectedChar.Deselect();
        selectedChar = null;
        SetStageWaiting();
    }


    public void SetSelectable(Skill s)
    {
        Skill.Targeting targeting;
        if (s.user.isEnemy) targeting = s.aiTargeting;
        else targeting = s.playerTargeting;

        foreach (Node child in partyGrid.GetChildren() + enemyGrid.GetChildren())
        {
            if (child is CharacterIcon icon)
            {
                if (targeting == Skill.Targeting.any)
                {
                    if (s.targetState == Skill.TargetState.any) icon.select.Disabled = false;
                    else if (s.targetState == Skill.TargetState.alive && !icon.sheet.statBlock.Dead) icon.select.Disabled = false;
                    else if (s.targetState == Skill.TargetState.dead && icon.sheet.statBlock.Dead) icon.select.Disabled = false;
                    else icon.select.Disabled = true;
                }
                else if (targeting == Skill.Targeting.enemy && s.user.isEnemy != icon.isEnemy)
                {
                    if (s.targetState == Skill.TargetState.any) icon.select.Disabled = false;
                    else if (s.targetState == Skill.TargetState.alive && !icon.sheet.statBlock.Dead) icon.select.Disabled = false;
                    else if (s.targetState == Skill.TargetState.dead && icon.sheet.statBlock.Dead) icon.select.Disabled = false;
                    else icon.select.Disabled = true;
                }
                else if (targeting == Skill.Targeting.ally && s.user.isEnemy == icon.isEnemy)
                {
                    if (s.targetState == Skill.TargetState.any) icon.select.Disabled = false;
                    else if (s.targetState == Skill.TargetState.alive && !icon.sheet.statBlock.Dead) icon.select.Disabled = false;
                    else if (s.targetState == Skill.TargetState.dead && icon.sheet.statBlock.Dead) icon.select.Disabled = false;
                    else icon.select.Disabled = true;
                }
                else icon.select.Disabled = true;                
            }
        }
    }

    public void SetSelectable(Skill.TargetState targetState, Skill.Targeting targeting)
    {
        foreach(Node child in partyGrid.GetChildren() + enemyGrid.GetChildren())
        {
            if (child is CharacterIcon icon)
            {
                if ((!icon.isEnemy && (targeting == Skill.Targeting.ally || targeting == Skill.Targeting.any)) 
                ||   (icon.isEnemy && (targeting == Skill.Targeting.enemy || targeting == Skill.Targeting.any)))
                {
                    if ((icon.sheet.statBlock.Dead == (targetState == Skill.TargetState.dead)) || targetState == Skill.TargetState.any)
                    {
                        icon.select.Disabled = false;
                    }
                    else icon.select.Disabled = true;
                }
                else icon.select.Disabled = true;
            }
        }
    }

    public void DisableOccupied()
    {
        foreach (Node child in partyGrid.GetChildren() + enemyGrid.GetChildren())
        {
            if (child is CharacterIcon icon)
            {
                if (cm.player_party.ContainsKey(icon) && !icon.select.Disabled) icon.select.Disabled = (cm.player_party[icon] > 0);
                else if (cm.enemy_party.ContainsKey(icon)&& !icon.select.Disabled) icon.select.Disabled = (cm.enemy_party[icon] > 0);
            }
        }
    }

}
