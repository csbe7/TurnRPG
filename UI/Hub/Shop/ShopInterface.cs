using Godot;
using System;

public partial class ShopInterface : Control
{
    [Export] PackedScene shopItemButton;
    [Export] GridContainer playerGrid, shopGrid;

    public Inventory shopItems = new Inventory(20);

    public override void _Ready()
    {
        RandomizeItems(10);
        PopulateGrids();
    }

    public void RandomizeItems(int amount)
    {
        shopItems.items.Clear();
        int added = 0;
        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Randomize();
        while(added < amount)
        {
            foreach (String iname in Game.state.avaible_items.Keys)
            {
                Item i = Game.state.avaible_items[iname];
                float r = rng.RandfRange(0, Game.state.avaible_items.Count);
                if (r <= i.shopProbability)
                {
                   int toAdd = rng.RandiRange(1,i.maxStack);
                   if (added + toAdd > amount) toAdd = amount - added;
                   if (!shopItems.AddItem(i, toAdd)) GD.Print("AddItem fail");
                
                    added += toAdd;
                    if (added == amount) break;
                }
            }
        }
    }

    void PopulateGrids()
    {
        foreach (Node child in playerGrid.GetChildren() + shopGrid.GetChildren())
        {
            child.QueueFree();
        }

        foreach (ItemSlot itemSlot in Game.state.player_inventory.items)
        {
            ItemButton sib = shopItemButton.Instantiate<ItemButton>();
            sib.SetItemSlot(itemSlot);
            sib.player = true;
            playerGrid.AddChild(sib);
            sib.ButtonClicked += OnPlayerButtonClicked;
        }
        foreach (ItemSlot itemSlot in shopItems.items)
        {
            ItemButton sib = shopItemButton.Instantiate<ItemButton>();
            sib.SetItemSlot(itemSlot);
            shopGrid.AddChild(sib);
            sib.ButtonClicked += OnShopButtonClicked;
        }

    }

    void OnShopButtonClicked(ItemButton sib)
    {
        if ((int)Game.state.variables["Gold"] < sib.item.item.value) return;
        if (Game.state.player_inventory.CanAddItem(sib.item.item, 1))
        {
            shopItems.RemoveItem(sib.item, 1);
            Game.state.player_inventory.AddItem(sib.item.item, 1);
        }
        PopulateGrids();
        Game.state.variables["Gold"] = (int)Game.state.variables["Gold"]- sib.item.item.value;
    }

    void OnPlayerButtonClicked(ItemButton sib)
    {

    }
}
