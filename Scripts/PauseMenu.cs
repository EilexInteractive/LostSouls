using Godot;
using System;

public class PauseMenu : CanvasModulate
{

    

    public void OnResumeGame()
    {
        GetTree().Paused = false;
        this.Hide();
        GetNode<CanvasModulate>("../Canvas").Show();
    }

    public void OnSaveGame()
    {
        GetNode<GameController>("/root/GameController").SaveGame();
    }

    public void OnLoadGame()
    {
        GetNode<GameController>("/root/GameController/").LoadGame();
    }

    public void OnQuitGame()
    {
        GetTree().Quit();
    }
}
