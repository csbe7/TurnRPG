using Godot;
using System;

public partial class StatusEffect : Node
{
    [Export] public Texture2D Icon;
    [Export] public bool hidden;
    public Sheet giver;
    public Sheet receiver;
    [Export] public int priority = 0;

    [Export] bool countRounds = false;

    [Export] public int duration; //0 = infinite

    public int durationLeft;

    [Signal] public delegate void EffectEndedEventHandler(StatusEffect self);
    [Signal] public delegate void DurationChangedEventHandler(StatusEffect self);

    public override void _Ready()
    {
        durationLeft = duration;
    
        CombatManager cm = GetTree().Root.GetNode<CombatManager>("CombatManager");
        cm.TurnEnded += Countdown;
    }

    public virtual void Countdown()
    {
        GD.Print("Countdown");
        ApplyEffect();
        durationLeft--;
        EmitSignal(SignalName.DurationChanged, this);

        if (durationLeft <= 0 && duration != 0) EndEffect();
    }

    public virtual void ApplyEffect() {}


    public virtual void EndEffect()
    {
        if (!IsInstanceValid(this)){ 
            GD.Print("Invalid");
            return;
        }

        EmitSignal(SignalName.EffectEnded, this);
        //QueueFree();
    }

    new public void Free()
    {
        CombatManager cm = GetTree().Root.GetNode<CombatManager>("CombatManager");
        cm.TurnEnded -= Countdown;
    }


}
