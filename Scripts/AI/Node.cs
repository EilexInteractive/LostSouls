using Godot;
using System;
using System.Collections.Generic;

namespace EilexFramework.AI
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public class Node
    {
        protected NodeState _State;                  // Reference to the state of the node

        public Node Parent;                        // Reference to a parent node
        protected List<Node> _Children = new List<Node>();

        private Dictionary<string, object> DataContext = new Dictionary<string, object>();


        protected EilexFramework.AI.Tree _OwningTree;
        protected EnemyController _Owner;

        public Node(EilexFramework.AI.Tree owningTree, EnemyController owner)
        {
            _OwningTree = owningTree;
            _Owner = owner;
            Parent = null;
        }

        public Node(List<Node> children)
        {
            foreach(var child in children)
            {
                Attach(child);
            }
        }

        private void Attach(Node node)
        {
            node.Parent = this;
            _Children.Add(node);
        }

        public virtual NodeState Evaluate() => NodeState.FAILURE;
}