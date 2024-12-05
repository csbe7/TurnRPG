using Godot;
using System;

[GlobalClass]
public partial class Blast : Skill
{
    [Export] public AttackInfo attack;

    public override void UseSkill()
    {
        base.UseSkill();
        user.sheet.Attack(attack, target.sheet, true);
        GD.Print(GetDescription());
    }

    public override string GetDescription()
    {
        string desc = DescFillAttack(new string(description), attack);
        desc = DescAddCost(desc);
        return desc;
    }
}
