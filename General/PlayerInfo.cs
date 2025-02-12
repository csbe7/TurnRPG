using Godot;
using System;

public partial class PlayerInfo : Node
{
    public override void _Ready()
    {
        Sheet testParty1 = (Sheet)ResourceLoader.Load("res://Sheets/Party/TestParty.tres");
        Sheet testParty2 = (Sheet)ResourceLoader.Load("res://Sheets/Party/TestParty2.tres");

        /*Game.state.avaible_party.Add(testParty1);
        Game.state.avaible_party.Add(testParty2);*/
    }
}
