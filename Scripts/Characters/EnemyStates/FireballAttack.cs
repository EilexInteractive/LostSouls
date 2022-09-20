using System;
using System.Collections.Generic;
using Godot;
using EilexFramework.AI;

public class FireballAttack : AttackState
{
    private Viewport _RootNode;
    public FireballAttack(CharacterController owner, Viewport rootNode) : base(owner)
    {
        _RootNode = rootNode;
    }

    public override void Update(float delta)
    {
        base.Update(delta);

        EnemyController controller = StateOwner as EnemyController;

        // Spawn the fire ball
        var fireball = GD.Load<PackedScene>("res://Prefabs/FireBall.tscn");
        var fireBallInstance = fireball.Instance();
        _RootNode.AddChild(fireBallInstance);
        var fireballNode = fireBallInstance as Node2D;

        fireballNode.Position = StateOwner.Position;                    // Set the position of the fireball
        fireballNode.LookAt(controller.GetBlackboard().GetValueAsNode2D("Player").Position);        // Look at the player character when firing
    }
}