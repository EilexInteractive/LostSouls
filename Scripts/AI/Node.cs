using Godot;
using System;
using System.Collections.Generic;

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

    public Node()
    {
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

    public void SetData(string key, object value)
    {
        DataContext[key] = value;
    }

    public object GetData(string key)
    {
        object value = null;

        if(DataContext.TryGetValue(key, out value))
            return value;

        Node node = Parent;
        while(node != null)
        {
            value = node.GetData(key);
            if(value != null)
                return value;

            node = node.Parent;
        }
        return null;
    }

    public bool ClearData(string key)
    {
        if(DataContext.ContainsKey(key))
        {
            DataContext.Remove(key);
            return true;
        }

        Node node = Parent;
        while(node != null)
        {
            bool cleared = node.ClearData(key);
            if(cleared)
                return true;

            node = node.Parent;
        }

        return false;
    }
}