using Godot;
using System;
using System.Collections.Generic;

public class Room_1 : SceneController
{
    private PackedScene _EnemyCharacter;

    public override void _Ready()
    {
        base._Ready();

        _EnemyCharacter = GD.Load <PackedScene> ("res://Prefabs/Pirate.tscn");
        

        var nodes = GetTree().GetNodesInGroup("SpawnPoint");
        for (int i = 0; i < nodes.Count; ++i)
        {
            var enemyInstance = _EnemyCharacter.Instance();             // Create a new enemy instance
            
            // Add the enemy to the scene
            AddChild(enemyInstance);
            
            // Ensure that the instance node has an enemy controller
            if (enemyInstance is EnemyController)
            {
                // Create the controller
                var enemyNode = enemyInstance as EnemyController;
                // Get the spawn node as a node so we can get the position
                var spawnNode = nodes[i] as Node2D;
                // Spawn the enemy at the node
                enemyNode.Position = spawnNode.GlobalPosition;
                enemyNode.CanMove = true;
            }
            
            // TODO: Setup the enemy with stats based on the room along with a random inventory the player can collect
        }
    }
}
