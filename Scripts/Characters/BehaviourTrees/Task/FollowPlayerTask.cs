using System;
using System.Collections.Generic;
using EilexFramework.AI;

public class FollowPlayerTask : Node 
{
    private Tree _OwningTree;
    private EnemyController _Owner;

    public FollowPlayerTask(Tree owningTree, EnemyController owner)
    {
        _OwningTree = owningTree;
        _Owner = owner;
    }

    public override NodeState Evaluate()
    {
        var playerLocation = _Owner.GetBlackboard().GetValueAsVector2("TargetLocation");
        _Owner.GetNavAgent().SetPath(playerLocation);

    
        _State = NodeState.RUNNING;
        return _State;
    }
    
}