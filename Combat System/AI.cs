using Godot;
using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

public partial class AI : Node
{
    static CombatManager cm;

    public override void _Ready()
    {
        cm = GetTree().Root.GetNode<CombatManager>("CombatManager");
    }

    public void AITurn()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Randomize();
        cm.bi.SetSelectable(Skill.TargetState.alive, Skill.Targeting.enemy);
        cm.bi.DisableOccupied();
        
        Godot.Collections.Array<CharacterIcon> keys = new Godot.Collections.Array<CharacterIcon>(cm.enemy_party.Keys);

        CharacterIcon character;
        do{
            int rand = rng.RandiRange(0, cm.enemy_party.Count-1);
            character = keys[rand];
        }while (character.select.Disabled);
    }
}
