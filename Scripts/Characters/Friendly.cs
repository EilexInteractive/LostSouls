using Godot;
using System;
using System.Collections.Generic;

public class Friendly : CharacterController, IInteractable
{
    [Export] public bool FollowPath;                             // If the friendly is following a path
    private Godot.Collections.Array _PathPoints;
    private int _CurrentPointIndex;
    private Sprite _InteractionPrompt;

    private Timer WaitToMoveTimer;

    private NavAgent _NavAgent;                         // Reference to the nav agent
    
    
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

        _NavAgent = GetNode<NavAgent>("NavAgent");
        _NavAgent.SetOwner(this);

        
        //_NavAgent.SetPath(GetNode<Node2D>("/root/Main/PathPoint").Position);
        
        

        // Disable movement for now
        CanMove = true;

    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if(_NavAgent != null && CanMove)
        {
            AnimationUpdate(_NavAgent.GetMovementDirection());
        }
    }

    private void Move(float delta)
    {
        if (!CanMove)
            return;
        
    }

    protected override void AnimationUpdate(Vector2 velocity)
    {
        if(velocity.x > 0)
        {
            _Anim.Play("WalkRight");
        } else if(velocity.x < 0)
        {
            _Anim.Play("WalkLeft");
        } else if(velocity.y > 0)
        {
            _Anim.Play("WalkDown");
        } else if(velocity.y < 0)
        {
            _Anim.Play("WalkUp");
        } else 
        {
            _Anim.Play("IdleDown");
        }
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
