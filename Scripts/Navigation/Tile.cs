using Godot;
using System;

public class Tile
{
    public Vector2 TilePos;                 // Position of the tile
    public bool Walkable;                   // If this tile can be walked on
    public float F, G, H;                   // A* Score
    public Tile Parent;                     // Which tile lead to this

    public Tile(Vector2 pos, bool walkable = false)
    {
        TilePos = pos;
        Walkable = walkable;
    }

    public Tile(int x, int y)
    {
        TilePos.x = x;
        TilePos.y = y;
    }
}