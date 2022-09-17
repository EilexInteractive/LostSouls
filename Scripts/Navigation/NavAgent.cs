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

    private CharacterController _Owner;                       // Owner of the navigation agent
    private float _AgentMovementSpeed;

    public override void _Ready()
    {
        base._Ready();

        _NavMap = GetNode<Navigation>("/root/Main/Navigation2D/Ground");            // Get reference to the navgiation system

        
    }

    public override void _Process(float delta)
    {
        base._PhysicsProcess(delta);

        if(_HasPath && _Owner != null && _Owner.CanMove)
        {
            if(_Path.Count > 2)
            {
                float distanceToPathPoint = this.GlobalPosition.DistanceTo(_NextTilePos);               // Get the distance to the next path point
                // Move the object
                _Owner.MoveAndCollide(GetMovementDirection() * (_AgentMovementSpeed * delta));

                // Check if we have reached the next path point, if so update the path point
                if(distanceToPathPoint < 12)
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
        if(_NavMap == null && _Owner != null && _Owner.CanMove)
            return;

        
        _Path = _NavMap.GetPath(this.GlobalPosition, PathTo);                 // Get the path from the navigation system.
        if(_Path != null)
        {
            _HasPath = true;
            if(_Path.Count > 2)
            {
                _CurrentTilePos = _Path[0];
                _NextTilePos = _Path[1];
            }
        }
        
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

    public void SetOwner(CharacterController owner) => _Owner = owner;
    public void SetMovementSpeed(float speed) => _AgentMovementSpeed = speed;
}