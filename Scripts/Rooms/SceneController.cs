using Godot;
using System;
using System.Collections.Generic;

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


        // If we are loading a game
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
        
        // If we are loading a room than don't spawn enemies
        if(GetNode<GameController>("/root/GameController").IsLoadedGame)
            return;


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
        // Already have implemented custom save scripts for these levels
        if(_RoomName == "Main" || _RoomName == "Room_1")
            return;



    }

    public virtual void LoadRoomData()
    {
        GameController gc = GetNode<GameController>("/root/GameController");
        _SaveData = gc.LoadRoomData(_RoomName);                // Load this room

        // Already have implemented custom save scripts for these levels
        if(_RoomName == "Main" || _RoomName == "Room_1")
            return;

        LoadEnemyCharacters();
        LoadLootChest();

        
    }

    protected List<ChestSave> GetLootChestSaveData()
    {
        var lootChest = GetTree().GetNodesInGroup("LootChest");
        List<ChestSave> lootSaveData = new List<ChestSave>();

        foreach(var chest in lootChest)
        {
            ChestSave chestSave = new ChestSave();
            var chestNode = chest as LootChest;
            chestSave.IsOpen = chestNode.IsOpen;
            lootSaveData.Add(chestSave);
        }

        return lootSaveData;
    }

    protected void LoadLootChest()
    {
        // Get all the chest data
        var lootChest = GetTree().GetNodesInGroup("LootChest");

        // check that there are chest in the room that we need to check
        if(lootChest.Count > 0)
        {
            int count = 0;                  // Stores reference to the current chest count
            foreach(var chest in lootChest)
            {
                // Get the chest node
                var chestNode = chest as LootChest;
                if(chest != null)
                {
                    // Set the status & if required show chest as opem
                    chestNode.IsOpen = _SaveData.RoomChest[count].IsOpen;
                    if(chestNode.IsOpen == true)
                        chestNode.AlreadyOpen();
                }

                count++;            // Increment the count
            }
        }
    }

    protected List<CharacterSaveData> GetCharactersSaveData()
    {
        var enemies = GetTree().GetNodesInGroup("Enemy");
        List<CharacterSaveData> characters = new List<CharacterSaveData>();

        for(int i = 0; i < enemies.Count; ++i)
        {
            var enemyNode = enemies[i] as CharacterController;
            var enemyChar = enemyNode.GetOwningCharacter();

            CharacterSaveData saveData = new CharacterSaveData(enemyChar, enemyNode.Position);
            characters.Add(saveData);
        }

        return characters;
    }

    protected void LoadEnemyCharacters()
    {
        var enemies = GetTree().GetNodesInGroup("Enemy");
        for(int i = 0; i < enemies.Count; ++i)
        {
            CharacterSaveData loadData = _SaveData.CharactersInRoom[i];
            if(loadData != null)
            {
                var enemyNode = enemies[i] as EnemyController;                  // Gets the request enemy node
                loadData.CharacterRef.SetOwningController(enemyNode);           // Set the controller to the new enemy
                enemyNode.SetOwningCharacter(loadData.CharacterRef);                // Sets the owner
                enemyNode.Position = loadData.Position;                 // Sets the position of the enemy
                enemyNode.AlreadyDead();                                        // Checks if the enemy has already died
            }
        }
    }

    public int GetRoomLevel() => _RoomLevel;
}
