using Godot;
using System;

public partial class CombatManager : Node
{
    Godot.Collections.Array<int> turnOrder = new Godot.Collections.Array<int>();
    RandomNumberGenerator rng = new RandomNumberGenerator();

    public enum Turn
    {
        player_turn,
        ai_turn,
    }
    public Turn currTurn;

    public enum TurnStage
    {
        character_selection,
        action_selection,
        target_selection,
    }
    public TurnStage turnStage;

    [Signal] public delegate void RoundEndedEventHandler();
    [Signal] public delegate void PlayerTurnStartedEventHandler();
    [Signal] public delegate void AITurnStartedEventHandler();
    [Signal] public delegate void TurnEndedEventHandler();
    
    public Godot.Collections.Array<CharacterIcon> player_party = new Godot.Collections.Array<CharacterIcon>();
    public Godot.Collections.Array<CharacterIcon> ai_party = new Godot.Collections.Array<CharacterIcon>();

    public BattleInterface battleInterface;
    
    public CharacterIcon selectedCharacter;
    public Skill selectedSkill;

    int roundCounter;

    public void BattleStart()
    {
        rng.Randomize();

        battleInterface = GetNode<BattleInterface>("../Battle Interface");
        int i = 0;
        foreach(Node child in battleInterface.partyGrid.GetChildren())
        {
            if (child is CharacterIcon icon)
            {
                player_party.Add(icon);
                icon.Selected += PlayerCharacterSelect;
                icon.Selected += PlayerTargetSelection;
                i++;
            }
        }

        foreach(Node child in battleInterface.enemyGrid.GetChildren())
        {
            if (child is CharacterIcon icon)
            {
                ai_party.Add(icon);
                icon.Selected += PlayerTargetSelection;
                icon.ai = true;
            }
        }

        battleInterface.actionDrawer.SkillSelected += PlayerSkillSelect;
        battleInterface.selectedSkillDisplay.button.ButtonDown += PlayerCancelSkill;

        roundCounter = player_party.Count + ai_party.Count;

        if (ai_party.Count >= player_party.Count)
        {
            MakeTurnOrder();
            turnIndex = 0;
            turnsLeft = turnOrder[turnIndex];
            SetTurn(Turn.player_turn);
            SetTurnStage(TurnStage.character_selection);
            battleInterface.LoadTurnOrder(turnOrder, false);
        }
        else if (ai_party.Count < player_party.Count)
        {
            MakeTurnOrder(true);
            turnIndex = 0;
            turnsLeft = turnOrder[turnIndex];
            SetTurnStage(TurnStage.character_selection);
            SetTurn(Turn.ai_turn);
            battleInterface.LoadTurnOrder(turnOrder, true);
        }

        EmitSignal(SignalName.RoundEnded);

    }


    public int turnsLeft;
    public int turnIndex;
    public void CountDownTurn()
    {
        GD.Print("Count");
        if (PlayerDefeated() || AIDefeated())
        {
            Control endScren = battleInterface.GetNode<Control>("Battle End Screen");
            Label l = battleInterface.GetNode<Label>("Battle End Screen/Label");
            if (PlayerDefeated()) l.Text = "DEFEAT";
            else l.Text = "VICTORY";
            endScren.Show();
            battleInterface.selectedSkillDisplay.Hide();
            battleInterface.aiSkillDisplay.Hide();
            return;
        }

        turnsLeft--;

        if (IsInstanceValid(selectedCharacter))
        {
            selectedCharacter.Spent = true;
            selectedCharacter.sheet.ChangeEnergy(selectedSkill.restoreEnegry);
            selectedCharacter.sheet.EmitSignal(Sheet.SignalName.TurnEnded);

        } 

        if (turnsLeft <= 0 || (currTurn == Turn.ai_turn && AISpent()) || (currTurn == Turn.player_turn && PlayerSpent()))
        {
            turnIndex++;
            if (turnIndex >= turnOrder.Count)
            {
                turnIndex = 0;
                PlayerSpent(true);
                AISpent(true);
                GD.Print("Round Ended");
                EmitSignal(SignalName.RoundEnded);
            }
            turnsLeft = turnOrder[turnIndex];
            if (currTurn == Turn.ai_turn) SetTurn(Turn.player_turn);
            else SetTurn(Turn.ai_turn);
        }

        EmitSignal(SignalName.TurnEnded); 
        SetTurnStage(TurnStage.character_selection);


        //battleInterface.roundCounter.Text = turnsLeft.ToString();
    }

