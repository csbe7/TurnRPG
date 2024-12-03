using Godot;
using System;

[GlobalClass]
public partial class Blast : Skill
{
    [Export] public AttackInfo attack;

    public override void UseSkill()
    {
        base.UseSkill();
        attack = (AttackInfo)attack.Duplicate();
        attack.attacker = user.sheet;
        target.sheet.HandleAttack(attack);
    }
}
