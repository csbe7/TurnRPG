using Godot;
using System;

[GlobalClass]
public partial class GameState : Resource
{

    public Godot.Collections.Dictionary variables = new Godot.Collections.Dictionary
    {
        {"temp", new int()},
        
        {"PlayerName", new string("Player")},
    };

}
