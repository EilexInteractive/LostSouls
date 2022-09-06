using Godot;
using System;
using System.Collections.Generic;



public class Room_1 : SceneController
{
    
    
    private PackedScene _EnemyCharacter;
    private bool _DialogComplete;
    
    private bool _AllEnemiesDead = false;               // if all the enemies have died
    private RoomData _SaveData;
    
    
    // === UNLOCK DOOR === //
    [Export] private Vector2 _DoorPosition;
    [Export] private string _TileMapName;
    [Export] private int _NewTile;


    private Timer ChestDialogTimer;

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

            GetNode<LootChest>("LootChest").OnChestOpen += OpenChest;

            // TODO: Setup the enemy with stats based on the room along with a random inventory the player can collect
        }
    }

    public void OpenChest()
    {
        var player =
            GetNode<GameController>("/root/GameController").GetPlayerCharacter().GetController() as PlayerController;

        if (player != null)
        {
            player.GetUI().SetMessage("** YOU FOUND A PART OF YOU! **");
            
            // Setup the new message for the friendly person
            GetNode<Friendly>("Friendly").DialogMessage =
                "See you found some of your soul. Maybe you could become the great warrior you once were...";
            GetNode<Friendly>("Friendly").EnableInteractionPrompt();
            player.SetDialogEvent(UnlockDoor);
        }
    }

    public void UnlockDoor()
    {
        GetNode<ChangeScene>("NextRoom").CanChangeScene = true;
        var tileMap = GetNode<TileMap>("Navigation2D/" + _TileMapName);
        tileMap.SetCellv(_DoorPosition, _NewTile);
    }

    public override void SaveRoom()
    {
        List<CharacterSaveData> characters = new List<CharacterSaveData>();
        var charactersInScene = GetTree().GetNodesInGroup("Enemy");
        foreach (var enemy in charactersInScene)
        {
            var saveEnemy = enemy as CharacterController;
            var character = saveEnemy.GetOwningCharacter();
            var position = saveEnemy.Position;
            characters.Add(new CharacterSaveData(character, position));
        }

        Room_1_SaveData save = new Room_1_SaveData("Room_1", characters);

    }

    public override void LoadRoomData()
    {
        base.LoadRoomData();
        if (_SaveData != null)
        {
            var roomData = _SaveData as Room_1_SaveData;
            if (roomData != null)
            {
                foreach (var characters in _SaveData.CharactersInRoom)
                {
                    var enemies = GetTree().GetNodesInGroup("Enemy");
                    int characterIndex = 0;
                    foreach (var enemy in enemies)
                    {
                        GetNode<CharacterController>("Enemy_" + characterIndex + 1).SetOwningCharacter(_SaveData.CharactersInRoom[characterIndex].CharacterRef);
                        GetNode<CharacterController>("Enemy_" + characterIndex + 1).Position =
                            _SaveData.CharactersInRoom[characterIndex].Position;
                        
                    }
                }

                GetNode<CharacterController>("Player").Position = GetNode<Node2D>("ReturnPoint").Position;
            }
        }

    }
}

public class Room_1_SaveData : RoomData
{
    public Room_1_SaveData(string roomName, List<CharacterSaveData> characters) : base(roomName, characters)
    {
        
    }
}
