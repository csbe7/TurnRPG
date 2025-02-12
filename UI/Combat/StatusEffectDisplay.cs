using Godot;
using System;

public partial class StatusEffectDisplay : Control
{
    Button button;
    Label durationLabel;
    public StatusEffect effect;


    public override void _Ready()
    {
        button = GetNode<Button>("%Button");
        durationLabel = GetNode<Label>("%Duration Label");
    }

    public void SetEffect(StatusEffect se)
    {
        effect = se;
        button.Icon = se.Icon;
        OnDurationChanged(se);
        se.DurationChanged += OnDurationChanged;
        se.EffectEnded += Free;

    }

    public void OnDurationChanged(StatusEffect se)
    {
        GD.Print("Duration Changed");
        if (se.duration == 0) this.Hide();
        durationLabel.Text = se.durationLeft.ToString();
    }

    public void Free(StatusEffect e)
    {
        effect.EffectEnded -= Free;
        effect.DurationChanged -= OnDurationChanged;
        QueueFree();
    }
}
