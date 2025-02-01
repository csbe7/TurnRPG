using Godot;
using System;
using System.Threading.Tasks;

public partial class DialogueAutoscrollInterface : Control
{
    [Export] ScrollContainer scrollContainer;
    [Export] RichTextLabel label;
    [Export] Button b;

    [Export] float scrollSpeed;

    public override void _Ready()
    {
        SetScroll(true);
        b.ButtonDown += () => {
            if (scrollEnded)
            {
                QueueFree();
            }
            else
            {
                OnScrollEnded();
            }
        };

        /*label.CustomMinimumSize = new Vector2(label.Size.X, label.GetLineCount() * 100);
        label.Size = label.CustomMinimumSize;*/
        //label.Position = new Vector2(label.Position.X, label.Position.Y - scrollContainer.Size.Y);
    }

    public void SetScroll(bool state)
    {
        if (state)
        {
            SetPhysicsProcess(true);
            scrollEnded = false;
        }
        else
        {
            SetPhysicsProcess(false);
            scrollEnded = true;
        }
    }



    float amountScrolled = 0;
    bool scrollEnded;
    public override void _PhysicsProcess(double delta)
    {
        label.Position = new Vector2(label.Position.X, label.Position.Y - (scrollSpeed * (float)delta));
        amountScrolled += scrollSpeed * (float)delta;
        if (amountScrolled >= label.Size.Y + b.Size.Y)
        {
            OnScrollEnded();
        }
    }

    public void OnScrollEnded()
    {
        label.Position = new Vector2(label.Position.X, label.Position.Y - (label.Size.Y - amountScrolled + b.Size.Y));
        b.Text = "Continue";
        SetScroll(false);
    }

    public override void _UnhandledInput(InputEvent @event){
    if (@event is InputEventMouseButton){
        InputEventMouseButton emb = (InputEventMouseButton)@event;
        if (emb.IsPressed()){
            if (emb.ButtonIndex == MouseButton.WheelUp){
                if (!scrollEnded) return;
                label.Position = new Vector2(label.Position.X, label.Position.Y - (scrollSpeed * (float)GetProcessDeltaTime() * 30));
                if (label.GlobalPosition.Y + label.Size.Y <= scrollContainer.GlobalPosition.Y + scrollContainer.Size.Y - b.Size.Y)
                {
                    label.GlobalPosition = new Vector2(label.GlobalPosition.X, scrollContainer.GlobalPosition.Y + scrollContainer.Size.Y - b.Size.Y - label.Size.Y);
                }
            }
            if (emb.ButtonIndex == MouseButton.WheelDown){
                if (!scrollEnded) return;
                label.Position = new Vector2(label.Position.X, label.Position.Y + (scrollSpeed * (float)GetProcessDeltaTime() * 30));
                if (label.GlobalPosition.Y >= Mathf.Max(scrollContainer.GlobalPosition.Y, b.GlobalPosition.Y - label.Size.Y))
                {
                    label.Position = new Vector2(label.Position.X, label.Position.Y - (scrollSpeed * (float)GetProcessDeltaTime() * 30));
                }
            }
        }
    }
}
}
