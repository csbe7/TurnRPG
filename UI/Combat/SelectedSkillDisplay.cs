using Godot;
using System;

public partial class SelectedSkillDisplay : Control
{
    [Export] int borderWidth = 3;
    [Export] Vector2 buttonSpacing;
    ColorRect bg, border;
    Label label;
    public Button button;

    public override void _Ready()
    {
        bg = GetNode<ColorRect>("%Background");
        border = GetNode<ColorRect>("%Border");
        label = GetNode<Label>("%Label");
        button = GetNode<Button>("%Button");
        SetText("Miao");
    }

    public void SetText(String text)
    {
        label.Text = text;

        bg.Size = new Vector2(bg.Size.X + label.Size.X + 4, bg.Size.Y);
        border.Size = new Vector2(bg.Size.X + (borderWidth*2), bg.Size.Y + (borderWidth*2));
        //bg.Position = new Vector2(bg.Position.X + borderWidth, bg.Position.Y + borderWidth);
        border.Position = new Vector2(bg.Position.X - borderWidth, bg.Position.Y - borderWidth);
    }
}
