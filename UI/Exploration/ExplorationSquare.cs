using Godot;
using System;

public partial class ExplorationSquare : Control
{
    Button button;
    public Vector2I gridPos;
    Sprite2D sprite;
    public Texture2D icon;
    
    [Signal] public delegate void ButtonClickedEventHandler(ExplorationSquare self);
    [Signal] public delegate void PlayerEnteredEventHandler();

    public bool explored;

    public Event squareEvent;


    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("Sprite2D");
        button = GetNode<Button>("Button");
        button.ButtonDown += OnButtonDown;
    }

    void OnButtonDown()
    {
        EmitSignal(SignalName.ButtonClicked, this);
    }

    public void SetIcon(Texture2D t)
    {
        sprite.Texture = t;
    }

    public void SetIconVisibility(bool show)
    {
        if (show) sprite.Show();
        else sprite.Hide();
    }

    public void SetEvent(Event e)
    {
        if (!IsInstanceValid(e)) return;
        squareEvent = e;
        SetIcon(e.icon);
    }
}
