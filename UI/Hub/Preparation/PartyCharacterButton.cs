using Godot;
using System;

public partial class PartyCharacterButton : Control
{
    public Sheet sheet;
    [Export] public Button b;
    [Export] public Label l;

    public void SetSheet(Sheet s)
    {
        sheet = s;
        l.Text = s.name;
    }
}
