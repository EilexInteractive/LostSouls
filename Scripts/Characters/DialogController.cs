using Godot;
using System;

public class DialogController : Node2D
{
    private Friendly Owner;
    private bool _DialogOpen = false;
    private event Action _DialogEvent = null;

    private Timer _CloseDialogTimer;

    public override void _Process(float delta)
    {
        base._Process(delta);
        
    }

    public void OpenDialog()
    {
        _DialogOpen = true;
        var player = GetNode<GameController>("/root/GameController").GetPlayerCharacter().GetController() as PlayerController;
        if (player != null)
        {
            player.GetUI().SetMessage(Owner.DialogMessage);
        }
    }

    public void CloseDialog()
    {
        GD.Print("Closing Dialog");
        _DialogOpen = false;
        var player =
            GetNode<GameController>("/root/GameController").GetPlayerCharacter().GetController() as PlayerController;
        player?.GetUI()?.HideMessageRect();

        if (_DialogEvent != null)
        {
            _DialogEvent.Invoke();
        }
    }
    
    public void SetOwner(Friendly friend) => Owner = friend;
    public void SetDialogEvent(Action actionEvent) => _DialogEvent = actionEvent;
}
