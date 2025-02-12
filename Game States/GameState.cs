using Godot;
using System;
using System.Diagnostics;

[GlobalClass]
public partial class GameState : Resource
{
    [Export] public Godot.Collections.Array<Sheet> avaible_party = new Godot.Collections.Array<Sheet>();
    [Export] public Godot.Collections.Array<Sheet> current_party = new Godot.Collections.Array<Sheet>();

    [Export] public Inventory player_inventory = new Inventory(0);
    [Export] public Inventory exploration_inventory = new Inventory(10);
    [Export] public Godot.Collections.Dictionary<string, Item> avaible_items = new Godot.Collections.Dictionary<string, Item>();

    public void LoadAvaibleParty()
    {
        var dir = DirAccess.Open("res://Sheets/Party");
        if (dir == null) 
        {
            GD.Print("dir not found");
            return;
        }
        avaible_party.Clear();
        dir.ListDirBegin();
        string filename = dir.GetNext();
        while (filename != "")
        {
            if (dir.CurrentIsDir()) continue;
            Sheet s = (Sheet)ResourceLoader.Load("res://Sheets/Party/" + filename);
            avaible_party.Add(s);
            //current_party.Add(s); //DEBUG
            filename = dir.GetNext();
        }
    }    
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
            avaible_items.Add(i.name, i);
            filename = dir.GetNext();
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
        bool contains = player_inventory.HasItem(toCheck); 
        
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
        bool contains = player_inventory.HasItem(toCheck);
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
