using Godot;
using System;

public class ChangeScene : Area2D
{
    [Export] private string _SceneName;
    [Export] public bool CanChangeScene = true;
    private bool _CanSave = false;

    private Timer _CanSaveRoomTimer;                 // Timer that will stop us saving when the game launches
    

    private void ChangeToScene()
    {
        if (!CanChangeScene)
            return;
        
        if (_SceneName != "")
        {
            GetTree().ChangeScene("res://Scenes/" + _SceneName + ".tscn");
        }
    }

    public override void _Ready()
    {
        base._Ready();
        _CanSaveRoomTimer = new Timer(1.0f, false, ToggleSaveRoom);

    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        if(_CanSaveRoomTimer != null)
            _CanSaveRoomTimer.UpdateTimer(delta);
    }

    private void ToggleSaveRoom()
    {
        _CanSave = true;
        _CanSaveRoomTimer = null;
    }

    public void OnBodyEntered(Node node)
    {
        if (!_CanSave)
            return; 
        
        GetNode<SceneController>("/root/Main").SaveRoom();
        ChangeToScene();
    }
}
