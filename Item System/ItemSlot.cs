using Godot;
using System;

[GlobalClass]
public partial class ItemSlot : Resource
{
    [Export] public Item item;
    [Export] public int amount;

    public ItemSlot(Item item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
}
