using Godot;
using System;
using System.Collections.Generic;



public class Room_1 : SceneController
{
    
    
    private PackedScene _EnemyCharacter;                // Reference to the enemy character
    
    private bool _AllEnemiesDead = false;               // if all the enemies have died
    private RoomData _SaveData;
    
    
    // === UNLOCK DOOR === //
    [Export] private Vector2 _DoorPosition;             // Position of the door
    [Export] private string _TileMapName;               // TileMap name that the door is on
    [Export] private int _NewTile;                      // Tile to replace the for with
    
    

    public override void _Ready()
    {
        base._Ready();
        
        LoadRoomData();
        
        GetNode<LootChest>("LootChest").OnChestOpen += OpenChest;
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
        var enemies = GetTree().GetNodesInGroup("Enemy");
        List<CharacterSaveData> characters = new List<CharacterSaveData>();

        for(int i = 0; i < enemies.Count; ++i)
        {
            var enemyNode = enemies[i] as CharacterController;
            var enemyChar = enemyNode.GetOwningCharacter();

            CharacterSaveData saveData = new CharacterSaveData(enemyChar, enemyNode.Position);
            characters.Add(saveData);
        }

        

        // Save the room data
        Room_1_SaveData save = new Room_1_SaveData("Room_1", characters);

        if(GetNode<LootChest>("LootChest").IsOpen)
        {
            save.ChestOpen = true;
            save.InteractionComplete = true;  
        }

        save.HasMovedForward = GetNode<GameController>("/root/GameController").MovingForward;

        
        GetNode<GameController>("/root/GameController").SaveRoomData(save);

    }

    public override void LoadRoomData()
    {
        base.LoadRoomData();

        GameController gc = GetNode<GameController>("/root/GameController");
        _SaveData = gc.LoadRoomData(_RoomName);                // Load this room
        
        if (_SaveData != null)
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

            var roomSave = _SaveData as Room_1_SaveData;

            if(roomSave.InteractionComplete)
            {
                GetNode<Friendly>("Friendly").DisableInteractionPrompt();
                Vector2 safeDoorPos = new Vector2(2, 16);
                GetNode<TileMap>("Navigation2D/Ground").SetCellv(safeDoorPos, 54);
                UnlockDoor();
            }

            if(roomSave.ChestOpen)
            {
                GetNode<LootChest>("LootChest").AlreadyOpen();
            }

           if(gc.IsLoadedGame)
           {
                GetNode<PlayerController>("Player").Position = gc.Save.PlayerPosition;
                gc.IsLoadedGame = false;
           } else if(roomSave.HasMovedForward)
           {
                GetNode<PlayerController>("Player").Position = GetNode<Node2D>("ReturnPoint").Position;
           }

        }

    }
}

[Serializable]
public class Room_1_SaveData : RoomData
{
    public bool InteractionComplete;
    public bool ChestOpen;
    public bool HasMovedForward;                // If the character has moved foward into the room
    public bool IsSafeOpen;
    public Room_1_SaveData(string roomName, List<CharacterSaveData> characters) : base("Room_1", characters)
    {
        
    }
}
