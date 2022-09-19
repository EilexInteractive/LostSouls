using System;
using System.Collections.Generic;

namespace EilexFramework.AI
{
    public class Selector : Node 
    {
        public Selector(EilexFramework.AI.Tree owningTree, EnemyController owner) : base(owningTree, owner)
        {}

        public Selector(List<Node> children) : base(children)
        {}

        public override NodeState Evaluate()
        {
            foreach(var node in _Children)
            {
                switch(node.Evaluate())
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        _State = NodeState.SUCCESS;
                        return _State;
                    case NodeState.RUNNING:
                        _State = NodeState.RUNNING;
                        return _State;
                    default:
                        continue;
                }
            }

            _State = NodeState.FAILURE;
            return _State;
        }
    }
}