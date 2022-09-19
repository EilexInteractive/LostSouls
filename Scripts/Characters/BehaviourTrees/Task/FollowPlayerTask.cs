using System;
using System.Collections.Generic;
using EilexFramework.AI;
using Godot;

public class FollowPlayerTask : EilexFramework.AI.Node 
{
    
    public FollowPlayerTask(EilexFramework.AI.Tree owningTree, EnemyController owner) : base(owningTree, owner)
    {}

    public override NodeState Evaluate()
    {
        if(_Owner == null)
            GD.Print("Null Owner");
        var playerLocation = _Owner.GetBlackboard().GetValueAsVector2("TargetLocation");
        _Owner.GetNavAgent().SetPath(playerLocation);

    
        _State = NodeState.RUNNING;
        return _State;
    }
    
}