using Godot;
using System;
using System.Linq;

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
    [Export] public int cooldown;
    [Export] public Targeting playerTargeting;
    [Export] public Targeting aiTargeting;
    [Export] public TargetState targetState;
    [Export] public bool canTargetSelf = true;
    [Export] public float restoreEnegry = 0;
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

    public virtual string GetDescription()
    {
        if (cost != 0) return DescAddCost(new string(description));
        return description;
    }

    public virtual string DescFillAttack(string desc, AttackInfo a)
    {
        //string desc = new string(description);
        a = user.sheet.Attack(a, null, true);
        if (a.minDamage != a.maxDamage) desc = desc.Replace("%DamageRange", a.minDamage.ToString() + "-" + a.maxDamage.ToString());
        else desc = desc.Replace("%DamageRange", a.maxDamage.ToString());
        desc = desc.Replace("%DamageType", a.damageType.ToString());
        return desc;
    }

    public virtual string DescAddCost(string desc)
    {
        return desc + "\nCost: " + cost;
    }

}
