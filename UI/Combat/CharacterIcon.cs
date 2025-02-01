using Godot;
using System;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;

public partial class CharacterIcon : Control
{
    [Export] public Sheet sheet;
    [Export] PackedScene hpChangedDisplay;
    [Export] PackedScene statusEffectDisplay;

    [Export] public Control topToAttachment, topFromAttachment, bottomToAttachment, bottomFromAttachment;

    AnimationPlayer ap;

    public bool isEnemy = false;

    Label characterName;
    CounterBar healthBar, energyBar;

    Sprite2D actionIcon;

    Control hpDisplayPos;

    public ColorRect effectDisplay;
    SkillEffect currEffect;
    float effectProgress;

    GridContainer effectDisplayGrid;

    public Button select;

    bool spent;
    public bool Spent{
        get{
            return spent;
        }
        set{
            spent = value;
            if (spent || sheet.statBlock.Dead) actionIcon.Hide();
            else actionIcon.Show();
        }
    } //has this character already been used this round?

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
        actionIcon = GetNode<Sprite2D>("%Action Icon");

        
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
        hpchange.Add(newVal - oldVal);

        GD.Print("Health Changed");
        if (!displayingHealthChange) DisplayHealthChange();
        
        if (sheet.statBlock.Dead) actionIcon.Hide();
    }

    void OnEnergyChanged(float oldVal, float newVal)
    {
        energyBar.SetValue(sheet.statBlock.CurrEnergy.ModValue);
    }

    Godot.Collections.Array<float> hpchange = new Godot.Collections.Array<float>();
    bool displayingHealthChange = false;
    async void DisplayHealthChange()
    {   
        displayingHealthChange = true;
        foreach(float change in hpchange)
        {
            GD.Print(hpchange.Count);
            var display = hpChangedDisplay.Instantiate<HealthChangeDisplay>();
            AddChild(display);
            display.Position = hpDisplayPos.Position;
            display.Start(change, isEnemy);
            await Task.Delay(500);
        }

        hpchange.Clear();
        displayingHealthChange = false;
    }

    public void Select()
    {
        if (!isEnemy) ap.Play("Up");
        else ap.Play("Down");
    }
    public void Deselect()
    {
        if (!isEnemy) ap.PlayBackwards("Up");
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
