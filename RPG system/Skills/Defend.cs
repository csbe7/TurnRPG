using Godot;
using System;

public partial class Defend : Skill
{
    [Export] PackedScene defendEffect;

    public override void UseSkill()
    {
        base.UseSkill();
        StatusEffect se = defendEffect.Instantiate<StatusEffect>();
        se.giver = user.sheet;
        se.receiver = target.sheet;
        user.sheet.AddStatusEffect(se);
    }
}
