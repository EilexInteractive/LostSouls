using System;
using System.Collections.Generic;
using Godot;
using EilexFramework.AI;

public class BaseEnemyFSM : FiniteStateMachine
{
    private EnemyController _EnemyController;
    public BaseEnemyFSM(CharacterController owner) : base(owner)
    {
        _EnemyController = owner as EnemyController;
    }

    public override void Update(float delta)
    {
        base.Update(delta);

        if(_EnemyController == null)
            return;

        if(_EnemyController.GetBlackboard().GetValueAsBool("IsAlive") == false)
        {
            _EnemyController.AlreadyDead();
            return;
        }



        if(_Owner.Position.DistanceTo(_EnemyController.GetBlackboard().GetValueAsNode2D("Player").Position) < 
            _EnemyController.GetBlackboard().GetValueAsFloat("AttackDistance") && _EnemyController.GetBlackboard().GetValueAsBool("CanAttack"))
        {
            SetState("Attack");
            return;
        } 


        SetState("FollowPlayer");
        return;

    }
}