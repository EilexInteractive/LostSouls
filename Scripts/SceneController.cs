using Godot;
using System;

public class SceneController : Node2D
{
    [Export] protected int _RoomLevel;                    // What level of difficulty this level is
    
    public virtual void SaveRoom()
    {
        
    }

    public virtual void LoadRoomData()
    {
        
    }

    public int GetRoomLevel() => _RoomLevel;
}
