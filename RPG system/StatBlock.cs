using Godot;
using System;

[GlobalClass]
public partial class StatBlock : Resource
{
    [Export] public Stat Health;
    public Stat CurrHealth = new Stat();
    [Export] public Stat Energy;
    public Stat CurrEnergy = new Stat(); 


    [Export] public Stat Defence = new Stat(0, 0, 100);

    [Export] public AttackInfo baseAttack;

    bool dead;
    public bool Dead{
        get{
            return CurrHealth.Value <= 0;
        }
    }
    
}
