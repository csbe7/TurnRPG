using Godot;
using System;


[GlobalClass]
public partial class Sheet : Resource
{
    [Export] public string name;
    [Export] public StatBlock statBlock;
	[Export] public Godot.Collections.Array<PackedScene> packedSkills = new Godot.Collections.Array<PackedScene>();
    
	//[Export] static public PackedScene attackSkill;

    [Signal] public delegate void TurnEndedEventHandler();

    [Signal] public delegate void AttackedEventHandler(AttackInfo info);
    [Signal] public delegate void HealthChangedEventHandler(float change);
    [Signal] public delegate void EnergyChangedEventHandler(float change);
    
	public Node skillParent;
	public Godot.Collections.Array<Skill> skills = new Godot.Collections.Array<Skill>();
	
	RandomNumberGenerator rng = new RandomNumberGenerator();

	public void Ready()
	{
		statBlock.CurrHealth = new Stat(statBlock.Health.ModValue, 0, statBlock.Health.ModValue);
		statBlock.CurrEnergy = new Stat(statBlock.Energy.ModValue, 0, statBlock.Energy.ModValue);
		
		/*PackedScene r = (PackedScene)ResourceLoader.Load("res://RPG system/Skills/attack.tscn");
		Skill rs = r.Instantiate<Skill>();
		skillParent.AddChild(rs);
		skills.Add(rs);
		rs.user = this;

		foreach(PackedScene skillScene in packedSkills)
		{
			Skill s = skillScene.Instantiate<Skill>();
			skillParent.AddChild(s);
			skills.Add(s);
			s.user = this;
		}*/
	}


    public void HandleAttack(AttackInfo attack)
    {
        EmitSignal(SignalName.Attacked, attack);

		float damage = rng.RandiRange((int)attack.minDamage, (int)attack.maxDamage);
		
		damage = (damage/100) * (100-statBlock.Defence.ModValue);
		
        statBlock.CurrHealth.Value -= damage;
		//GD.Print(this.name + " damaged by " + attack.attacker.name);

		EmitSignal(SignalName.HealthChanged, -attack.maxDamage);
    }

	public AttackInfo Attack(AttackInfo attack, Sheet target = null, bool dup = false)
	{
		if (dup) 
		{
			attack = (AttackInfo)attack.Duplicate();
			attack.attacker = this;
		}
		if (IsInstanceValid(target)) target.HandleAttack(attack);
		return attack;
	} 
    
    public void ChangeHealth(float amount)
    {
        statBlock.CurrHealth.Value += amount;
		EmitSignal(SignalName.HealthChanged, amount);
    }

    public void ChangeEnergy(float amount)
    {
        statBlock.CurrEnergy.Value += amount;
		EmitSignal(SignalName.EnergyChanged, amount);
    }




    public void AddStatModifier(StatModifier mod, string targetStat)
	{
		((Stat)statBlock.GetType().GetField(targetStat).GetValue(statBlock)).AddModifier(mod);
	}
	public void RemoveStatModifier(StatModifier mod, string targetStat)
    {
        ((Stat)statBlock.GetType().GetField(targetStat).GetValue(statBlock)).RemoveModifier(mod);
    }

    
    public Node statusEffects;
    public void AddStatusEffect(StatusEffect statusEffect)
	{
		statusEffect.receiver = this;
        statusEffects.AddChild(statusEffect);
        
		Godot.Collections.Array<StatusEffect> statusEffectList = GetStatusEffects();
		SortStatusEffects(statusEffectList, 0, statusEffectList.Count - 1);
	}


    public Godot.Collections.Array<StatusEffect> GetStatusEffects()
	{
        Godot.Collections.Array<StatusEffect> effects = new Godot.Collections.Array<StatusEffect>();

        foreach(Node child in statusEffects.GetChildren())
		{
			if (child is StatusEffect)
			{
				effects.Add((StatusEffect)child);
			}
		}

		return effects;
	}

    private void SortStatusEffects(Godot.Collections.Array<StatusEffect> array, int start, int end)
	{
		if (end <= start) return;
        
		int index, index2;
		StatusEffect temp;
		int i, j = start-1;

        for(i = start; i < end; i++)
		{
			if (array[i].priority < array[end].priority)
			{
				j++;
				temp = array[j];
                array[j] = array[i];
				array[i] = temp;
                
				index = array[i].GetIndex();
				index2 = array[j].GetIndex();
                
				statusEffects.MoveChild(array[i], index2);
				statusEffects.MoveChild(array[j], index);
			}
		}
		j++;
		temp = array[j];
		array[j] = array[end];
		array[end] = temp;
        
		index = array[i].GetIndex();
		index2 = array[j].GetIndex();

		statusEffects.MoveChild(array[i], index2);
		statusEffects.MoveChild(array[j], index);

		SortStatusEffects(array, start, j-1);
		SortStatusEffects(array, j+1, end);
	
	}


}
