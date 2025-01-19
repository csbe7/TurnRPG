using Godot;
using System;

public partial class ShopItemButton : Control
{
    public ItemSlot item;
    [Signal] public delegate void ButtonClickedEventHandler(ShopItemButton sib);

    public bool player = false;
    [Export] Button b;
    [Export] Label l;

    public override void _Ready()
    {
        b.ButtonDown += () => EmitSignal(SignalName.ButtonClicked, this);
    }

    public void SetItemSlot(ItemSlot i)
    {
        item = i;
        b.Text = i.item.name;
        l.Text = "x" + i.amount;
    }

}
