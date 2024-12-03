using Godot;
using System;

public partial class AiSkillDisplay : Control
{
    AnimationPlayer ap;
    Label label;

    public override void _Ready()
    {
        ap = GetNode<AnimationPlayer>("AnimationPlayer");
        label = GetNode<Label>("Label");
    }

    public void SetText(string t)
    {
        label.Text = t;
    }

    public void FadeIn()
    {
        ap.Play("Fade In");
    }
    public void FadeOut()
    {
        ap.PlayBackwards("Fade In");
    }
}
