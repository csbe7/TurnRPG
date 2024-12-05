using Godot;
using System;

public partial class StatusEffect : Node
{
    [Export] public Texture2D Icon;
    [Export] public bool hidden;
    public Sheet giver;
    public Sheet receiver;
    [Export] public int priority = 0;

    [Export] public int duration; //0 = infinite

    private int durationLeft;

    [Signal] public delegate void EffectEndedEventHandler(StatusEffect self);

    public override void _Ready()
    {
        durationLeft = duration;
        receiver.TurnEnded += OnTurnEnded;
    }

    public virtual void OnTurnEnded()
    {
        durationLeft--;

        if (durationLeft <= 0 && duration != 0) EndEffect();
    }

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
