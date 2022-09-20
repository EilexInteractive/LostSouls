using System;
using System.Collections.Generic;

namespace EilexFramework.AI
{
    public class FiniteStateMachine
    {
        protected CharacterController _Owner;
        protected List<State> _States = new List<State>();
        protected State _CurrentState;

        public FiniteStateMachine(CharacterController owner)
        {
            _Owner = owner;
        }

        public void AddState(State state) => _States.Add(state);
        public virtual void Update(float delta)
        {
            if(_CurrentState != null)
            {
                _CurrentState.Update(delta);
            }
        }

        public void SetState(string stateName)
        {
            if(_CurrentState == GetState(stateName))
                return;

            if(_CurrentState != null)
                _CurrentState.OnExit();

            _CurrentState = GetState(stateName);

            if(_CurrentState != null)
                _CurrentState.OnEnter();
            
        }

        private bool HasState(string stateName)
        {
            foreach(var state in _States)
            {
                if(state.StateName == stateName)
                    return true;
            }

            return false;
        }

        private State GetState(string stateName)
        {
            if(!HasState(stateName))
                return null;

            
            foreach(var state in _States)
                if(state.StateName == stateName)
                    return state;

            return null;
        }
    }
}