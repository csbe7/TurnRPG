using Godot;
using System;

[GlobalClass]
public partial class GameState : Resource
{
    [Export] public Godot.Collections.Array<Sheet> avaible_party = new Godot.Collections.Array<Sheet>();
    [Export] public Godot.Collections.Array<Sheet> current_party = new Godot.Collections.Array<Sheet>();

    [Export] public Godot.Collections.Array<ItemSlot> inventory = new Godot.Collections.Array<ItemSlot>();

    public Godot.Collections.Dictionary variables = new Godot.Collections.Dictionary
    {
        //Temporary variables
        {"temp", new int()},
        
        //Player info
        {"PlayerName", new string("Player")},
        {"Gold", new int()},
    };
    


    public bool CheckAvaibleParty(Sheet toCheck, string dictEntry = null)
    {
        bool contains = avaible_party.Contains(toCheck);
        if (dictEntry != null)
        {
            if (variables.ContainsKey(dictEntry)){
                if (contains == true) variables[dictEntry] = 1;
                else variables[dictEntry] = 0;
            }
            else GD.PrintErr("Result key not present in variables dictionary");
        }
        return contains;
    }
    public bool CheckAvaibleParty(string toCheck, string dictEntry = null)
    {
        bool contains = false;
        foreach (Sheet s in avaible_party)
        {
            if (s.name == toCheck){
                contains = true;
                break;
            } 
        }
        if (dictEntry != null)
        {
            if (variables.ContainsKey(dictEntry)){
                if (contains == true) variables[dictEntry] = 1;
                else variables[dictEntry] = 0;
            }
            else GD.PrintErr("Result key not present in variables dictionary");
        }
        return contains;
    }

    public bool CheckCurrentParty(Sheet toCheck, string dictEntry = null)
    {
        bool contains = current_party.Contains(toCheck);
        if (dictEntry != null)
        {
            if (variables.ContainsKey(dictEntry)){
                if (contains == true) variables[dictEntry] = 1;
                else variables[dictEntry] = 0;
            }
            else GD.PrintErr("Result key not present in variables dictionary");
        }
        return contains;
    }
    public bool CheckCurrentParty(string toCheck, string dictEntry = null)
    {
        bool contains = false;
        foreach (Sheet s in current_party)
        {
            if (s.name == toCheck){
                contains = true;
            } 
        }
        if (dictEntry != null)
        {
            if (variables.ContainsKey(dictEntry)){
                if (contains == true) variables[dictEntry] = 1;
                else variables[dictEntry] = 0;
            }
            else GD.PrintErr("Result key not present in variables dictionary");
        }
        return contains;
    }

    public bool CheckInventory(Item toCheck, string dictEntry = null)
    {
        bool contains = false; 
        foreach (ItemSlot i in inventory)
        {
            if (i.item == toCheck){
                contains = true;
                break;
            } 
        }
        if (dictEntry != null)
        {
            if (variables.ContainsKey(dictEntry)){
                if (contains == true) variables[dictEntry] = 1;
                else variables[dictEntry] = 0;
            }
            else GD.PrintErr("Result key not present in variables dictionary");
        }
        return contains;
    }
    public bool CheckInventory(string toCheck, string dictEntry = null)
    {
        bool contains = false;
        foreach (ItemSlot i in inventory)
        {
            if (i.item.name == toCheck){
                contains = true;
                break;
            } 
        }
        if (dictEntry != null)
        {
            if (variables.ContainsKey(dictEntry)){
                if (contains == true) variables[dictEntry] = 1;
                else variables[dictEntry] = 0;
            }
            else GD.PrintErr("Result key not present in variables dictionary");
        }
        return contains;
    }
    
}
