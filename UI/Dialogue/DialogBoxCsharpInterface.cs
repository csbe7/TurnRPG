using Godot;
using System;

public partial class DialogBoxCsharpInterface : Control
{
    GodotObject dialogueBox;
    [Export] Json test;

    [Signal] public delegate void DialogueEndedEventHandler();

    Godot.Collections.Dictionary dict = new Godot.Collections.Dictionary();
    public override void _Ready()
    {
        dialogueBox = (GodotObject)GetNode("Dialogue Box");
        SetDialogueJSON(test);
    }

    public void SetDialogueJSON(Json dialogueJSON)
    {
        if (IsInstanceValid(dialogueJSON)) dialogueBox.Call("LoadDialogue", dialogueJSON, Game.state.variables);
    }

    void OnEODReached()
    {
        EmitSignal(SignalName.DialogueEnded);
    }

    void OnDialogueSignalReceived(string signal)
    {
        Game.DialogueSignalHandler(signal);
    }
}
