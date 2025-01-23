using Godot;
using System;

public partial class Inventory : Resource
{
    public Godot.Collections.Array<ItemSlot> items = new Godot.Collections.Array<ItemSlot>();
    public int maxSlots = 10;

    public Inventory()
    {
        maxSlots = 0;
        items = new Godot.Collections.Array<ItemSlot>();
    }
    public Inventory(int maxSlots, Godot.Collections.Array<ItemSlot> items = null)
    {
        this.maxSlots = maxSlots;
        if (items != null) this.items = items;
        else items = new Godot.Collections.Array<ItemSlot>();
    }

    public bool HasItem(Item item, int amount = 1, bool less = false)
    {
        foreach(ItemSlot slot in items)
        {
            if (slot.item == item && ((slot.amount >= amount && !less) || (slot.amount < amount && less))) return true; 
        }
        return false;
    }
    public bool HasItem(string itemName, int amount = 1, bool less = false)
    {
        foreach(ItemSlot slot in items)
        {
            if (slot.item.name == itemName && ((slot.amount >= amount && !less) || (slot.amount < amount && less))) return true; 
        }
        return false;
    }

    public bool RemoveItem(Item item, int amount = 1)
    {
        ItemSlot toRemove = null;
        foreach (ItemSlot itemSlot in items)
        {
            if (itemSlot.item == item)
            {
                if (itemSlot.amount > amount)
                {
                    itemSlot.amount -= amount;
                    return true;
                }

                if (itemSlot.amount == amount)
                {
                    toRemove = itemSlot;
                    break;
                } 
            }
        }
        if (toRemove != null)
        {
            items.Remove(toRemove);
            return true;
        }
        return false;
    }
    public bool RemoveItem(string itemName, int amount = 1)
    {
        ItemSlot toRemove = null;
        foreach (ItemSlot itemSlot in items)
        {
            if (itemSlot.item.name == itemName)
            {
                if (itemSlot.amount > amount)
                {
                    itemSlot.amount -= amount;
                    return true;
                }

                if (itemSlot.amount == amount)
                {
                    toRemove = itemSlot;
                    break;
                } 
            }
        }
        if (toRemove != null)
        {
            items.Remove(toRemove);
            return true;
        }
        return false;
    }
    public bool RemoveItem(ItemSlot slot, int amount = 1)
    {
        ItemSlot toRemove = null;
        foreach (ItemSlot itemSlot in items)
        {
            if (itemSlot == slot)
            {
                if (itemSlot.amount > amount)
                {
                    itemSlot.amount -= amount;
                    return true;
                }

                if (itemSlot.amount == amount)
                {
                    toRemove = itemSlot;
                    break;
                } 
            }
        }
        if (toRemove != null)
        {
            items.Remove(toRemove);
            return true;
        }
        return false;
    }

    public bool CanAddItem(Item item, int amount = 1)
    {
        if (!IsInstanceValid(item)) return false;
        foreach(ItemSlot itemSlot in items)
        {
            if (itemSlot.item == item)
            {
                if (itemSlot.amount < item.maxStack)
                {
                    int insert = Mathf.Clamp(item.maxStack - itemSlot.amount, 0, amount);
                    amount -= insert;
                    if (amount == 0) return true;
                }
            }
        }

        int c = items.Count;
        while (amount > 0 && (c < maxSlots || maxSlots == 0))
        {
            int insert = Mathf.Min(item.maxStack, amount);
            amount -= insert;
            c++;
        }

        if (amount < 0) GD.Print("Error in CanAddItem");
        if (amount > 0) return false;
        else return true;
    }

    public bool AddItem(Item item, int amount = 1)
    {
        if (!CanAddItem(item, amount)) return false;

        foreach(ItemSlot itemSlot in items)
        {
            if (itemSlot.item == item)
            {
                if (itemSlot.amount < item.maxStack)
                {
                    int insert = Mathf.Clamp(item.maxStack - itemSlot.amount, 0, amount);
                    itemSlot.amount += insert;
                    amount -= insert;
                    if (amount == 0) return true;
                }
            }
        }
        while (amount > 0 && (items.Count < maxSlots || maxSlots == 0))
        {
            int insert = Mathf.Min(item.maxStack, amount);
            ItemSlot newSlot = new ItemSlot(item, insert);
            items.Add(newSlot);
            amount -= insert;
        }

        if (amount < 0) GD.Print("Error in AddItem");
        if (amount > 0) return false;
        else return true;
    }


}
