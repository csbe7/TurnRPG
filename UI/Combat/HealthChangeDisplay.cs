using Godot;
using System;

public partial class HealthChangeDisplay : Label
{   
    [Export] Color positiveColor;
    [Export] Color negativeColor;
    [Export] Color neutralColor;

    AnimationPlayer ap;

    public override void _Ready()
    {
        ap = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public void Start(float val, bool down)
    {
        Text = Mathf.Abs(val).ToString();
        Color col;
        if (val < 0)
        {
            col = negativeColor;
        }
        else if (val > 0)
        {
            col = positiveColor;
        }
        else
        {
            col = neutralColor;
        }


        if (down)
        {
            Animation anim  = ap.GetAnimation("Float Down");
            int t_idx = anim.FindTrack(".:modulate", Animation.TrackType.Value);
            
            int k_idx = anim.TrackFindKey(t_idx, 0);
            anim.TrackSetKeyValue(t_idx, k_idx, col);
            k_idx = anim.TrackFindKey(t_idx, anim.Length);
            anim.TrackSetKeyValue(t_idx, k_idx, new Color(col.R, col.G, col.B, 0));
            ap.Play("Float Down");
            ap.AnimationFinished += Free;
        }
        else
        {
            Animation anim  = ap.GetAnimation("Float Up");
            int t_idx = anim.FindTrack(".:modulate", Animation.TrackType.Value);
            
            int k_idx = anim.TrackFindKey(t_idx, 0);
            anim.TrackSetKeyValue(t_idx, k_idx, col);
            k_idx = anim.TrackFindKey(t_idx, anim.Length);
            anim.TrackSetKeyValue(t_idx, k_idx, new Color(col.R, col.G, col.B, 0));
            ap.Play("Float Up");
            ap.AnimationFinished += Free;
        }
    }

    private void Free(StringName animName)
    {
        ap.AnimationFinished -= Free;
        QueueFree();
    }

}
