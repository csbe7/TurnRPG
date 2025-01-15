using Godot;
using System;

public partial class PartyManagementInterface : Control
{
    GridContainer PartyGrid;

    PartyCharacterIcon currSelection;

    [Export] PackedScene PartyIcon;
   
    public override void _Ready()
    {
        PartyGrid = GetNode<GridContainer>("%Avaible Party");

        foreach (Sheet s in Game.state.avaible_party)
        {
            PartyCharacterIcon pci = PartyIcon.Instantiate<PartyCharacterIcon>();
            pci.SetSheet(s);
            PartyGrid.AddChild(pci);
            pci.ButtonDown += OnCharacterSelected;
            pci.HideButtons();
            pci.RemoveButtonDown += RemoveFromParty;
            pci.AddButtonDown += AddToParty;
        }
    }

    void OnCharacterSelected(PartyCharacterIcon pci)
    {
        if (IsInstanceValid(currSelection))
        {
            currSelection.HideButtons();
        }
        currSelection = pci;

        if (Game.state.current_party.Contains(pci.sheet)) pci.ShowRemoveFromParty();
        else pci.ShowAddToParty();
    }

    void RemoveFromParty(PartyCharacterIcon pci)
    {
        Game.state.current_party.Remove(pci.sheet);
        pci.ShowAddToParty();
    }
    void AddToParty(PartyCharacterIcon pci)
    {
        Game.state.current_party.Add(pci.sheet);
        pci.ShowRemoveFromParty();
    }
}
