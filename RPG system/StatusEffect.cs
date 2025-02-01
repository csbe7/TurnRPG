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

    private int durationLeft;

    [Signal] public delegate void EffectEndedEventHandler(StatusEffect self);

    public override void _Ready()
    {
        durationLeft = duration;
        if (!countRounds) receiver.TurnEnded += Countdown;
        else
        {
            CombatManager cm = GetTree().Root.GetNode<CombatManager>("CombatManager");
        }
    }

    public virtual void Countdown()
    {
        GD.Print("Countdown");
        ApplyEffect();
        durationLeft--;
        

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


}
