using System;
using System.Collections.Generic;

namespace EilexFramework.AI
{
    public class Sequence : Node 
    {
        public Sequence() : base()
        {}

        public Sequence(List<Node> children) : base(children)
        {}

        public override NodeState Evaluate()
        {
            bool childrenRunning = false;                   // pre-defined check to see if children nodes are running

            // Loop through each child node to get their state and update this state
            foreach(var node in _Children)
            {
                switch(node.Evaluate())
                {
                    case NodeState.FAILURE:
                        _State = NodeState.FAILURE;
                        return _State;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        childrenRunning = true;
                        continue;
                    default:
                        _State = NodeState.SUCCESS;
                        return _State;
                }
            }

            // Determine the state of this node
            if(childrenRunning)
            {
                _State = NodeState.RUNNING;
            } else 
            {
                _State = NodeState.SUCCESS;
            }

            return _State;              // Return the current state of this node
        }
    }
}