using Godot;
using System;

public partial class Heal : Skill
{
    [Export] float healAmount;
    public override void UseSkill()
    {
        base.UseSkill();
        target.sheet.ChangeHealth(healAmount);
    }
}
