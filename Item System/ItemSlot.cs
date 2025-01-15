using Godot;
using System;

[GlobalClass]
public partial class ItemSlot : Resource
{
    [Export] public Item item;
    [Export] public int amount;
}
