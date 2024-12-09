using Godot;
using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

public partial class AI : Node
{
    static CombatManager cm;

    //public static Godot.Collections.Array<CharacterIcon> cm.player_party = new Godot.Collections.Array<CharacterIcon>();
    //public static Godot.Collections.Array<CharacterIcon> cm.ai_party = new Godot.Collections.Array<CharacterIcon>();

    static RandomNumberGenerator rng = new RandomNumberGenerator();

    public override void _Ready()
    {
        cm = GetTree().Root.GetNode<CombatManager>("CombatManager");
    }


    public static async void Turn()
    {
        await Task.Delay(1000);
        
        do{
            int charI = rng.RandiRange(0, cm.ai_party.Count-1);
            cm.selectedCharacter = cm.ai_party[charI];
            //rng.Randomize();
        }while(cm.selectedCharacter.sheet.statBlock.Dead || cm.selectedCharacter.Spent);
        
        cm.selectedCharacter.Select();
        await Task.Delay(500);
        
        do{
            int skillI = rng.RandiRange(0, cm.selectedCharacter.sheet.skills.Count-1);
            cm.selectedSkill = cm.selectedCharacter.sheet.skills[skillI];
            //rng.Randomize();
        }while(cm.selectedSkill.cost > cm.selectedCharacter.sheet.statBlock.CurrEnergy.ModValue);
        

        CharacterIcon target = null;
        if (cm.selectedSkill.aiTargeting == Skill.Targeting.self)
        {
            target = cm.selectedCharacter;
        }
        else if (cm.selectedSkill.aiTargeting == Skill.Targeting.enemy)
        {
            do{
                int targetI = rng.RandiRange(0, cm.player_party.Count-1);
                target = cm.player_party[targetI];
                //rng.Randomize();
            }while((target.sheet.statBlock.Dead && cm.selectedSkill.targetState == Skill.TargetState.alive) 
            || (!target.sheet.statBlock.Dead && cm.selectedSkill.targetState == Skill.TargetState.dead));
        }
        if (cm.selectedSkill.aiTargeting == Skill.Targeting.ally)
        {
            do{
                int targetI = rng.RandiRange(0, cm.ai_party.Count-1);
                target = cm.player_party[targetI];
                //rng.Randomize();
            }while((target.sheet.statBlock.Dead && cm.selectedSkill.targetState == Skill.TargetState.alive) 
            || (!target.sheet.statBlock.Dead && cm.selectedSkill.targetState == Skill.TargetState.dead) 
            || (target == cm.selectedCharacter && !cm.selectedSkill.canTargetSelf));
        }
        if (cm.selectedSkill.aiTargeting == Skill.Targeting.any)
        {
            do{
                int targetI = rng.RandiRange(0, cm.ai_party.Count-1 + cm.player_party.Count-1);

                if (targetI > cm.ai_party.Count-1) target = cm.player_party[targetI - cm.ai_party.Count-1];
                else target = cm.ai_party[targetI];
                //rng.Randomize();
            }while((target.sheet.statBlock.Dead && cm.selectedSkill.targetState == Skill.TargetState.alive) 
            || (!target.sheet.statBlock.Dead && cm.selectedSkill.targetState == Skill.TargetState.dead) 
            || (target == cm.selectedCharacter && !cm.selectedSkill.canTargetSelf));
        }

        cm.selectedCharacter.Spent = true;
        
        cm.battleInterface.aiSkillDisplay.Show();
        cm.battleInterface.aiSkillDisplay.SetText(cm.selectedSkill.name);
        cm.battleInterface.aiSkillDisplay.FadeIn();
        await Task.Delay(300);
        cm.selectedSkill.SetTarget(target);
        cm.selectedSkill.UseSkill();
        await Task.Delay(200);
        cm.battleInterface.aiSkillDisplay.FadeOut();

        

        cm.SetTurn(CombatManager.Turn.player_turn);

    }
}
