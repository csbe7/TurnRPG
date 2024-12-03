using Godot;
using System;

[GlobalClass]
public partial class Skill : Node
{
    public enum SkillType
    {
        attack,
        heal,
        defense,
        buff,
        debuff,
    }

    public enum Targeting
    {
        self,
        ally,
        enemy,
        any,
    }

    public enum TargetState
    {
        alive,
        dead,
        any,
    }
    
    [ExportCategory("Info")]
    [Export] public string name;
    [Export(PropertyHint.MultilineText)] public string description;
    [Export] public bool hidden = false;

    [ExportCategory("Use")]
    [Export] public float cost;
    [Export] public Targeting playerTargeting;
    [Export] public Targeting aiTargeting;
    [Export] public TargetState targetState;
    [Export] public bool canTargetSelf = true;
    //[Export] public Godot.Collections.Array<StatusEffect> effects;

    [ExportCategory("VFX")]
    [Export] SkillEffect userEffect;
    [Export] SkillEffect targetEffect;



    public CharacterIcon user;
    public CharacterIcon target;

    public void SetTarget(CharacterIcon t)
    {
        target = t;
    }

    public virtual void UseSkill()
    {
        GD.Print(user.sheet.name + " USED " + name + " ON " + target.sheet.name);
        user.sheet.ChangeEnergy(-cost);
        if (IsInstanceValid(userEffect)) user.PlayEffect(userEffect);
        if (IsInstanceValid(targetEffect)) target.PlayEffect(targetEffect);
    }

    public virtual string GetDescrption()
    {
        return description;
    }

}
