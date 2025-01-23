using Godot;
using System;

public partial class PreparationInterface : Control
{
    [Export] GridContainer partyContainer;
    [Export] GridContainer chosenPartyContainer;
    [Export] GridContainer inventoryContainer, backpackContainer;

    [Export] PackedScene partyCharacterButton, partyCharacterIcon, itemButton;

    [Export] Control partySelect, itemSelect;

    public override void _Ready()
    {
        partySelect.Show();
        itemSelect.Hide();

        GetNode<Button>("%Embark Button").ButtonDown += () =>{
            ExplorationManager em = Game.explorationInterface.Instantiate<ExplorationManager>();
            GetParent().AddChild(em);
        };

        GetNode<Button>("%Backpack Button").ButtonDown += () =>{
            partySelect.Hide();
            itemSelect.Show();
            DrawInventory();
        };

        foreach(Node child in partyContainer.GetChildren() + chosenPartyContainer.GetChildren()) child.QueueFree();


        foreach (Sheet character in Game.state.avaible_party)
        {
            DeselectCharacter(character);
        }        
    }

    public void SelectCharacter(Sheet character)
    {
        if (!Game.state.current_party.Contains(character)) Game.state.current_party.Add(character);
        PartyCharacterIcon pci = partyCharacterIcon.Instantiate<PartyCharacterIcon>();
        pci.SetSheet(character);
        pci.ButtonDown += (b) => {
            DeselectCharacter(character);
            pci.QueueFree();
        };
        chosenPartyContainer.AddChild(pci);
    }

    public void DeselectCharacter(Sheet character)
    {
        if (Game.state.current_party.Contains(character)) Game.state.current_party.Remove(character);
        PartyCharacterButton pcb = partyCharacterButton.Instantiate<PartyCharacterButton>();
        pcb.SetSheet(character);
        pcb.b.ButtonDown += () => {
            pcb.QueueFree();
            SelectCharacter(character);
        };
        partyContainer.AddChild(pcb);
    }

    public void DrawInventory()
    {
        foreach(Node child in inventoryContainer.GetChildren() + backpackContainer.GetChildren()) child.QueueFree();

        foreach(ItemSlot slot in Game.state.player_inventory.items)
        {
            ItemButton ib = itemButton.Instantiate<ItemButton>();
            ib.SetItemSlot(slot);
            ib.ButtonClicked += (b) => {
                if (Game.state.exploration_inventory.AddItem(b.item.item))
                {
                    Game.state.player_inventory.RemoveItem(b.item);
                    DrawInventory();
                }
            };
            inventoryContainer.AddChild(ib);
        }

        foreach(ItemSlot slot in Game.state.exploration_inventory.items)
        {
            ItemButton ib = itemButton.Instantiate<ItemButton>();
            ib.SetItemSlot(slot);
            ib.ButtonClicked += (b) => {
                if (Game.state.player_inventory.AddItem(b.item.item))
                {
                    Game.state.exploration_inventory.RemoveItem(b.item);
                    DrawInventory();
                }
            };
            backpackContainer.AddChild(ib);
        }

        GetNode<Label>("%Backpack Label").Text = "BACKPACK " + Game.state.exploration_inventory.items.Count + "/" + Game.state.exploration_inventory.maxSlots;
    }


}
