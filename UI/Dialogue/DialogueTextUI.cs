using Godot;
using System;

public partial class DialogueTextUI : RichTextLabel
{
    [Export] Vector2 minSize;
    [Export] float maxYsize;
    ScrollContainer sc;
    public override void _Ready()
    {
        sc = GetParent<ScrollContainer>();
    }

    public override void _PhysicsProcess(double delta)
    {
        float ySize = minSize.Y * GetLineCount();
        ySize = Mathf.Clamp(ySize, minSize.Y, maxYsize);
        CustomMinimumSize = new Vector2(minSize.X, ySize);
        Size = new Vector2(minSize.X, ySize);
        sc.CustomMinimumSize = new Vector2(minSize.X, ySize);
        sc.Size = new Vector2(minSize.X, ySize);
    }

}
