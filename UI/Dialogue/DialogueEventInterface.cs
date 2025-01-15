using Godot;
using System;

public partial class DialogueEventInterface : Control
{
    [Export] public Sprite2D sprite;
    [Export] public DialogBoxCsharpInterface dci;

    public DialogueEvent dialogueEvent;

    public override void _Ready()
    {
        dci.DialogueEnded += OnDialogueEnded;
        dci.SetDialogueJSON(dialogueEvent.dialogueJSON);
    }

    void OnDialogueEnded()
    {
        dialogueEvent.Resolve();
        QueueFree();
    }

}
