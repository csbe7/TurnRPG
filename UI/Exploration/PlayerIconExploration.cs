using Godot;
using System;

public partial class PlayerIconExploration : Control
{
    [Export] float moveSpeed;
    Vector2 target, startPos;
    Vector2I targetSquare;
    public Vector2I currSquare;

    [Signal] public delegate void ArrivedEventHandler(Vector2I startSquare, Vector2I endSquare);

    bool isMoving;

    public override void _Ready()
    {
        targetSquare = currSquare;
        SetPhysicsProcess(false);
    }

    public void Move(Vector2 t, Vector2I tsquare)
    {
        target = t;
        targetSquare = tsquare;
        startPos = this.Position;
        isMoving = true;
        SetPhysicsProcess(true);
    }

    public override void _PhysicsProcess(double delta)
    {
        Position += (target - Position).Normalized() * moveSpeed;
        if (Position.DistanceTo(target) < 5f)
        {
            Position = target;
            EmitSignal(SignalName.Arrived, currSquare, targetSquare);
            isMoving = false;
            SetPhysicsProcess(false);
        }
    }

    public void CancelMove()
    {
        GD.Print("Move Cancelled");
        targetSquare = currSquare;
        target = startPos;
        EmitSignal(SignalName.Arrived, currSquare, targetSquare);
        SetPhysicsProcess(false);
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public void SetCurrentPosition(Vector2I pos)
    {
        currSquare = pos;
        targetSquare = pos;
    }
}
