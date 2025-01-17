using Godot;
using System;

[GlobalClass]
public partial class GameState : Resource
{
    [Export] public Godot.Collections.Array<Sheet> avaible_party = new Godot.Collections.Array<Sheet>();
    [Export] public Godot.Collections.Array<Sheet> current_party = new Godot.Collections.Array<Sheet>();

    [Export] public Inventory playerInventory = new Inventory(10);
    [Export] public Godot.Collections.Array<Item> avaibleItems = new Godot.Collections.Array<Item>();
    public void LoadItems()
    {
        var dir = DirAccess.Open("res://Item System/Items");
        if (dir == null) 
        {
            GD.Print("dir not found");
            return;
        }
        dir.ListDirBegin();
        string filename = dir.GetNext();
        while (filename != "")
        {
            if (dir.CurrentIsDir()) continue;
            Item i = (Item)ResourceLoader.Load("res://Item System/Items/" + filename);
            avaibleItems.Add(i);
            filename = dir.GetNext();
            GD.Print(filename);
        }
    }


    public Godot.Collections.Dictionary variables = new Godot.Collections.Dictionary
    {
        //Temporary variables
        {"temp", new int()},
        
        //Player info
        {"PlayerName", new string("Player")},
        {"Gold", 40},
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
        bool contains = playerInventory.HasItem(toCheck); 
        
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
        bool contains = playerInventory.HasItem(toCheck);
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
