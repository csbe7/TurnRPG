using Godot;
using System;

public partial class Attack : Skill
{

    public override void UseSkill()
    {
        base.UseSkill();
        AttackInfo attack = user.sheet.statBlock.baseAttack;
        user.sheet.Attack(attack, target.sheet, true);
    }
}
