using Godot;
using System;

[GlobalClass]
public partial class Event : Resource
{
    [Export] public string name;
    [Export] public Texture2D icon;

    [Signal] public delegate void EventResolvedEventHandler(Event self);

    public void Resolve()
    {
        EmitSignal(SignalName.EventResolved, this);
    }
}
