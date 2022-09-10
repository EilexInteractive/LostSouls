using Godot;
using System;

public class SceneController : Node2D
{
    [Export] protected int _RoomLevel;                    // What level of difficulty this level is
    protected RoomData _SaveData;                           // Reference to the save data of the room
    [Export] protected string _RoomName;
    public string RoomName { get => _RoomName; }

    [Export] protected int _MinEnemySpawn;                // Minimum amount of enemies that will spawn
    [Export] protected int _MaxEnemySpawn;                // Maximum amount of enemies that will spawn
    protected int _NumOfEnemies;
    
    
    [Export] protected PackedScene EnemyPrefab;
    
    public override void _Ready()
    {
        base._Ready();


        // If we are loading a new game
        if(GetNode<GameController>("/root/GameController").IsLoadedGame)
        {
            var gc = GetNode<GameController>("/root/GameController");
            GetNode<PlayerController>("/root/Main/Player").SetOwningCharacter(gc.LoadGameData.Player);
            GetNode<PlayerController>("/root/Main/Player").GetOwningCharacter().SetOwningController(GetNode<PlayerController>("/root/Main/Player"));
            GetNode<PlayerController>("/root/Main/Player").Position = gc.LoadGameData.Position;
        }        


        // Setup the players weapon
        var playerController = GetNode<GameController>("/root/GameController").GetPlayerCharacter().GetController();
        playerController.SetEquippedItem(GetNode<GameController>("/root/GameController").GetPlayerCharacter().GetInventory().GetEquippedWeapon());
        
        // Get all the spawn points
        var spawnPoints = GetTree().GetNodesInGroup("SpawnPoint");
        // Generate a random number of enemies
        _NumOfEnemies = (int)GD.RandRange(_MinEnemySpawn, _MaxEnemySpawn);
        for (int i = 0; i < _NumOfEnemies; ++i)
        {
            int loopCount = 0;
            while (true)
            {
                // Prevent infinity loop
                if (loopCount >= 200)
                    break;
                
                // Get a random spawn point
                var point = spawnPoints[(int)GD.RandRange(0, spawnPoints.Count)];
                var pointController = point as SpawnPointController;
                // Check the point is valid and not been used already
                if (pointController != null && !pointController.HasBeenUsed)
                {
                    // Create an instance of the enemy and load spawn it
                    var instance = EnemyPrefab.Instance();
                    AddChild(instance);

                    // Get the instance as the enemy controller & set the position to the spawn point
                    var enemy = instance as EnemyController;
                    if (enemy != null)
                    {
                        enemy.Name = "Enemy_" + (i + 1);
                        enemy.Position = pointController.Position;
                        enemy.CanMove = true;
                    }

                    pointController.HasBeenUsed = true;             // Set the point to in use
                    
                    
                    loopCount = 0;              // Reset the loop count
                    break;                      // Leave the while loop
                }
                else
                {
                    loopCount++;
                }
            }
        }
    }
    
    public virtual void SaveRoom()
    {
        
    }

    public virtual void LoadRoomData()
    {
        GameController gc = GetNode<GameController>("/root/GameController");
        _SaveData = gc.LoadRoomData(_RoomName);                // Load this room
    }

    public int GetRoomLevel() => _RoomLevel;
}
