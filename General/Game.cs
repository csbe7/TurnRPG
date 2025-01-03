using Godot;
using System;

public partial class Game : Node
{
    public static GameState state = (GameState)ResourceLoader.Load("res://Game States/GameState.tres");
}
