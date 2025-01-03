using Godot;
using System;

public partial class ExplorationManager : Control
{
    [Export] PackedScene explorationSquare;
    [Export] Vector2I explorationGridMaxSize = new Vector2I(15, 15);
    [Export] Vector2I explorationGridMinSize = new Vector2I(6, 6);
    [Export] float squareSpacing;

    [ExportCategory("Icons")] 
    [Export] PackedScene playerIcon;
    [Export] Texture2D enterDoorTexture;
    

    [Export] EventTable eventTable;

    [ExportCategory("Interfaces")]
    [Export] PackedScene dialogueEventInterface;

    Vector2I currGridSize;
    Godot.Collections.Array<Godot.Collections.Array<ExplorationSquare>> grid  = new Godot.Collections.Array<Godot.Collections.Array<ExplorationSquare>>();

    Control gridTopLeft;

    PlayerIconExploration pi;

    RandomNumberGenerator rng = new RandomNumberGenerator();

    CombatManager cm;

    public override void _Ready()
    {
        cm = GetTree().Root.GetNode<CombatManager>("CombatManager");
        gridTopLeft = GetNode<Control>("%Grid Top Right");
        MakeGrid();
        PlacePlayer();
        PlaceExit();
    }

    void MakeGrid()
    {
        rng.Randomize();
        Vector2I gridSize = new Vector2I(rng.RandiRange(explorationGridMinSize.X, explorationGridMaxSize.X), rng.RandiRange(explorationGridMinSize.Y, explorationGridMaxSize.Y));
        currGridSize = gridSize;
        int i, j;


        Vector2 screenSize =  GetViewport().GetVisibleRect().Size;
        Vector2 screenCenter = screenSize/2;
        gridTopLeft.Position = new Vector2(screenCenter.X - (gridSize.X*squareSpacing/2), screenCenter.Y - (gridSize.Y*squareSpacing/2));

        foreach(Godot.Collections.Array row in grid)
        {
            row.Clear();
        }
        grid.Clear();

        for(i = 0; i < gridSize.X; i++)
        {
            grid.Insert(i, new Godot.Collections.Array<ExplorationSquare>());
            for(j = 0; j < gridSize.Y; j++)
            {
                ExplorationSquare es = explorationSquare.Instantiate<ExplorationSquare>();
                es.gridPos = new Vector2I(i, j);
                es.ButtonClicked += OnSquareClicked;
                gridTopLeft.AddChild(es);
                es.Position = new Vector2((squareSpacing*i), (squareSpacing*j));
                var e = eventTable.PickEvent();
                if (IsInstanceValid(e)) es.SetEvent(e);
                grid[i].Insert(j, es);
                es.SetIconVisibility(false);
            }
        }

       /* for(i = 0; i < gridSize.X; i++)
        {
            for(j = 0; j < gridSize.Y; j++)
            {
                int nconnections = 0;
                if (GetSquare(new Vector2I(i+1, j)) != null) nconnections++; 
                if (GetSquare(new Vector2I(i-1, j)) != null) nconnections++; 
                if (GetSquare(new Vector2I(i, j+1)) != null) nconnections++; 
                if (GetSquare(new Vector2I(i, j-1)) != null) nconnections++; 

                if (nconnections == 4)
                {
                    if (rng.RandiRange(0, 1) == 1)
                    {
                        var s = GetSquare(new Vector2I(i, j));
                        s.ButtonClicked -= OnSquareClicked;
                        s.QueueFree();
                    } 
                }
                if (nconnections == 3)
                {
                    if (rng.RandiRange(0, 2) == 1)
                    {
                        var s = GetSquare(new Vector2I(i, j));
                        s.ButtonClicked -= OnSquareClicked;
                        s.QueueFree();
                    } 
                }
            }
        }*/

    }

    void PlacePlayer()
    {
        int x = rng.RandiRange(0, grid.Count-1);
        int y = rng.RandiRange(0, grid[x].Count-1);
        Vector2I pos = new Vector2I(x,y);

        pi = playerIcon.Instantiate<PlayerIconExploration>();
        gridTopLeft.AddChild(pi);
        
        Vector2 squarePos = grid[pos.X][pos.Y].Position;
        Vector2 squareSize = grid[pos.X][pos.Y].Size;
        pi.Position = new Vector2(squarePos.X + squareSize.X/2, squarePos.Y + squareSize.Y/2);
        pi.currSquare = (Vector2I)pos;

        grid[pos.X][pos.Y].SetIcon(enterDoorTexture); 
        grid[pos.X][pos.Y].SetIconVisibility(false);

        grid[pos.X][pos.Y].explored = true;

        pi.Arrived += PlayerArrived;
    }

    void PlaceExit()
    {
        while(true)
        {
            int x = rng.RandiRange(0, grid.Count-1);
            int y = rng.RandiRange(0, grid[x].Count-1);
            Vector2I pos = new Vector2I(x,y);

            if (pos == pi.currSquare) continue;

            var es = GetSquare(pos);
            if (es != null)
            {
                es.SetEvent(eventTable.exitEvent);
                es.SetIconVisibility(true);
                break;   
            }
        }

    }

    async void OnSquareClicked(ExplorationSquare square)
    {
        Vector2I displacement = pi.currSquare - square.gridPos;
        if (displacement == new Vector2I(1, 0) || displacement == new Vector2I(0, 1) || displacement == new Vector2I(0, -1) || displacement == new Vector2I(-1, 0))
        {
            square.SetIconVisibility(true);
            if (!square.explored){
                if (IsInstanceValid(square.squareEvent))
                {
                    if (square.squareEvent is Encounter encounter) StartEncounter(encounter);
                    else if (square.squareEvent is DialogueEvent dialogueEvent) StartDialogueEvent(dialogueEvent);
                    else if (square.squareEvent.name == "Exit") ExitReached();
                }
            }
            
            if (IsInstanceValid(square.squareEvent)) await ToSignal(square.squareEvent, Event.SignalName.EventResolved);

            Vector2 squarePos = square.Position;
            Vector2 squareSize = square.Size;
            pi.Move(new Vector2(squarePos.X + squareSize.X/2, squarePos.Y + squareSize.Y/2), square.gridPos);

            square.explored = true;
        }
    }
    

    BattleInterface bi;
    void StartEncounter(Encounter encounter)
    {
        cm.BattleStart(PlayerInfo.current_party, encounter);
    }

    void StartDialogueEvent(DialogueEvent dialogueEvent)
    {
        DialogueEventInterface dei = dialogueEventInterface.Instantiate<DialogueEventInterface>();
        dei.dialogueEvent = dialogueEvent;
        GetParent().AddChild(dei);
    }

    ExplorationSquare GetSquare(Vector2I pos)
    {
        if (pos.X < 0 || pos.X > grid.Count || pos.Y < 0 || pos.Y > grid[pos.X].Count) return null;

        try{
            if (IsInstanceValid(grid[pos.X][pos.Y])) return grid[pos.X][pos.Y];
            else return null;
        }
        catch (IndexOutOfRangeException)
        {
            return null;
        }
        
    }

    void PlayerArrived(Vector2I startPos, Vector2I endPos)
    {
        pi.currSquare = endPos;
        GetSquare(pi.currSquare).SetIconVisibility(false);
        GetSquare(startPos).SetIconVisibility(true);
        
    }

    public void ExitReached()
    {

        QueueFree();
    }
}
