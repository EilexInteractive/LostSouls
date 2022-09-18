using System;
using System.Collections.Generic;

public abstract class Tree 
{
    private Node _Root = null;
    protected CharacterController _Owner;                       // Reference to the owning character
    protected Blackboard _Blackboard;                           // Reference to the characters blackboard

    public Tree(CharacterController character, Blackboard blackboard)
    {
        _Root = SetupTree();
        _Owner = character;
        _Blackboard = blackboard;
    }

    public void Update(float delta)
    {
        if(_Root != null)
            _Root.Evaluate();
    }

    public abstract Node SetupTree();
}