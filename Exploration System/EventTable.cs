using Godot;
using System;

[GlobalClass]
public partial class EventTable : Resource
{
    [Export] public Godot.Collections.Dictionary<Event, int> possible_events = new Godot.Collections.Dictionary<Event, int>();
    [Export] public int no_event_chance;

    [Export] public Event exitEvent;

    public Event PickEvent()
    {
        int max = no_event_chance;
        foreach(Event key in possible_events.Keys)
        {
            max += possible_events[key];
        }

        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Randomize();
        int val = rng.RandiRange(0, max);

        int curr = 0;
        foreach(Event key in possible_events.Keys)
        {
            curr += possible_events[key];
            if (curr >= val) return key;
        }

        return null;
    }
}
