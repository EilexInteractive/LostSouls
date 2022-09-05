using Godot;
using System;

public class DialogController : Node2D
{
    private Friendly Owner;
    private bool _DialogOpen = false;

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
        _DialogOpen = false;
        var player =
            GetNode<GameController>("/root/GameController").GetPlayerCharacter().GetController() as PlayerController;
        player?.GetUI()?.HideMessageRect();
    }

    public void SetOwner(Friendly friend) => Owner = friend;
}
