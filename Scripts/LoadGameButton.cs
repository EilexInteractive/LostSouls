using Godot;
using System;

public class LoadGameButton : TextureButton
{
    public string SavePath;
    public string SaveGameName;

    public void OnLoadPressed()
    {
        GetNode<GameController>("/root/GameController").GameName = SaveGameName;
        GetNode<GameController>("/root/GameController").LoadGame();
    }

    public void SetSaveGameName(string name)
    {
        SaveGameName = name;
        GetNode<Label>("Label").Text = name;
    }
}
