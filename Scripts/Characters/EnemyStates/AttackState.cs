using System;
using System.Collections.Generic;
using Godot;
using EilexFramework.AI;

public class AttackState : State
{
    public AttackState(CharacterController owner) : base("Attack", owner)
    {
    }

    public override void OnEnter()
    {
        EnemyController controller = StateOwner as EnemyController;
        if(controller != null)
        {
            controller.GetBlackboard().SetValueAsBool("IsAttacking", true);
        }
    }

    public override void OnExit()
    {
        EnemyController controller = StateOwner as EnemyController;
        if(controller != null)
        {
            controller.StartAttackCooldown();
            controller.GetBlackboard().SetValueAsBool("CanAttack", false);
        }
    }

    public override void Update(float delta)
    {
        // Get reference to the enemy controller
        EnemyController enemyController = StateOwner as EnemyController;
        if(enemyController == null)
            return;

        

        // Get reference to the player controller
        PlayerController player = enemyController.GetBlackboard().GetValueAsNode2D("Player") as PlayerController;
        if(player == null)
            return;

        enemyController.PlayAttackAnimation();              // Play the attack animation

        // Apply attack damage to the player character
        player.GetOwningCharacter().TakeDamage(enemyController.GetOwningCharacter().GenerateAttackPoints());
        
        enemyController.GetFSM().SetState("FollowPlayer");
    }
}