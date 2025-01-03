using Godot;
using System;

public partial class PlayerInfo : Node
{
    public static Godot.Collections.Array<Sheet> current_party = new Godot.Collections.Array<Sheet>();

    public override void _Ready()
    {
        Sheet testParty1 = (Sheet)ResourceLoader.Load("res://Sheets/Party/TestParty.tres");
        Sheet testParty2 = (Sheet)ResourceLoader.Load("res://Sheets/Party/TestParty2.tres");

        current_party.Add(testParty1);
        current_party.Add(testParty2);
        current_party.Add(testParty1);
        current_party.Add(testParty1);
    }
}
