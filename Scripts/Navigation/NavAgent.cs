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

    private KinematicBody2D _Owner;                       // Owner of the navigation agent

    public override void _Ready()
    {
        base._Ready();

        _NavMap = GetNode<Navigation>("/root/Main/Navigation2D/Ground");            // Get reference to the navgiation system


    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if(_HasPath && _Owner != null)
        {
            if(_Path.Count > 0)
            {
                float distanceToPathPoint = this.GlobalPosition.DistanceTo(_NextTilePos);
                _Owner.MoveAndCollide(GetMovementDirection());
                if(distanceToPathPoint < 9)
                    ReachedPathPoint();

            }
        }
    }


    /// <summary>
    /// Creates the path to the select location
    /// </summary>
    /// <param name="PathTo">Path to move towards</param>
    public void SetPath(Vector2 PathTo)
    {
        // Check that we have reference to the navigation system
        if(_NavMap == null && _Owner != null)
            return;

        _HasPath = true;
        _Path = _NavMap.GetPath(this.Position, PathTo);                 // Get the path from the navigation system.
    }


    /// <summary>
    /// Gets the direction that the agent is moving towards
    /// </summary>
    /// <returns></returns>
    public Vector2 GetMovementDirection()
    {
        var velocity = new Vector2();

        if(_NextTilePos.x > _CurrentTilePos.x)
            velocity.x += 1;

        if(_NextTilePos.x < _CurrentTilePos.x)
            velocity.x -= 1;

        if(_NextTilePos.y > _CurrentTilePos.y)
            velocity.y += 1;

        if(_NextTilePos.y < _CurrentTilePos.y)
            velocity.y -= 1;

        return velocity;
    }

    public void ReachedPathPoint()
    {
        _CurrentTilePos = _NextTilePos;
        if(_Path.Count > 2)
        {
            _Path.RemoveAt(0);                          // We are no longer at this tile
            _NextTilePos = _Path[1];                    // Get the next tile position
        } else 
        {
            GD.Print("Location has been reached");
            _HasPath = false;
        }
    }

    public void SetOwner(KinematicBody2D owner) => _Owner = owner;
}