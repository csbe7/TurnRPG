using Godot;
using System;

public partial class Attack : Skill
{

    public override void UseSkill()
    {
        base.UseSkill();
        AttackInfo attack = user.sheet.statBlock.baseAttack;
        attack = (AttackInfo)attack.Duplicate();
        attack.attacker = user.sheet;
        target.sheet.HandleAttack(attack);
    }
}
