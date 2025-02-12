using Godot;
using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Threading.Tasks;

public partial class CombatManager : Node
{
    public const int FLEE = 0, LOSS = 1, WIN = 2;

    [Signal] public delegate void TurnEndedEventHandler();
    [Signal] public delegate void PlayerTurnEndedEventHandler();
    [Signal] public delegate void AITurnEndedEventHandler();
    [Signal] public delegate void PlayerTurnStartedEventHandler();
    [Signal] public delegate void AITurnStartedEventHandler();
    [Signal] public delegate void BattleEndedEventHandler(int state); 

    public enum Turn{
        player_turn,
        ai_turn,
    }

    public Turn currTurn;

    public Godot.Collections.Dictionary<CharacterIcon, int> player_party = new Godot.Collections.Dictionary<CharacterIcon, int>();
    public Godot.Collections.Dictionary<CharacterIcon, int> enemy_party = new Godot.Collections.Dictionary<CharacterIcon, int>(); 

    static int turnCountdownDelay = 1500;

    public BattleInterface bi;

    public Encounter encounter;

    public void StartBattle(Node location, Encounter encounter)
    {
        bi = Game.battleInterface.Instantiate<BattleInterface>();
        location.AddChild(bi);

        bi.GetNode<Button>("Win Button").ButtonDown += Win;
        bi.GetNode<Button>("Lose Button").ButtonDown += Lose;
        bi.GetNode<Button>("Flee Button").ButtonDown += () => {
            bi.battleEndScreen.Show();
            bi.battleEndScreen.GetNode<Label>("Label").Text = "FLEEING";
        
            EmitSignal(SignalName.BattleEnded, 0);
            bi.QueueFree();
        return;
        };

        this.encounter = encounter;

        foreach(Sheet s in Game.state.current_party)
        {
            CharacterIcon ci = bi.AddCharacterIcon(s, false);
            player_party.Add(ci, 0);
        }

        foreach(Sheet s in encounter.enemies)
        {
            CharacterIcon ci = bi.AddCharacterIcon(s, true);
            enemy_party.Add(ci, 0);
        }

        EmitSignal(SignalName.PlayerTurnStarted);
        currTurn = Turn.player_turn;
    }

    public void LoadBattle(BattleInterface bi)
    {
        this.bi = bi;
        
        foreach(Node child in bi.partyGrid.GetChildren())
        {
            if (child is CharacterIcon icon)
            {
                player_party.Add(icon, 0);
            }
        }
        foreach(Node child in bi.enemyGrid.GetChildren())
        {
            if (child is CharacterIcon icon)
            {
                enemy_party.Add(icon, 0);
            }
        }
    }

    async void CountDownTurn()
    {
        int dead = 0;
        foreach(CharacterIcon c in player_party.Keys)
        {
            if (!c.sheet.statBlock.Dead) break;
            else dead++;
        }
        if (dead == player_party.Keys.Count)
        {
            bi.battleEndScreen.Show();
            bi.battleEndScreen.GetNode<Label>("Label").Text = "DEFEAT";
            await Task.Delay(2000);
            EmitSignal(SignalName.BattleEnded, 1);
            bi.QueueFree();
            return;
        }

        dead = 0;
        foreach(CharacterIcon c in enemy_party.Keys)
        {
            if (!c.sheet.statBlock.Dead) break;
            else dead++;
        }
        if (dead == enemy_party.Keys.Count)
        {
            bi.battleEndScreen.Show();
            bi.battleEndScreen.GetNode<Label>("Label").Text = "Victory";
            await Task.Delay(2000);
            EmitSignal(SignalName.BattleEnded, 2);
            bi.QueueFree();
            return;
        }

        int occupied = 0;
        foreach(CharacterIcon key in player_party.Keys)
        {
            player_party[key] = Mathf.Max(0, player_party[key]-1);
            key.SetCooldown(player_party[key]);
            if (player_party[key] > 0) occupied++;
            else{
                key.select.Disabled = false;
            }
        }
        foreach(CharacterIcon key in enemy_party.Keys)
        {
            enemy_party[key] = Mathf.Max(0, enemy_party[key]-1);
            key.SetCooldown(enemy_party[key]);
            if (enemy_party[key] > 0) occupied++;
            else{
                key.select.Disabled = false;
            }
        }
        
        EmitSignal(SignalName.TurnEnded);
        if (occupied == enemy_party.Count + player_party.Count)
        {
            await Task.Delay(turnCountdownDelay);
            CountDownTurn();
        }
        bi.DisableOccupied();
    }

