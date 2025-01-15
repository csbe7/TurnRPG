using Godot;
using System;

public partial class ExplorationSquare : Control
{
    Button button;
    public Vector2I gridPos;
    Sprite2D sprite;
    public Texture2D icon;

    [Export] float iconFadeInTime;
    
    [Signal] public delegate void ButtonClickedEventHandler(ExplorationSquare self);
    [Signal] public delegate void PlayerEnteredEventHandler();
    [Signal] public delegate void IconFadedInEventHandler();

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

    int physicsUsageCounter;
    int PhysicsUsageCounter{
        get{
            return physicsUsageCounter;
        }
        set{
            physicsUsageCounter = value;
            if (physicsUsageCounter > 0) SetPhysicsProcess(true);
            else SetPhysicsProcess(false);
        }
    }
    float fadeInProgress;
    float fadeOutProgress;
    bool fadingIn;
    bool fadingOut;
    public override void _PhysicsProcess(double delta)
    {
        if (fadingIn)
        {
            if (fadeInProgress >= 1)
            {
                EmitSignal(SignalName.IconFadedIn);
                fadingIn = true;
                PhysicsUsageCounter--;
                return;
            }
            fadeInProgress += (float)delta * iconFadeInTime;
            sprite.SelfModulate = new Color(1, 1, 1, fadeInProgress);
        }

        if (fadingOut)
        {
            if (fadeOutProgress >= 1)
            {
                EmitSignal(SignalName.IconFadedIn);
                fadingOut = false;
                PhysicsUsageCounter--;
                return;
            }
            fadeOutProgress += (float)delta * iconFadeInTime;
            sprite.SelfModulate = new Color(1, 1, 1, 1 - fadeOutProgress);
        }
    }
    
    public void OnExplore()
    {
        FadeInIcon();
    }

    public void FadeInIcon()
    {
        sprite.SelfModulate = new Color(1, 1, 1, 0);
        SetIconVisibility(true);
        PhysicsUsageCounter++;
        fadeInProgress = 0;
        fadingIn = true;
    }

    public void FadeOutIcon()
    {
        sprite.SelfModulate = new Color(1, 1, 1, 1);
        SetIconVisibility(true);
        PhysicsUsageCounter++;
        fadeOutProgress = 0;
        fadingOut = true;
    }
}
