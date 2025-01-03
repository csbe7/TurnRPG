using Godot;
using System;

[GlobalClass]
public partial class Encounter : Event
{
   [Export] public Godot.Collections.Array<Sheet> enemies = new Godot.Collections.Array<Sheet>();

   public void Resolve()
   {
      EmitSignal(SignalName.EventResolved, this);
   }
}
