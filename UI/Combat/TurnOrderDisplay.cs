using Godot;
using System;

public partial class TurnOrderDisplay : Control
{
    [Export] Vector2 turnDisplaySize;
    [Export] float moveSpeed = 2;

    public GridContainer grid;

    Vector2 targetPos;

    public override void _Ready()
    {
        grid = GetNode<GridContainer>("GridContainer");
        SetPhysicsProcess(false);
        PositionDisplay(0);
    }

    public void PositionDisplay(int pos)
    {
        Vector2 screenSize =  GetViewport().GetVisibleRect().Size;
        float halfpoint = screenSize.Y/2 - turnDisplaySize.Y/2;
        targetPos = new Vector2(grid.Position.X, halfpoint - (turnDisplaySize.Y * pos));
        SetPhysicsProcess(true);
    }

    public override void _PhysicsProcess(double delta)
    {
        grid.Position += (targetPos - grid.Position).Normalized() * moveSpeed;
        if (grid.Position.DistanceTo(targetPos) <= 1f)
        {
            grid.Position = targetPos;
            SetPhysicsProcess(false);
        }
    }
}