    public void SetTurn(Turn t)
    {
        currTurn = t;

        switch(t)
        {
            case Turn.ai_turn:
            //SetTurnStage(TurnStage.character_selection);
            EmitSignal(SignalName.AITurnStarted);
            AI.Turn();
            break;

            case Turn.player_turn:
            //SetTurnStage(TurnStage.character_selection);
            EmitSignal(SignalName.PlayerTurnStarted);
            break;
        }
    }
    
    public void SetTurnStage(TurnStage t)
    {
        turnStage = t;
        switch(t)
        {
            case TurnStage.character_selection:
            CharacterSelection();
            break;

            case TurnStage.action_selection:
            ActionSelection();
            break;

            case TurnStage.target_selection:
            TargetSelection();
            break;
        }
    }

    public void CharacterSelection()
    {
        battleInterface.selectedSkillDisplay.Hide();
        battleInterface.actionDrawer.Hide();

        if (IsInstanceValid(selectedCharacter))
        {
            selectedCharacter.Deselect();
            selectedCharacter = null;
        } 

    
        if (currTurn != Turn.player_turn)
        {
            SetSelection(1, false);
            SetSelection(2, false);
        }
        else
        {
            SetSelection(2, true);
            SetSelection(1, false);
            SetSelection(3, false);
            SetSelection(4, false);
        }
    }

    public void ActionSelection()
    {
        if (currTurn == Turn.player_turn)
        {
            SetSelection(1, true);
            SetSelection(1, false);
            SetSelection(4, false);
            battleInterface.selectedSkillDisplay.Hide();
            battleInterface.actionDrawer.Show();
            battleInterface.actionDrawer.ClearSheet();
            battleInterface.actionDrawer.LoadSheet(selectedCharacter.sheet);
        }
    }

    public void TargetSelection()
    {
        if (currTurn == Turn.player_turn)
        {
            SetSelection(4, true);
            if (selectedSkill.playerTargeting == Skill.Targeting.self)
            {
                selectedSkill.SetTarget(selectedCharacter);
                selectedSkill.UseSkill();
                CountDownTurn();
            }
            else if (selectedSkill.playerTargeting == Skill.Targeting.any)
            {
                if (selectedSkill.targetState == Skill.TargetState.any)
                {
                    SetSelection(0, true);
                }
                else if (selectedSkill.targetState == Skill.TargetState.alive)
                {
                    SetSelection(0, true);
                    SetSelection(3, false);
                }
                else if (selectedSkill.targetState == Skill.TargetState.dead)
                {
                    SetSelection(0, false);
                    SetSelection(3, true);
                }
                ProgressTargetSelection();
            }
            else if (selectedSkill.playerTargeting == Skill.Targeting.ally)
            {
                if (selectedSkill.targetState == Skill.TargetState.any)
                {
                    SetSelection(1, false);
                    SetSelection(2, true);
                }
                else if (selectedSkill.targetState == Skill.TargetState.alive)
                {
                    SetSelection(2, true);
                    SetSelection(3, false);
                }
                else if (selectedSkill.targetState == Skill.TargetState.dead)
                {
                    SetSelection(2, true);
                    SetSelection(3, true);
                    SetSelection(1, false);
                }
                ProgressTargetSelection();
            }
            else if (selectedSkill.playerTargeting == Skill.Targeting.enemy)
            {
                if (selectedSkill.targetState == Skill.TargetState.any)
                {
                    SetSelection(2, false);
                    SetSelection(1, true);
                }
                else if (selectedSkill.targetState == Skill.TargetState.alive)
                {
                    SetSelection(1, true);
                    SetSelection(3, false);
                }
                else if (selectedSkill.targetState == Skill.TargetState.dead)
                {
                    SetSelection(1, true);
                    SetSelection(3, true);
                    SetSelection(2, false);
                }
                ProgressTargetSelection();
            }
        }
    }

    void ProgressTargetSelection()
    {
        battleInterface.actionDrawer.Hide();

        battleInterface.selectedSkillDisplay.Show();
        battleInterface.selectedSkillDisplay.SetText(selectedSkill.name);
    }

    private void PlayerCharacterSelect(CharacterIcon character)
    {
        if (turnStage == TurnStage.target_selection || currTurn != Turn.player_turn) return;

        if (IsInstanceValid(selectedCharacter) && selectedCharacter != character) selectedCharacter.Deselect();
        if (selectedCharacter != character) character.Select();
        selectedCharacter = character;
        SetTurnStage(TurnStage.action_selection);
    }

