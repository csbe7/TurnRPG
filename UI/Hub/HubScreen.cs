using Godot;
using System;

public partial class HubScreen : Control
{
    public Control currInterface;
    
    public override void _Ready()
    {
        Game.hub = this;
        HubButton hb = GetNode<HubButton>("%Shop Button");
        hb.b.ButtonDown += OnShopButtonDown;

        hb = GetNode<HubButton>("%Party Button");
        hb.b.ButtonDown += OnPartyButtonDown;

        hb = GetNode<HubButton>("%Explore Button");
        hb.b.ButtonDown += OnExploreButtonDown;
    }


    void OnShopButtonDown()
    {
        if (IsInstanceValid(currInterface)) currInterface.QueueFree();
        ShopInterface si = Game.shopInterface.Instantiate<ShopInterface>();
        GetParent().AddChild(si);
        currInterface = si;
    }

    void OnPartyButtonDown()
    {
        if (IsInstanceValid(currInterface)) currInterface.QueueFree();
        PartyManagementInterface pmi = Game.partyManagementInterface.Instantiate<PartyManagementInterface>();
        GetParent().AddChild(pmi);
        currInterface = pmi;
    }

    void OnExploreButtonDown()
    {
        if (IsInstanceValid(currInterface)) currInterface.QueueFree();
        PreparationInterface pi = Game.preparationInterface.Instantiate<PreparationInterface>();
        GetParent().AddChild(pi);
        currInterface = pi;

        /*ExplorationManager em = Game.explorationInterface.Instantiate<ExplorationManager>();
        GetParent().AddChild(em);*/
    }
}
