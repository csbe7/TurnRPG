using Godot;
using System;

public partial class CounterBar : ProgressBar
{
    Label label;

    [Export] float changeSpeed = 3;
    public float targetVal;
    bool rising = false;

    public override void _Ready()
    {
        label = GetNode<Label>("Label");
        ValueChanged += SetText;
        SetPhysicsProcess(false);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (rising && targetVal > Value)
        {
            Value += delta * changeSpeed;
            if (Value >= targetVal)
            {
                Value = targetVal;
                SetPhysicsProcess(false);
            }
        }
        else if (!rising && targetVal < Value)
        {
            Value -= delta * changeSpeed;
            if (Value <= targetVal)
            {
                Value = targetVal;
                SetPhysicsProcess(false);
            }
        }
        else{
            Value = targetVal;
            SetPhysicsProcess(false);
        }
    }

    public void SetValue(float value)
    {
        targetVal = value;
        //GD.Print(targetVal);
        if (Value < targetVal) rising = true;
        else rising = false;
        SetPhysicsProcess(true);
    }


    public void SetText(double value)
    {
        label.Text = Value.ToString() + "/" + MaxValue.ToString();
    }
}
