using System;
using System.Collections.Generic;
using Godot;
using EilexFramework.AI;

public class FollowPlayerState : State
{
    public FollowPlayerState(CharacterController owner) : base("FollowPlayer", owner)
    {
    }

    public override void OnEnter()
    {
        
    }

    public override void OnExit()
    {
        
    }

    public override void Update(float delta)
    {
        var enemyController = StateOwner as EnemyController;
        if(enemyController == null)
            return;

        var playerController = enemyController.GetBlackboard().GetValueAsNode2D("Player");
        if(playerController == null)
            return;

        enemyController.GetNavAgent().SetPath(playerController.Position);
    }
}