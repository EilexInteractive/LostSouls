using Godot;
using System;

public class LoadGameButton : Button
{
    public string SavePath;
    public string SaveGameName;

    public void OnLoadPressed()
    {
        GetNode<GameController>("/root/GameController").GameName = SaveGameName;
        GetNode<GameController>("/root/GameController").LoadGame();
    }
}
