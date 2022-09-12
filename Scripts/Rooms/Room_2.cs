using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;

public class Room_2 : SceneController
{
    public override void _Ready()
    {
        base._Ready();

        var player = GetNode<GameController>("/root/GameController").GetPlayerCharacter().GetController() as PlayerController;
        player.GetUI().SetMessage("Man there's so many demons in here. This is gonna be a challenge...");
    }

    public override void SaveRoom()
    {
        base.SaveRoom();

        Room_2_SaveData saveData = new Room_2_SaveData("Room_2", GetCharactersSaveData());
        saveData.RoomChest = GetLootChestSaveData();

    }

    public override void LoadRoomData()
    {
        base.LoadRoomData();
    }
}

[Serializable]
public class Room_2_SaveData : RoomData
{
    
    public Room_2_SaveData(string roomName, List<CharacterSaveData> characters) : base(roomName, characters)
    {
    }
}
