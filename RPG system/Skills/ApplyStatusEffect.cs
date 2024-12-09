using Godot;
using System;

public partial class ApplyStatusEffect : Skill
{
    [Export] Godot.Collections.Array<PackedScene> targetStausEffects = new Godot.Collections.Array<PackedScene>();
    [Export] Godot.Collections.Array<PackedScene> userStausEffects = new Godot.Collections.Array<PackedScene>();

    public override void UseSkill()
    {
        foreach (PackedScene ps in targetStausEffects)
        {
            StatusEffect se = ps.Instantiate<StatusEffect>();
            se.giver = user.sheet;
            target.sheet.AddStatusEffect(se);
        }
        foreach (PackedScene ps in userStausEffects)
        {
            StatusEffect se = ps.Instantiate<StatusEffect>();
            se.giver = user.sheet;
            user.sheet.AddStatusEffect(se);
        }
    }
}
