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
        cm.AITurnStarted += AITurn;
    }

    public async static void AITurn()
    {
        await Task.Delay(1000);
        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Randomize();
        cm.bi.SetSelectable(Skill.TargetState.alive, Skill.Targeting.enemy);
        cm.bi.DisableOccupied();
        
        Godot.Collections.Array<CharacterIcon> keys = new Godot.Collections.Array<CharacterIcon>(cm.enemy_party.Keys);

       
        //SELECT CHARACTER
        CharacterIcon character;
        do{
            int rand = rng.RandiRange(0, keys.Count-1);
            character = keys[rand];
        }while (character.select.Disabled);

        Skill selectedSkill;
        do{
            int rand = rng.RandiRange(0, character.sheet.skillParent.GetChildCount()-1);
            selectedSkill = (Skill)character.sheet.skillParent.GetChild(rand);
        }while(selectedSkill == null);

        if (selectedSkill.aiTargeting == Skill.Targeting.self) 
        {
            character.Select();
            cm.bi.aiSkillDisplay.Show();
            cm.bi.aiSkillDisplay.SetText(selectedSkill.name);
            cm.bi.aiSkillDisplay.FadeIn();
            await Task.Delay(1000);
            cm.bi.aiSkillDisplay.FadeOut();
            character.Deselect();
            cm.UseSkill(character, character, selectedSkill);
            return;
        }

        cm.bi.SetSelectable(selectedSkill);

        CharacterIcon target;
        keys = new Godot.Collections.Array<CharacterIcon>(cm.enemy_party.Keys) + new Godot.Collections.Array<CharacterIcon>(cm.player_party.Keys);
        do{
            int rand = rng.RandiRange(0, keys.Count-1);
            target = keys[rand];
        }while(target.select.Disabled);

        character.Select();
        cm.bi.aiSkillDisplay.Show();
        cm.bi.aiSkillDisplay.SetText(selectedSkill.name);
        cm.bi.aiSkillDisplay.FadeIn();
        await Task.Delay(2000);
        cm.bi.aiSkillDisplay.FadeOut();
        character.Deselect();
        cm.UseSkill(character, target, selectedSkill);

    }

}
