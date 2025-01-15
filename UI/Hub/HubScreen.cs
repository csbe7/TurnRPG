using Godot;
using System;

public partial class HubScreen : Control
{
    public override void _Ready()
    {
        HubButton hb = GetNode<HubButton>("%Shop Button");
        hb.b.ButtonDown += OnShopButtonDown;

        hb = GetNode<HubButton>("%Party Button");
        hb.b.ButtonDown += OnPartyButtonDown;

        hb = GetNode<HubButton>("%Explore Button");
        hb.b.ButtonDown += OnExploreButtonDown;
    }


    void OnShopButtonDown()
    {
        
    }

    void OnPartyButtonDown()
    {
        PartyManagementInterface pmi = Game.partyManagementInterface.Instantiate<PartyManagementInterface>();
        GetParent().AddChild(pmi);
    }

    void OnExploreButtonDown()
    {
        ExplorationManager em = Game.explorationInterface.Instantiate<ExplorationManager>();
        GetParent().AddChild(em);
    }
}
