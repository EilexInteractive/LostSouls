using Godot;
using System;
using System.Collections.Generic;

public class MainMenu : Node2D
{
    [Export] private PackedScene _LoadGameBtn;
    private List<Node> _LoadedLoadGameBtns = new List<Node>();

    public void OnNewGame()
    {
        GetNode<VBoxContainer>("Canvas/ColorRect/OptionsMenu").Hide();
        GetNode<Control>("Canvas/ColorRect/NewGame").Show();
        PlaySFX();
    }

    public void OnStartGame()
    {
        PlaySFX();
        var name = GetNode<LineEdit>("Canvas/ColorRect/NewGame/LineEdit").Text;
        GetNode<GameController>("/root/GameController").GameName = name;
        GetTree().ChangeScene("res://Scenes/Main.tscn");
        
    }

    public void OnLoadGame()
    {
        GetNode<VBoxContainer>("Canvas/ColorRect/OptionsMenu").Hide();
        GetNode<VBoxContainer>("Canvas/ColorRect/LoadGame").Show();

        if(System.IO.Directory.Exists("Saves"))
        {
            var files = System.IO.Directory.GetFiles("Saves");
            for(int i = 0; i < files.Length; ++i)
            {
                var file = files[i].BaseName();             // Remove the extension from the file name
                file = file.Replace(@"Saves\", "");
                GD.Print(file);

                
                Node loadGameInstance = _LoadGameBtn.Instance();
                GetNode<VBoxContainer>("Canvas/ColorRect/LoadGame").AddChild(loadGameInstance);
                GetNode<VBoxContainer>("Canvas/ColorRect/LoadGame").MoveChild(loadGameInstance, 0);
                _LoadedLoadGameBtns.Add(loadGameInstance);
                var loadGameObj = loadGameInstance as LoadGameButton;
                if(loadGameObj != null)
                {
                    loadGameObj.SavePath = files[i];
                    loadGameObj.SetSaveGameName(file);
                }

                
            }
        } 

        PlaySFX();
    }

    public void OnQuitGame()
    {
        PlaySFX();
        GetTree().Quit();
    }
    
    public void OnSettingsPressed()
    {
        PlaySFX();
        GetNode<VBoxContainer>("Canvas/ColorRect/OptionsMenu").Hide();                 // Hide the menu options
        GetNode<Control>("Canvas/ColorRect/Settings").Show();                    // Display the settings
        GetNode<SettingsController>("Canvas/ColorRect/Settings").SetupSettings();               // Displauy the current Settings
    }

    public void LoadGameBack()
    {
        PlaySFX();
        GetNode<VBoxContainer>("Canvas/ColorRect/LoadGame").Hide();
        GetNode<VBoxContainer>("Canvas/ColorRect/OptionsMenu").Show();

        foreach(var btn in _LoadedLoadGameBtns)
        {
            btn.QueueFree();
        }

        _LoadedLoadGameBtns.Clear();
    }

    public void PlaySFX()
    {
        if(GetNode<GameController>("/root/GameController").GetCurrentSettings().SFXOn)
            GetNode<AudioStreamPlayer2D>("MenuSFX").Play();
    }
}
