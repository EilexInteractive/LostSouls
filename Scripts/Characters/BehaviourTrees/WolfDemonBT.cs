using System;
using System.Collections.Generic;
using Godot;
using EilexFramework.AI;

public class WolfDemonBT : EilexFramework.AI.Tree
{
    public WolfDemonBT(CharacterController owner, Blackboard blackboard) : base(owner, blackboard)
    {}

    protected override EilexFramework.AI.Node SetupTree()
    {


        return default;
    }
}