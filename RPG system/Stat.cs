using Godot;
using System;


[GlobalClass]
public partial class Stat : Resource
{
	[Signal] public delegate void ModifierAddedEventHandler(StatModifier modifier);
	[Signal] public delegate void ModifierRemovedEventHandler(StatModifier modifier);
	[Signal] public delegate void ValueChangedEventHandler(float oldVal, float newVal);

    [Export] public float minValue = 0, maxValue = 9999;

	public float value;

	[Export] public float Value
    {
		get{
			return value;
		}
		set{
		   float old = Value;
		   this.value = Mathf.Clamp(value, minValue, maxValue);
		   hasBeenModified = true;
           EmitSignal(SignalName.ValueChanged, old, this.value);
		}
	}
	
	private float modValue;
	public float ModValue{
		get{
			if (hasBeenModified) {
				QuickSort(modifiers, 0, modifiers.Count - 1);
				ModValue = Calculate();
                hasBeenModified = false;
			}
			return modValue;
		}
		private set{
			modValue = value;
		}
	}

	private Godot.Collections.Array<StatModifier> modifiers;
	public bool hasBeenModified = true;

	public Stat(float v, float min, float max)
	{
		value = v;
		minValue = min;
		maxValue = max;

		modifiers = new Godot.Collections.Array<StatModifier>{};
	}

	public Stat(float v) : this(v, 0, 999) { } 

	public Stat() : this(0, 0, 999) { }


	public void AddModifier(StatModifier mod)
	{
		modifiers.Add(mod);
        hasBeenModified = true;
        EmitSignal(SignalName.ModifierAdded, mod);
	}

	public void RemoveModifier(StatModifier mod)
	{
		modifiers.Remove(mod);
        hasBeenModified = true;
		EmitSignal(SignalName.ModifierRemoved, mod);
	}

	private float Calculate()
    {
        float finalValue = value;

        foreach(StatModifier sm in modifiers)
        {
			switch (sm.mode){
				case StatModifier.Mode.Flat:
                finalValue += sm.value;
				break;

				case StatModifier.Mode.PercentageFromBase:
				finalValue += (value/100) * sm.value;
				break;

				case StatModifier.Mode.Percentage:
				finalValue += (finalValue/100) * sm.value;
				break;
			}
           
		}

		return Mathf.Clamp(finalValue, minValue, maxValue);
	}

	private static void QuickSort(Godot.Collections.Array<StatModifier> array, int start, int end)
	{
		if (end <= start) return;

		StatModifier temp;
		int i, j = start-1;

        for(i = start; i < end; i++)
		{
			if (array[i].order < array[end].order)
			{
				j++;
				temp = array[j];
                array[j] = array[i];
				array[i] = temp;
			}
		}
		j++;
		temp = array[j];
		array[j] = array[end];
		array[end] = temp;

		QuickSort(array, start, j-1);
		QuickSort(array, j+1, end);
	
	}

	
}

