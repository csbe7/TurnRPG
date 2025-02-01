using Godot;
using System;

public partial class DialogueTextUI : RichTextLabel
{
    [Export] public Vector2 minSize;
    [Export] public float maxYSize;
    ScrollContainer sc;
    public override void _Ready()
    {
        sc = GetParent<ScrollContainer>();
    }

    public override void _PhysicsProcess(double delta)
    {
        CustomMinimumSize = new Vector2(minSize.X, CustomMinimumSize.Y);
        Size = CustomMinimumSize;

        float ySize = minSize.Y * GetLineCount();
        ySize = Mathf.Clamp(ySize, minSize.Y, maxYSize);

        CustomMinimumSize = new Vector2(minSize.X, ySize);
        Size = CustomMinimumSize;

        sc.CustomMinimumSize = new Vector2(minSize.X, ySize);
        sc.Size = new Vector2(minSize.X, ySize);
    }

}
