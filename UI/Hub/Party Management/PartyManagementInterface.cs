using Godot;
using System;

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

    void EquipmentButtonDown(EquipmentButton eb)
    {
        GridContainer gc = GetNode<GridContainer>("%Inventory Display/GridContainer");
        foreach(ItemSlot i in Game.state.playerInventory.items)
        {
            if (i.item is EquippableItem ei && eb.slot == ei.slot)
            {
                ShopItemButton iib = inventoryItemButton.Instantiate<ShopItemButton>();
                iib.SetItemSlot(i);
                gc.AddChild(iib);
            }
        }
    }
}
