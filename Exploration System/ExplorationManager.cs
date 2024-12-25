using Godot;
using System;

public partial class ExplorationManager : Control
{
    [Export] PackedScene explorationSquare;
    [Export] PackedScene playerIcon;
    [Export] Vector2I explorationGridMaxSize = new Vector2I(15, 15);
    [Export] Vector2I explorationGridMinSize = new Vector2I(6, 6);
    [Export] float squareSpacing;

    Vector2I currGridSize;
    Godot.Collections.Array<Godot.Collections.Array<ExplorationSquare>> grid  = new Godot.Collections.Array<Godot.Collections.Array<ExplorationSquare>>();

    Control gridTopLeft;

    RandomNumberGenerator rng = new RandomNumberGenerator();

    public override void _Ready()
    {
        gridTopLeft = GetNode<Control>("%Grid Top Right");

        MakeGrid();
        PlacePlayer();
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
                grid[i].Insert(j, es);
            }
        }

    }

    void PlacePlayer()
    {
        Vector2I pos = new Vector2I(rng.RandiRange(0, currGridSize.X-1), rng.RandiRange(0, currGridSize.Y-1));
        Control icon = playerIcon.Instantiate<Control>();
        gridTopLeft.AddChild(icon);
        
        Vector2 squarePos = grid[pos.X][pos.Y].Position;
        Vector2 squareSize = grid[pos.X][pos.Y].GetMinimumSize();
        icon.Position = new Vector2(squarePos.X + squareSize.X/2, squarePos.Y + squareSize.Y/2);
        GD.Print(pos);
        GD.Print(squarePos, squareSize);
        GD.Print(icon.Position);
    }

    void OnSquareClicked(ExplorationSquare square)
    {

    }

    
}
