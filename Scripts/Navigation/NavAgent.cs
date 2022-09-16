using System;
using Godot;
using System.Collections.Generic;

public class NavAgent : Node2D
{
    private Navigation _NavMap;              // Reference to the navigation map

    private List<Vector2> _Path;          // reference to the path
    private bool _HasPath;                  // check that the nav agent has a path

    private Vector2 _CurrentTilePos;            // Reference to the tile we are currently at
    private Vector2 _NextTilePos;               // Reference to the next tile position
}