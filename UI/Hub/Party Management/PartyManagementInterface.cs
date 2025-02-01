using Godot;
using System;
using System.Dynamic;

public partial class PartyManagementInterface : Control
{
    GridContainer PartyGrid;

    PartyCharacterIcon currSelection;

    [Export] PackedScene PartyIcon;
    [Export] PackedScene inventoryItemButton;
   
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
            //pci.RemoveButtonDown += RemoveFromParty;
            //pci.AddButtonDown += AddToParty;
        }

        Button cib = GetNode<Button>("%Inventory Display/Close Inventory Button");
        cib.ButtonDown += () => {
            cib.GetParent<Control>().Hide();
        };

    }

    void OnCharacterSelected(PartyCharacterIcon pci)
    {
        if (IsInstanceValid(currSelection))
        {
            currSelection.HideButtons();
            if (currSelection == pci)
            {
                GetNode<Control>("%Character Info").Hide();
            }
        }
        currSelection = pci;

        //if (Game.state.current_party.Contains(pci.sheet)) pci.ShowRemoveFromParty();
        //else pci.ShowAddToParty();
        LoadCharacterInfo(pci.sheet);
    }

    void LoadCharacterInfo(Sheet c)
    {
        Control ci = GetNode<Control>("%Character Info");
        ci.Show();

        EquipmentButton armorButton = GetNode<EquipmentButton>("%Armor Button");
        EquipmentButton weaponButton = GetNode<EquipmentButton>("%Weapon Button");
        EquipmentButton accessoryButton = GetNode<EquipmentButton>("%Accessory Button");

        armorButton.SetItem(c.equips.armor);
        weaponButton.SetItem(c.equips.weapon);
        accessoryButton.SetItem(c.equips.accessory);

        armorButton.ButtonClicked += EquipmentButtonDown;
        weaponButton.ButtonClicked += EquipmentButtonDown;
        accessoryButton.ButtonClicked += EquipmentButtonDown;
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


    void ClearInventory()
    {
        GridContainer gc = GetNode<GridContainer>("%Inventory Container/GridContainer");
        foreach (var child in gc.GetChildren()) child.QueueFree();
    }

    async void EquipmentButtonDown(EquipmentButton eb)
    {
        ClearInventory();
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        Control id = GetNode<Control>("%Inventory Display");
        id.Hide();
        GridContainer gc = GetNode<GridContainer>("%Inventory Container/GridContainer");

        if (IsInstanceValid(eb.item))
        {
            ItemButton iib = inventoryItemButton.Instantiate<ItemButton>();
            iib.b.Text = "Unequip";
            iib.l.Hide();
            iib.ButtonClicked += (b) => {
                if (!Game.state.player_inventory.CanAddItem(eb.item)) return;

                currSelection.sheet.UnequipItem(eb.item.slot);
                eb.SetItem(null);
                id.Hide();
            };
            gc.AddChild(iib);
        }

        foreach(ItemSlot i in Game.state.player_inventory.items)
        {
            if (i.item is EquippableItem ei && eb.slot == ei.slot)
            {
                ItemButton iib = inventoryItemButton.Instantiate<ItemButton>();
                iib.SetItemSlot(i);

                iib.ButtonClicked += (b) => {
                    if (!IsInstanceValid(eb.item) || Game.state.player_inventory.CanAddItem(eb.item))
                    {
                        Game.state.player_inventory.RemoveItem(iib.item);

                        currSelection.sheet.EquipItem((EquippableItem)iib.item.item);
                        
                        id.Hide();

                        eb.SetItem((EquippableItem)iib.item.item);

                    }
                    ClearInventory();
                };

                gc.AddChild(iib);
            }
        }

        if (gc.GetChildCount() != 0) id.Show();  
    }
}
