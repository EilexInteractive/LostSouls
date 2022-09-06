using Godot;
using System;
using System.Collections.Generic;

public class Room_2 : SceneController
{
    [Export] private PackedScene EnemyPrefab;
    private List<Vector2> _SpawnPoints = new List<Vector2>();
    private int _NumOfEnemies = 0;

    public override void _Ready()
    {
        base._Ready();

        _NumOfEnemies = (int)GD.RandRange(4, 10);
        var spawnPoints = GetTree().GetNodesInGroup("SpawnPoint");
        for (int i = 0; i < _NumOfEnemies; ++i)
        {
            var enemyInstance = EnemyPrefab.Instance();
            AddChild(enemyInstance);

            var enemyController = enemyInstance as EnemyController;
            if (enemyController != null)
            {
                var spawnNode = spawnPoints[(int)GD.RandRange(0, spawnPoints.Count - 1)] as Node2D;
                enemyController.Position = spawnNode.GlobalPosition;
                enemyController.CanMove = true;
            }
        }
    }
}
