using Godot;
using System;

public partial class HubButton : Control
{
    public Button b;
    [Export(PropertyHint.MultilineText)] string buttonText;

    public override void _Ready()
    {
        b = GetNode<Button>("Button");
        //b.Text = buttonText;
    }
}
