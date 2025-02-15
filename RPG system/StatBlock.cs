using Godot;
using System;

[GlobalClass]
public partial class StatBlock : Resource
{
    [Export] public Stat Health;
    public Stat CurrHealth = new Stat();
    [Export] public Stat Energy;
    public Stat CurrEnergy = new Stat(); 


    [Export] public Stat PhysicalDefence = new Stat(0, -1000, 100);
    [Export] public Stat MagicalDefence = new Stat(0, -1000, 100);
    [Export] public Stat PhysicalAttack = new Stat(0, -1000, 1000);
    [Export] public Stat MagicalAttack = new Stat(0, -1000, 1000);

    [Export] public AttackInfo baseAttack;

    bool dead;
    public bool Dead{
        get{
            return CurrHealth.Value <= 0;
        }
    }

    public StatBlock Clone()
    {
        StatBlock sb = (StatBlock)this.Duplicate();
        sb.Health = (Stat)Health.Duplicate();
        sb.CurrHealth = (Stat)CurrHealth.Duplicate();
        sb.Energy = (Stat)Energy.Duplicate();
        sb.CurrEnergy = (Stat)CurrEnergy.Duplicate();
        sb.PhysicalDefence = (Stat)PhysicalDefence.Duplicate();
        sb.MagicalDefence = (Stat)MagicalDefence.Duplicate();
        sb.PhysicalAttack = (Stat)PhysicalAttack.Duplicate();
        sb.MagicalAttack = (Stat)MagicalAttack.Duplicate();
        return sb;
    }
    
}
