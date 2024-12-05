using Godot;
using System;

public partial class Attack : Skill
{

    public override void UseSkill()
    {
        base.UseSkill();
        AttackInfo attack = user.sheet.statBlock.baseAttack;
        user.sheet.Attack(attack, target.sheet, true);
        GD.Print(GetDescription());
    }

    public override string GetDescription()
    {
        string desc = DescFillAttack(new string(description), user.sheet.statBlock.baseAttack);
        return desc;
    }
}
