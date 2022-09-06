using Godot;
using System;
using System.Collections.Generic;

public class Friendly : CharacterController, IInteractable
{
    private Godot.Collections.Array _PathPoints;
    private Vector2[] _Path;
    private int _CurrentPointIndex;
    private Vector2 _MoveToPos;
    private Navigation2D _Nav;
    private Sprite _InteractionPrompt;

    private Timer WaitToMoveTimer;
    
    
    // === DIALOG MESSAGE === //
    [Export] public string DialogMessage;
    [Export] public float DialogCloseTime;

    public override void _Ready()
    {
        base._Ready();
        _PathPoints = GetTree().GetNodesInGroup("PathPoint");
        _Nav = GetNode<Navigation2D>("/root/Main/Navigation2D");
        _CurrentPointIndex = 0;

        // Set the owner of the dialog
        var dialogController = GetNode<DialogController>("DialogController");
        dialogController?.SetOwner(this);

        _InteractionPrompt = GetNode<Sprite>("InteractionPrompt");
        

        // Disable movement for now
        CanMove = false;

    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        
        // Wait until we increment to the next index
        if(WaitToMoveTimer != null)
            WaitToMoveTimer.UpdateTimer(delta);
        
        
        Move(delta);
    }

    private void Move(float delta)
    {
        if (!CanMove)
            return;
        
        _Path = _Nav?.GetSimplePath(this.GlobalPosition, _MoveToPos);
        var velocity = new Vector2();
        if (_Path.Length > 1)
        {
            var distanceToPath = _Path[1] - GlobalPosition;
            GD.Print("Distance" + distanceToPath);
            var directionToPath = distanceToPath.Normalized();
            if (distanceToPath.Length() > 0.08)
            {
                velocity = new Vector2(directionToPath * (_MovementSpeed * delta));
                _Anim.Play("Walk");
            }
            else
            {
                if(WaitToMoveTimer == null)
                    WaitToMoveTimer = new Timer(2.0f, false, IncrementPathPoint);
                _Anim.Play("Idle");
            }

            Update();
        }

        MoveAndCollide(velocity);
    }

    public void Interact()
    {
        GetNode<DialogController>("DialogController").OpenDialog();
        GetNode<Sprite>("InteractionPrompt").Hide();
    }

    private void IncrementPathPoint()
    {
        WaitToMoveTimer = null;
        _CurrentPointIndex++;               // Increment the path index
        if (_CurrentPointIndex >= _PathPoints.Count)
        {
            _CurrentPointIndex = 0;
        }

        GD.Print(_CurrentPointIndex);
        var node = _PathPoints[_CurrentPointIndex] as Node2D;
        _MoveToPos = node.Position;

    }

    public void EnableInteractionPrompt() => _InteractionPrompt.Show();
    public void DisableInteractionPrompt() => _InteractionPrompt.Hide();
    public DialogController GetDialogController() => GetNode<DialogController>("DialogController");


}
