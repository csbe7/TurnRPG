using Godot;
using System;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;

public partial class CharacterIcon : Control
{
    [Export] public Sheet sheet;
    [Export] PackedScene hpChangedDisplay;
    [Export] PackedScene statusEffectDisplay;

    AnimationPlayer ap;

    public bool ai = false;

    Label characterName;
    CounterBar healthBar, energyBar;

    Control hpDisplayPos;

    public ColorRect effectDisplay;
    SkillEffect currEffect;
    float effectProgress;

    GridContainer effectDisplayGrid;

    public Button select;

    public bool spent; //has this character already been used this round?

    [Signal] public delegate void SelectedEventHandler(CharacterIcon self);

    public override void _Ready()
    {
        if (!IsInstanceValid(sheet))
        {
            GD.PrintErr("SHEET NOT SET!!!!! KILL YUOSELF");
            return;
        }

        sheet = (Sheet)sheet.Duplicate();
        sheet.statBlock = (StatBlock)sheet.statBlock.Clone();
        sheet.statusEffects = new Node();
        sheet.statusEffects.Name = "Status Effects";
        sheet.skillParent = new Node();
        sheet.skillParent.Name = "Skills";
        AddChild(sheet.statusEffects);
        AddChild(sheet.skillParent);

        //LOAD SKILLS
        PackedScene r = (PackedScene)ResourceLoader.Load("res://RPG system/Skills/attack.tscn");
		Skill rs = r.Instantiate<Skill>();
		sheet.skillParent.AddChild(rs);
		sheet.skills.Add(rs);
		rs.user = this;
        r = (PackedScene)ResourceLoader.Load("res://RPG system/Skills/defend.tscn");
        rs = r.Instantiate<Skill>();
		sheet.skillParent.AddChild(rs);
		sheet.skills.Add(rs);
		rs.user = this;
        r = (PackedScene)ResourceLoader.Load("res://RPG system/Skills/rest.tscn");
        rs = r.Instantiate<Skill>();
		sheet.skillParent.AddChild(rs);
		sheet.skills.Add(rs);
		rs.user = this;
		foreach(PackedScene skillScene in sheet.packedSkills)
		{
			Skill s = skillScene.Instantiate<Skill>();
			sheet.skillParent.AddChild(s);
			sheet.skills.Add(s);
			s.user = this;
		}

        sheet.Ready();
        
        characterName = GetNode<Label>("%Character Name");
        healthBar = GetNode<CounterBar>("%Health Bar");
        energyBar = GetNode<CounterBar>("%Energy Bar");
        effectDisplay = GetNode<ColorRect>("%Effect");
        ap = GetNode<AnimationPlayer>("AnimationPlayer");
        hpDisplayPos = GetNode<Control>("HP Display Position");
        effectDisplayGrid = GetNode<GridContainer>("%Effect Display Container");

        
        select = GetNode<Button>("%Button");

        characterName.Text = sheet.name;
        healthBar.MaxValue = sheet.statBlock.Health.ModValue;
        energyBar.MaxValue = sheet.statBlock.Energy.ModValue;
        healthBar.Value = sheet.statBlock.CurrHealth.ModValue;
        energyBar.Value = sheet.statBlock.CurrEnergy.ModValue;

        select.ButtonDown += OnButtonDown;

        sheet.statBlock.CurrHealth.ValueChanged += OnHealthChanged;
        sheet.statBlock.CurrEnergy.ValueChanged += OnEnergyChanged;
        sheet.StatusEffectAdded += OnStatusEffectAdded;

        SetPhysicsProcess(false);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsInstanceValid(currEffect))
        {
            effectProgress += (float)delta;
            float sample = currEffect.fadeCurve.Sample(effectProgress);
            effectDisplay.Color = new Color(currEffect.color.R, currEffect.color.G, currEffect.color.B, sample);
            if (effectProgress > currEffect.fadeCurve.GetPointPosition(currEffect.fadeCurve.PointCount-1).X)
            {
                currEffect = null;
                SetPhysicsProcess(false);
            }
        }
    }

    private void OnButtonDown()
    {
        EmitSignal(SignalName.Selected, this);
    }

    void OnHealthChanged(float oldVal, float newVal)
    {
        healthBar.SetValue(sheet.statBlock.CurrHealth.ModValue);
        var display = hpChangedDisplay.Instantiate<HealthChangeDisplay>();
        AddChild(display);
        display.Position = hpDisplayPos.Position;
        display.Start(newVal - oldVal, ai);
    }

    void OnEnergyChanged(float oldVal, float newVal)
    {
        energyBar.SetValue(sheet.statBlock.CurrEnergy.ModValue);
    }

    public void Select()
    {
        if (!ai) ap.Play("Up");
        else ap.Play("Down");
    }
    public void Deselect()
    {
        if (!ai) ap.PlayBackwards("Up");
        else ap.PlayBackwards("Down");
    }

    public void PlayEffect(SkillEffect effect)
    {
        currEffect = effect;
        effectProgress = 0;
        SetPhysicsProcess(true);
    }

    void OnStatusEffectAdded(StatusEffect effect)
    {  
        if (effect.hidden) return;
        GD.Print("Effect Added");
        StatusEffectDisplay seDisplay = statusEffectDisplay.Instantiate<StatusEffectDisplay>();
        effectDisplayGrid.AddChild(seDisplay);
        seDisplay.SetEffect(effect);
    }
}
