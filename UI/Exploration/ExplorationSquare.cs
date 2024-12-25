using Godot;
using System;

public partial class ExplorationSquare : Control
{
    Button button;
    public Vector2I gridPos;
    
    [Signal] public delegate void ButtonClickedEventHandler(ExplorationSquare self);
    [Signal] public delegate void PlayerEnteredEventHandler();


    public override void _Ready()
    {
        button = GetNode<Button>("Button");
        button.ButtonDown += OnButtonDown;
    }

    void OnButtonDown()
    {
        EmitSignal(SignalName.ButtonClicked, this);
    }
}