    private void PlayerSkillSelect(Skill skill)
    {
        if (turnStage != TurnStage.action_selection || currTurn != Turn.player_turn) return;

        selectedSkill = skill;
        SetTurnStage(TurnStage.target_selection);
    }

    private void PlayerTargetSelection(CharacterIcon target)
    {
        if (turnStage != TurnStage.target_selection || currTurn != Turn.player_turn) return;
        
        
        selectedSkill.SetTarget(target);
        selectedSkill.UseSkill();

        selectedCharacter.Spent = true;
        selectedCharacter.sheet.EmitSignal(Sheet.SignalName.TurnEnded);
        CountDownTurn();
    }

    void PlayerCancelSkill()
    {
        if (turnStage != TurnStage.target_selection || currTurn != Turn.player_turn) return;
        SetTurnStage(TurnStage.action_selection);
    }


    void SetSelection(int who, bool mode) //0 - everyone    1 - enemy   2 - party   3 - dead  4 - spent
    {
        if (who == 0 || who == 1)
        {
            
            foreach(CharacterIcon enemy in ai_party)
            {
                enemy.select.Disabled = !mode;
            }
        }

        if (who == 0 || who == 2)
        {
            
            foreach(CharacterIcon pchar in player_party)
            {
                pchar.select.Disabled = !mode;
            }
        }

        if (who == 3)
        {
            foreach(CharacterIcon pchar in player_party)
            {
                if (pchar.sheet.statBlock.Dead) pchar.select.Disabled = !mode;
            }

            foreach(CharacterIcon enemy in ai_party)
            {
                if (enemy.sheet.statBlock.Dead) enemy.select.Disabled = !mode;
            }
        }

        if (who == 4)
        {
            foreach(CharacterIcon pchar in player_party)
            {
                if (pchar.Spent) pchar.select.Disabled = !mode;
            }

            foreach(CharacterIcon enemy in ai_party)
            {
                if (enemy.Spent) enemy.select.Disabled = !mode;
            }
        }
    }


    bool PlayerSpent(bool reset = false)
    {
        bool status = true;
        foreach(CharacterIcon icon in player_party)
        {
            if (icon.sheet.statBlock.Dead) continue;
            if (!icon.Spent) 
            {
                status = false;
                break;
            }
        }
        if (reset)
        {
            foreach(CharacterIcon icon in player_party)
            {
                if (icon.sheet.statBlock.Dead) continue;
                icon.Spent = false;
            }
        }
        return status;
    }

    bool AISpent(bool reset = false)
    {
        bool status = true;
        foreach(CharacterIcon icon in ai_party)
        {
            if (icon.sheet.statBlock.Dead) continue;
            if (!icon.Spent) 
            {
                status = false;
                break;
            }
        }
        if (reset)
        {
            foreach(CharacterIcon icon in ai_party)
            {
                if (icon.sheet.statBlock.Dead) continue;
                icon.Spent = false;
            }
        }
        return status;
    }

    bool PlayerDefeated()
    {
        foreach(CharacterIcon icon in player_party)
        {
            if (!icon.sheet.statBlock.Dead) return false;
        }
        return true;
    }
    bool AIDefeated()
    {
        foreach(CharacterIcon icon in ai_party)
        {
            if (!icon.sheet.statBlock.Dead) return false;
        }
        return true;
    }

    void MakeTurnOrder(bool aifirst = false)
    {
        turnOrder.Clear();
        int aileft = ai_party.Count;
        int pleft = player_party.Count;
        while(!aifirst)
        {
            int pn;
            if (aileft == 0 || pleft == 0) pn = pleft;
            else pn = rng.RandiRange(1, pleft);
            
            pleft -= pn;
            
            int ain;
            if (pleft == 0 || aileft == 0) ain = aileft;
            else ain = rng.RandiRange(1, aileft);

            aileft -= ain;

            if (pn != 0) turnOrder.Add(pn);
            if (ain != 0) turnOrder.Add(ain);

            if (aileft == 0 && pleft == 0) break;
        }

        while(aifirst)
        {
            
            int ain;
            if (pleft == 0 || aileft == 0) ain = aileft;
            else ain = rng.RandiRange(1, aileft);

            aileft -= ain;

            int pn;
            if (aileft == 0 || pleft == 0) pn = pleft;
            else pn = rng.RandiRange(1, pleft);
            
            pleft -= pn;

            if (ain != 0) turnOrder.Add(ain);
            if (pn != 0) turnOrder.Add(pn);

            if (aileft == 0 && pleft == 0)
            {

                break;
            }
        }

    }
}
