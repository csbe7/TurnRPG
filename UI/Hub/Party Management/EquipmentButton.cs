using Godot;
using System;

public partial class EquipmentButton : Button
{
    [Export] public EquippableItem.Slot slot;
    public EquippableItem item;

    [Signal] public delegate void ButtonClickedEventHandler(EquipmentButton self);

    public override void _Ready()
    {
        ButtonDown += () => EmitSignal(SignalName.ButtonClicked, this);
    }

    public void SetItem(EquippableItem ei)
    {
        if (!IsInstanceValid(ei))
        {
            Text = "NONE";
            item = null;
            return;
        }
        item = ei;
        Text = ei.name;
    }

}
