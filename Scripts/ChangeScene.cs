using Godot;
using System;

public class ChangeScene : Area2D
{
    // === GENERAL SCENE CHANGE PROPERTIES === //
    [Export] protected string _SceneName;
    [Export] public bool CanChangeScene = true;
    [Export] public bool MovingForward = true;
    private bool _CanSave = false;

    // === INTERACTIVE SCENE CHANGE === //
    [Export] private bool _PromptToChangeScene = false;
    private bool _IsInChangeArea;


    private Timer _CanSaveRoomTimer;                 // Timer that will stop us saving when the game launches
    

    private void ChangeToScene()
    {
        if (!CanChangeScene)
            return;
        
        if (_SceneName != "")
        {
            GetNode<GameController>("/root/GameController").MovingForward = MovingForward;
            GetTree().ChangeScene("res://Scenes/" + _SceneName + ".tscn");
            GetNode<GameController>("/root/GameController").SaveGame();
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

        if(_IsInChangeArea)
        {
            if(Input.IsActionJustPressed("Interact"))
            {
                
                GetNode<SceneController>("/root/Main").SaveRoom();
                ChangeToScene();
            }
        }
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
        
        if(!_PromptToChangeScene)
        {
            GetNode<SceneController>("/root/Main").SaveRoom();
            ChangeToScene();
        } else 
        {
            _IsInChangeArea = true;
            GetNode<PlayerController>("/root/Main/Player").GetUI().TogglePickupLabel(true, "Change Rooms");
        }
    }

    public void OnBodyExit(Node node)
    {
        _IsInChangeArea = false;
        GetNode<PlayerController>("/root/Main/Player").GetUI().TogglePickupLabel(false);
    }
}
