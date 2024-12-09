using Godot;
using System;

[GlobalClass]
public partial class AttackInfo : Resource
{
    public Sheet attacker;
    
    [Export] public float minDamage;
    [Export] public float maxDamage;
    

    public enum DamageType{
        physical,
        magical,
    }
    [Export] public DamageType damageType;

    

    public AttackInfo(float dmg, DamageType dt)
    {
        maxDamage = dmg;
        damageType = dt;
    }

    public AttackInfo()
    {
        maxDamage = 0f;
    }
}

