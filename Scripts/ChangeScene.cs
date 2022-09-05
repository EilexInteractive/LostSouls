using Godot;
using System;

public class ChangeScene : Area2D
{
    [Export] private string _SceneName;
    [Export] public bool CanChangeScene = true;

    private void ChangeToScene()
    {
        if (!CanChangeScene)
            return;
        
        if (_SceneName != "")
        {
            GetTree().ChangeScene("res://Scenes/" + _SceneName + ".tscn");
        }
    }

    public void OnBodyEntered(Node node)
    {
        GetNode<SceneController>("/root/Main").SaveRoom();
        ChangeToScene();
    }
}
