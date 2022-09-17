using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Navigation : TileMap
{
    [Export] private List<int> _WalkableTileIDs = new List<int>();
    private List<Tile> _WalkableTiles = new List<Tile>();

    public override void _Ready()
    {
        base._Ready();

        // Get reference to all the walkable tiles in the scene
        foreach(var tile in _WalkableTileIDs)
        {
            var cells = GetUsedCellsById(tile);
            foreach(var cell in cells)
            {
                var value = cell.ToString();
                value = value.Replace("(", "");
                value = value.Replace(")", "");
                var vString = value.Split(',');

                // Create the x & y value
                int xValue = int.Parse(vString[0]);
                int yValue = int.Parse(vString[1]);

                // Add tile
                _WalkableTiles.Add(new Tile(new Vector2(xValue, yValue), true));
            }
        }
    }

    public List<Vector2> GetPath(Vector2 startPos, Vector2 endPos)
    {
        var finalPath = new List<Vector2>();

        Tile current = null;

        // Get the tilemap co--ordinates for the start & finish position
        var startTile = WorldToMap(startPos);
        var endTile = WorldToMap(endPos);

        var openList = new List<Tile>();
        var closedList = new List<Tile>();
        int g = 0;

        var startingTile = new Tile(startTile);
        openList.Add(startingTile);

        while(openList.Count > 0)
        {
            // Get the tile with the lowest F score
            var lowest = openList.Min(t => t.F);
            // Set the current tile to the lowest score
            current = openList.First(t => t.F == lowest);

            closedList.Add(current);
            openList.Remove(current);

            // We have reached the end tile so trace back to find the final path
            if(closedList.FirstOrDefault(t => t.TilePos.x == endTile.x && t.TilePos.y == endTile.y) != null)
            {
                var path = new List<Vector2>();
                path.Insert(0, MapToWorld(current.TilePos));
                while(current != null)
                {
                    current = current.Parent;
                    if(current == null)
                        break;
                    
                    path.Insert(0, MapToWorld(current.TilePos));

                }
                path[0] = MapToWorld(startingTile.TilePos);
                return path;
            }

            var walkableTiles = GetWalkableTiles(current);
            g++;

            foreach(var tile in walkableTiles)
            {
                // Tile is already in the closed list (has been checked)
                if(closedList.FirstOrDefault(t => t.TilePos == tile.TilePos) != null)
                    continue;

                // Not in the open list so setup the tile score 
                if(openList.FirstOrDefault(t => t.TilePos == tile.TilePos) == null)
                {
                    var tilePos = MapToWorld(tile.TilePos);             // Get the world position of the tile

                    // Calcaulte the score
                    tile.G = g;
                    tile.H = CalculateHScore(tilePos, endPos);
                    tile.F = tile.G + tile.H;
                    
                    // Setup parent & Add to the open list
                    tile.Parent = current;
                    openList.Insert(0, tile);
                } else 
                {
                    // Test if using the current G Score makes the squares F score lower
                    // if yes update the parent vbecause it means it's a better path
                    if(g + tile.H < tile.F)
                    {
                        tile.G = g;
                        tile.F = tile.G + tile.H;
                        tile.Parent = current;
                    }
                }
            }

        }

        return default;
    }

    private List<Tile> GetWalkableTiles(Tile current)
    {

        // Get tiles left, right, up, down
        var proposedTiles = new List<Tile>()
        {
            new Tile(new Vector2(current.TilePos.x, current.TilePos.y + 1)),
            new Tile(new Vector2(current.TilePos.x, current.TilePos.y - 1)),
            new Tile(new Vector2(current.TilePos.x + 1, current.TilePos.y)),
            new Tile(new Vector2(current.TilePos.x - 1, current.TilePos.y))
        };

        // Determine if the tiles are walkable.
        foreach(var t in proposedTiles)
        {
            int id = GetCell((int)t.TilePos.x, (int)t.TilePos.y);
            if(_WalkableTileIDs.Contains(id))
                t.Walkable = true;
            else
                t.Walkable = false;
        }

        return proposedTiles;
    }

    private int CalculateHScore(Vector2 tilePos, Vector2 targetPos)
    {
        return (int)Math.Abs(targetPos.x - tilePos.x) + (int)Math.Abs(targetPos.y - tilePos.y);
    }
}
