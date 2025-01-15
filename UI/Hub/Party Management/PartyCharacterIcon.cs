using Godot;
using System;

public partial class PartyCharacterIcon : Control
{
    public Sheet sheet;

    [Signal] public delegate void ButtonDownEventHandler(PartyCharacterIcon self);
    [Signal] public delegate void RemoveButtonDownEventHandler(PartyCharacterIcon self);
    [Signal] public delegate void AddButtonDownEventHandler(PartyCharacterIcon self);

    [Export] public Button removeButton, addButton; 

    public override void _Ready()
    {
        Button b = GetNode<Button>("%Button");
        b.ButtonDown += OnButtonDown;
        removeButton.ButtonDown += OnRemoveButtonDown;
        addButton.ButtonDown += OnAddButtonDown;
    }

    public void SetSheet(Sheet s)
    {
        sheet = s;
        GetNode<Label>("%Character Name").Text = s.name;
    }

    private void OnAddButtonDown()
    {
        EmitSignal(SignalName.AddButtonDown, this);
    }


    private void OnRemoveButtonDown()
    {
        EmitSignal(SignalName.RemoveButtonDown, this);
    }


    private void OnButtonDown()
    {
        EmitSignal(SignalName.ButtonDown, this);
    }



    public void ShowRemoveFromParty()
    {
        addButton.Hide();
        removeButton.Show();
    }
    public void ShowAddToParty()
    {
        removeButton.Hide();
        addButton.Show();
    }
    public void HideButtons()
    {
        removeButton.Hide();
        addButton.Hide();
    }


}