    int turnCounter = 0;
    public void UseSkill(CharacterIcon user, CharacterIcon target, Skill skill)
    {
        skill.SetTarget(target);
        skill.UseSkill();
        if (player_party.ContainsKey(user))
        {
            player_party[user] = skill.cooldown;
        }
        else if (enemy_party.ContainsKey(user))
        {
            enemy_party[user] = skill.cooldown;
        }
        else return;

        user.SetCooldown(skill.cooldown);
        user.select.Disabled = true;
        TurnCounterUp();
        if (!user.isEnemy) TurnSwitchAI();
        else TurnSwitchPlayer();
    }

    public void TurnCounterUp()
    {
        turnCounter++;
        if (turnCounter >= 2)
        {
            turnCounter = 0;
            CountDownTurn();
        }
    }


    public async void TurnSwitchPlayer()
    {
        if (currTurn == Turn.player_turn) return;
        //GD.Print("Switch to player started");
        currTurn = Turn.player_turn;
        int occupied = 0;
        int dead = 0;
        bi.DisableOccupied();
        foreach (CharacterIcon ci in player_party.Keys)
        {
            if (ci.select.Disabled) occupied++;
            if (ci.sheet.statBlock.Dead) dead++;
        }
        //GD.Print("Player dead: " + dead);
        if (dead == enemy_party.Keys.Count)
        {
            Lose();
            return;
        }
        //GD.Print("Player occupied: " + occupied);
        if (occupied == player_party.Keys.Count)
        {
            //GD.Print("Player Occupied");
            await Task.Delay(turnCountdownDelay);
            TurnCounterUp();
            TurnSwitchAI();
            return;
        }
        //GD.Print("Switch to Player ended");
        EmitSignal(SignalName.PlayerTurnStarted);
    }

    public async void TurnSwitchAI()
    {
        if (currTurn == Turn.ai_turn) return;
        //GD.Print("Switch to Ai started");
        currTurn = Turn.ai_turn;
        int occupied = 0;
        int dead = 0;
        bi.DisableOccupied();
        foreach (CharacterIcon ci in enemy_party.Keys)
        {
            if (ci.select.Disabled) occupied++;
            if (ci.sheet.statBlock.Dead) dead++;
        }
        //GD.Print("AI dead: " + dead);
        if (dead == enemy_party.Keys.Count)
        {
            Win();
            return;
        }
        //GD.Print("AI occupied: " + occupied);
        if (occupied == enemy_party.Keys.Count)
        {
            //GD.Print("AI Occupied");
            await Task.Delay(turnCountdownDelay);
            TurnCounterUp();
            TurnSwitchPlayer();
            return;
        }
        //GD.Print("Switch to Ai ended");
        EmitSignal(SignalName.AITurnStarted);
    }


    public async void Win()
    {
        bi.battleEndScreen.Show();
        bi.battleEndScreen.GetNode<Label>("Label").Text = "Victory";
        await Task.Delay(2000);
        EmitSignal(SignalName.BattleEnded, 2);
        bi.QueueFree();
        return;
    }

    public async void Lose()
    {
        bi.battleEndScreen.Show();
        bi.battleEndScreen.GetNode<Label>("Label").Text = "Defeat";
        await Task.Delay(2000);
        EmitSignal(SignalName.BattleEnded, 1);
        bi.QueueFree();
        return;
    }
}
