using Godot;
using System;

public partial class ItemButton : Control
{
    public ItemSlot item;
    [Signal] public delegate void ButtonClickedEventHandler(ItemButton sib);

    public bool player = false;
    [Export] public Button b;
    [Export] public Label l;

    public override void _Ready()
    {
        b.ButtonDown += () => EmitSignal(SignalName.ButtonClicked, this);
    }

    public void SetItemSlot(ItemSlot i)
    {
        item = i;
        b.Text = i.item.name;
        l.Text = "x" + i.amount;
        if (i.amount == 1) l.Hide();
        else l.Show();
    }

    

}
