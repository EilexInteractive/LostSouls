using Godot;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public struct LoadGameData
{
    public Character Player;
    public Vector2 Position;
}

public class GameController : Node
{
    private Character _Player;
    public Save Save;
    public bool IsLoadedGame = false;
    public string GameName;
    public LoadGameData LoadGameData;
    public bool MovingForward;

    public bool _Respawning = false;

    private Settings CurrentGameSettings;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CreateNewPlayer();
        Save = new Save();

        LoadSettings();
    }

    public void CreateNewPlayer()
    {
        _Player = new Character("Player", true);
        Weapon firstWeapon = GetNode<ItemDatabase>("/root/ItemDatabase").GetItem("Old Sword") as Weapon;
        _Player.GetInventory().AddItem(firstWeapon);
        _Player.GetInventory().EquipWeapon(firstWeapon);

        HealthPotion HPItem = new HealthPotion(20, ItemRarity.Common);
        _Player.GetInventory().AddItem(HPItem);

    }

    public Character GetPlayerCharacter() => _Player;

    public void SaveRoomData(RoomData data)
    {
        Save.RoomData.Add(data);
    }

    public RoomData LoadRoomData(string roomName)
    {
        foreach (var room in Save.RoomData)
        {
            if (room.RoomName == roomName)
            {
                return room;
            }
        }

        return null;
    }

    public void SaveGame()
    {
        // Create a directory for saves if one hasn't been created already
        if(!System.IO.Directory.Exists("Saves"))
        {
            System.IO.Directory.CreateDirectory("Saves");
        }

        // Set the save data
        var node = GetTree().CurrentScene as SceneController;
        if(node != null)
        {
            Save._Player = _Player;
            Save._CurrentRoomName = node.RoomName;
            Save.PlayerPosition = GetNode<PlayerController>("/root/Main/Player").Position;
            node.SaveRoom();
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = System.IO.File.Create($"Saves/{GameName}.ei");

        bf.Serialize(file, Save);
        file.Close();
        GD.Print("Game Saved");
    }

    public void LoadGame()
    {
        if(System.IO.File.Exists($"Saves/{GameName}.ei") == false)
            return;


        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = System.IO.File.Open($"Saves/{GameName}.ei", FileMode.Open);
        Save = (Save)bf.Deserialize(file);
        file.Close();

        LoadGameData = new LoadGameData
        {
            Player = Save._Player,
            Position = Save.PlayerPosition
        };

        IsLoadedGame = true;

        GetTree().ChangeScene("res://Scenes/" + Save._CurrentRoomName + ".tscn");
        _Player = Save._Player;

        if(_Respawning)
        {
            _Player.IncreaseHealth(100);
            _Respawning = false;
        }
            

    }

    public void LoadSettings()
    {
        if(System.IO.File.Exists("Settings.ei") == false)
        {
            // Create new settings if we don't already have any
            Settings initSettings = new Settings();
            initSettings._MusicLevel = 1.0f;
            initSettings.SFXOn = true;
            SaveSettings(initSettings);
            CurrentGameSettings = initSettings;
        } else 
        {
            // Load Saved settings
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = System.IO.File.Open("Settings.ei", FileMode.Open);
            CurrentGameSettings = (Settings)bf.Deserialize(fs);
            fs.Close();

            // Set the music level
            GetNode<AudioStreamPlayer2D>("/root/BackgroundMusic/AudioStreamPlayer2D").VolumeDb = CurrentGameSettings._MusicLevel;

        }

        
        
    }

    public void SaveSettings(Settings newSettings)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = System.IO.File.Create("Settings.ei");
        bf.Serialize(fs, newSettings);
        fs.Close();
        GD.Print("Settings Saved");
    }

    public Settings GetCurrentSettings() => CurrentGameSettings;
}
