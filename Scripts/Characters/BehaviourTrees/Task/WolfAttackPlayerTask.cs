using System;
using System.Collections.Generic;
using Godot;
using EilexFramework.AI;

public class WolfAttackPlayerTask : EilexFramework.AI.Node
{
    public WolfAttackPlayerTask(EilexFramework.AI.Tree owningTree, EnemyController owner) : base(owningTree, owner)
    {}

    public override NodeState Evaluate()
    {
        if(_Owner == null || _OwningTree == null)
        {
            _State = NodeState.FAILURE;
            return _State;
        }

        // Get reference to the player character
        var player = _Owner.GetBlackboard().GetValueAsNode2D("Player");
        if(player == null)
        {
            _State = NodeState.FAILURE;
            return _State;
        }

        // Get the distance to the player from the owners position
        var distanceTo = _Owner.Position.DistanceTo(player.Position);

        if(distanceTo < _Owner.GetBlackboard().GetValueAsFloat("AttackDistance") &&
            _Owner.GetBlackboard().GetValueAsBool("CanAttack") == true)
        {
            _Owner.PlayAttackAnimation();
            _Owner.GetBlackboard().SetValueAsBool("CanAttack", false);

            // Validate the player controller
            var playerController = player as PlayerController;
            if(playerController == null)
            {
                _State = NodeState.FAILURE;
                return _State;
            }

            // Get the player character and validate it
            var playerCharacter = playerController.GetOwningCharacter();
            if(playerCharacter == null)
            {
                _State = NodeState.FAILURE;
                return _State;
            }

            playerCharacter.TakeDamage(_Owner.GetOwningCharacter().GetCurrentAP());

        }

        _State = NodeState.RUNNING;
        return _State;
        


    }
}

