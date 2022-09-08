using Godot;
using System;

public class GameController : Node
{
    private Character _Player;
    private SaveGame Save;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CreateNewPlayer();
        Save = new SaveGame();
    }

    public void CreateNewPlayer()
    {
        _Player = new Character("Player", true);
        Weapon firstWeapon = GetNode<ItemDatabase>("/root/ItemDatabase").GetItem("Old Sword") as Weapon;
        _Player.GetInventory().AddItem(firstWeapon);
        _Player.GetInventory().EquipWeapon(firstWeapon);

        HealthPotion HPItem = new HealthPotion(20);
        _Player.GetInventory().AddItem(HPItem);

    }

    public Character GetPlayerCharacter() => _Player;

    public void SaveRoomData(RoomData data)
    {
        Save.RoomData.Add(data);
    }

    public RoomData LoadRoomData(string RoomName)
    {
        foreach (var room in Save.RoomData)
        {
            if (room.RoomName == RoomName)
            {
                return room;
            }
        }

        return null;
    }
}
