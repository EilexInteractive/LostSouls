using Godot;
using System;
using System.Collections.Generic;

[Serializable]
public abstract class RoomData
{
    public string RoomName;
    public List<Character> CharactersInRoom = new List<Character>();

    public RoomData(string roomName, List<Character> characters)
    {
        RoomName = roomName;
        CharactersInRoom = characters;
    }
}

[Serializable]
public class SaveGame
{
    public List<RoomData> RoomData = new List<RoomData>();
    private Character _Player;
    private string _CurrentRoomName = "";
}