using Godot;
using System;

public class MainMenu : Node2D
{
    [Export] private PackedScene _LoadGameBtn;

    public void OnNewGame()
    {
        GetNode<VBoxContainer>("Canvas/ColorRect/OptionsMenu").Hide();
        GetNode<Control>("Canvas/ColorRect/NewGame").Show();
        PlaySFX();
    }

    public void OnStartGame()
    {
        // TODO: Create new save game
        var name = GetNode<LineEdit>("Canvas/ColorRect/NewGame/LineEdit").Text;
        GetNode<GameController>("/root/GameController").GameName = name;
        GetTree().ChangeScene("res://Scenes/Main.tscn");
        PlaySFX();
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

                
                var loadGameInstance = _LoadGameBtn.Instance();
                GetNode<VBoxContainer>("Canvas/ColorRect/LoadGame").AddChild(loadGameInstance);
                var loadGameObj = loadGameInstance as LoadGameButton;
                if(loadGameObj != null)
                {
                    loadGameObj.SavePath = files[i];
                    loadGameObj.SaveGameName = file;
                    loadGameObj.Text = file;
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

    public void PlaySFX()
    {
        if(GetNode<GameController>("/root/GameController").GetCurrentSettings().SFXOn)
            GetNode<AudioStreamPlayer2D>("MenuSFX").Play();
    }
}
