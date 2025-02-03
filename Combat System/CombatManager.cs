using Godot;
using System;
using System.Threading.Tasks;

public partial class CombatManager : Node
{
    [Signal] public delegate void TurnEndedEventHandler();

    public Godot.Collections.Dictionary<CharacterIcon, int> player_party = new Godot.Collections.Dictionary<CharacterIcon, int>();
    public Godot.Collections.Dictionary<CharacterIcon, int> enemy_party = new Godot.Collections.Dictionary<CharacterIcon, int>(); 

    static int turnCountdownDelay = 1000;

    [Export] PackedScene battleInterface;
    public BattleInterface bi;

    public void StartBattle(Node location, Encounter encounter)
    {
        bi = battleInterface.Instantiate<BattleInterface>();
        location.AddChild(bi);

        foreach(Sheet s in Game.state.current_party)
        {
            CharacterIcon ci = bi.AddCharacterIcon(s, false);
            player_party.Add(ci, 0);
        }

        foreach(Sheet s in encounter.enemies)
        {
            CharacterIcon ci = bi.AddCharacterIcon(s, false);
            player_party.Add(ci, 0);
        }
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
        int occupied = 0;
        foreach(CharacterIcon key in player_party.Keys)
        {
            player_party[key] = Mathf.Max(0, player_party[key]-1);
            if (player_party[key] > 0) occupied++;
            else{
                key.select.Disabled = false;
            }
        }
        foreach(CharacterIcon key in enemy_party.Keys)
        {
            enemy_party[key] = Mathf.Max(0, enemy_party[key]-1);
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

    public void useSkill(CharacterIcon user, CharacterIcon target, Skill skill)
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
        user.select.Disabled = true;
        CountDownTurn();
        if (!user.isEnemy) AI.AITurn();
    }
}
