using Godot;
using System;
using System.Linq;

public partial class Game : Node
{
    public static GameState state = (GameState)ResourceLoader.Load("res://Game States/GameState.tres");


    [ExportCategory("Interfaces")]
    public static PackedScene hubInterface = (PackedScene)ResourceLoader.Load("res://UI/Hub/hub_screen.tscn");
    public static PackedScene partyManagementInterface = (PackedScene)ResourceLoader.Load("res://UI/Hub/Party Management/party_management_interface.tscn");

    public static PackedScene explorationInterface = (PackedScene)ResourceLoader.Load("res://UI/Exploration/exploration_screen.tscn");
    public static PackedScene dialogueEventInterface = (PackedScene)ResourceLoader.Load("res://UI/Dialogue/dialogue_event_interface.tscn");
    public static PackedScene battleInterface = (PackedScene)ResourceLoader.Load("res://UI/Combat/battle_interface.tscn");



    [ExportCategory("Settings")] 
    public static int maxPartyMembers = 4;






    //Dialogue
    public static void DialogueSignalHandler(string command)
    {
        string[] arg = command.Split(",");

        if (arg[0] == "in_party")
        {
            if (arg.Length != 3)
            {
                GD.PrintErr("Wrong amount of arguments in signal");
                return;
            }
            state.CheckCurrentParty(arg[1], arg[2]);
        }
        else if (arg[0] == "is_avaible")
        {
            if (arg.Length != 3)
            {
                GD.PrintErr("Wrong amount of arguments in signal");
                return;
            }
            state.CheckAvaibleParty(arg[1], arg[2]);
        }
        else if (arg[0] == "has_item")
        {
            if (arg.Length != 3)
            {
                GD.PrintErr("Wrong amount of arguments in signal");
                return;
            }
            state.CheckInventory(arg[1], arg[2]);
        }

    }


}
