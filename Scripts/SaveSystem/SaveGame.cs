using Godot;
using System;
using System.Collections.Generic;

[Serializable]
public abstract class RoomData
{
    public string RoomName;
    public List<CharacterSaveData> CharactersInRoom = new List<CharacterSaveData>();

    public RoomData(string roomName, List<CharacterSaveData> characters)
    {
        RoomName = roomName;
        CharactersInRoom = characters;
    }
}

[Serializable]
public class Save
{
    public List<RoomData> RoomData = new List<RoomData>();
    public Character _Player;
    public Vector2 PlayerPosition;
    public string _CurrentRoomName = "";
}


[Serializable]
public class CharacterSaveData
{
    public Character CharacterRef;
    public Vector2 Position;

    public CharacterSaveData(Character ch, Vector2 pos)
    {
        CharacterRef = ch;
        Position = pos;
    }
}