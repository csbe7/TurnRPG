using Godot;
using System;

public partial class StatusEffectDisplay : Control
{
    Button button;
    public StatusEffect effect;

    public override void _Ready()
    {
        button = GetNode<Button>("%Button");
    }

    public void SetEffect(StatusEffect se)
    {
        effect = se;
        button.Icon = se.Icon;
        se.EffectEnded += Free;
    }

    public void Free(StatusEffect e)
    {
        effect.EffectEnded -= Free;
        QueueFree();
    }
}
