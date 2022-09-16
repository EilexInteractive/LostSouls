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
        
    }

    public void Interact()
    {
        GetNode<DialogController>("DialogController").OpenDialog();
        GetNode<Sprite>("InteractionPrompt").Hide();
    }

    public void EnableInteractionPrompt() => _InteractionPrompt.Show();
    public void DisableInteractionPrompt() => _InteractionPrompt.Hide();
    public DialogController GetDialogController() => GetNode<DialogController>("DialogController");


}
