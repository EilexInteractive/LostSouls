using Godot;
using System;

public class LeverController : Area2D, IInteractable
{
    [Export] private Vector2 _DoorPosition = new Vector2();
    [Export] private string _TileMapName = "Ground";
    [Export] private int _NewDoorIndex = 1;

    private Sprite _LeverSprite;

    public override void _Ready()
    {
        base._Ready();

        _LeverSprite = GetNode<Sprite>("Sprite");
    }

    public void OnBodyEnter(Node body)
    {
        if (body is PlayerController)
        {
            var pc = body as PlayerController;
            if (pc != null)
            {
                pc._InteractableItem = this;
                pc.GetUI().TogglePickupLabel(true, "Pull Lever");
            }
                
        }
    }

    public void OnBodyExit(Node body)
    {
        if (body is PlayerController)
        {
            var pc = body as PlayerController;
            if (pc != null)
            {
                pc._InteractableItem = null;
                pc.GetUI().TogglePickupLabel(false);
            }
        }
    }

    public void OpenDoor()
    {
        var tileMap = GetNode<TileMap>("/root/Main/Navigation2D/" + _TileMapName);
        if (tileMap != null)
        {
            tileMap.SetCellv(_DoorPosition, _NewDoorIndex);
            _LeverSprite.FlipH = true;
            
        }
    }
}
