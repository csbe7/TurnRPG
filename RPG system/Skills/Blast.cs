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
    }
}
