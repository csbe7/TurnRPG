using Godot;
using System;

public partial class StatusEffect : Node
{
    public Sheet giver;
    public Sheet receiver;
    [Export] public int priority = 0;

    [Export] public float power = 1;

    [Export] public int duration; //0 = infinite

    private int durationLeft;

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
        QueueFree();
    }


}
