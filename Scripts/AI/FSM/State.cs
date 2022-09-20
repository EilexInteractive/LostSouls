using System;
using System.Collections.Generic;
using Godot;

namespace EilexFramework.AI
{
    public abstract class State
    {
        public string StateName;                                     // Name of the task
        protected CharacterController StateOwner;                    // Which character owns this task

        public State(string stateName, CharacterController owner)
        {
            StateName = stateName;
            StateOwner = owner;
        }
        

        public abstract void OnEnter();
        public abstract void OnExit();

        public abstract void Update(float delta);

    }
}