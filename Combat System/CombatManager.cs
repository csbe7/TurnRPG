using Godot;
using System;

public partial class CombatManager : Node
{

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

    [Signal] public delegate void RoundStartedEventHandler();
    [Signal] public delegate void PlayerTurnStartedEventHandler();
    [Signal] public delegate void AITurnStartedEventHandler();
    
    public Godot.Collections.Array<CharacterIcon> player_party = new Godot.Collections.Array<CharacterIcon>();
    public Godot.Collections.Array<CharacterIcon> ai_party = new Godot.Collections.Array<CharacterIcon>();

    public BattleInterface battleInterface;
    
    public CharacterIcon selectedCharacter;
    public Skill selectedSkill;

    public void BattleStart()
    {
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

        AI.SetParties(player_party, ai_party);

        SetTurn(Turn.player_turn);
        SetTurnStage(TurnStage.character_selection);
        EmitSignal(SignalName.RoundStarted);
    }

    int turnCounter = 0;
    public void SetTurn(Turn t)
    {
        currTurn = t;

        if (IsInstanceValid(selectedCharacter)){
            selectedCharacter.spent = true;
            selectedCharacter.sheet.ChangeEnergy(selectedSkill.restoreEnegry);
            selectedCharacter.sheet.EmitSignal(Sheet.SignalName.TurnEnded);

        } 

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

        if (turnCounter != 2) turnCounter++;
        else{
            bool pspent = PlayerSpent(true);
            bool aspent = AISpent(true);
            if (pspent && aspent)
            {
               GD.Print("Round Ended");
               EmitSignal(SignalName.RoundStarted);
            }
            turnCounter = 1;
        }



        switch(t)
        {
            case Turn.ai_turn:
            SetTurnStage(TurnStage.character_selection);
            EmitSignal(SignalName.AITurnStarted);
            AI.Turn();
            break;

            case Turn.player_turn:
            SetTurnStage(TurnStage.character_selection);
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
                SetTurn(Turn.ai_turn);
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

        selectedCharacter.spent = true;
        selectedCharacter.sheet.EmitSignal(Sheet.SignalName.TurnEnded);
        SetTurn(Turn.ai_turn);
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
                if (pchar.spent) pchar.select.Disabled = !mode;
            }

            foreach(CharacterIcon enemy in ai_party)
            {
                if (enemy.spent) enemy.select.Disabled = !mode;
            }
        }
    }


    bool PlayerSpent(bool reset = false)
    {
        foreach(CharacterIcon icon in player_party)
        {
            if (icon.sheet.statBlock.Dead) continue;
            if (!icon.spent) return false;
        }
        if (reset)
        {
            foreach(CharacterIcon icon in player_party)
            {
                if (icon.sheet.statBlock.Dead) continue;
                icon.spent = false;
            }
        }
        return true;
    }

    bool AISpent(bool reset = false)
    {
        foreach(CharacterIcon icon in ai_party)
        {
            if (icon.sheet.statBlock.Dead) continue;
            if (!icon.spent) return false;
        }
        if (reset)
        {
            foreach(CharacterIcon icon in ai_party)
            {
                if (icon.sheet.statBlock.Dead) continue;
                icon.spent = false;
            }
        }
        return true;
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
}
