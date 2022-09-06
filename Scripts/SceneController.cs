using Godot;
using System;

public class SceneController : Node2D
{
    [Export] protected int _RoomLevel;                    // What level of difficulty this level is
    protected RoomData _SaveData;                           // Reference to the save data of the room

    [Export] protected int _MinEnemySpawn;                // Minimum amount of enemies that will spawn
    [Export] protected int _MaxEnemySpawn;                // Maximum amount of enemies that will spawn
    
    public virtual void SaveRoom()
    {
        
    }

    public virtual void LoadRoomData()
    {
        GameController gc = GetNode<GameController>("/root/GameController");
        _SaveData = gc?.LoadRoomData("Main");
    }

    public int GetRoomLevel() => _RoomLevel;
}
