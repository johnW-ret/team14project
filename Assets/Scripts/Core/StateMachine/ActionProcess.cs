// Written by John Whatley for personal project (Retruate). Used with permission (from self).
using System;
using UnityEngine;

namespace TeamFourteen.Core
{
    public class ActionProcess<S, C> : Process<S, C>
        where S : Enum
        where C : Enum
    {
        private ActionPairs<S> entryActions = new ActionPairs<S>();
        private ActionPairs<S> exitActions = new ActionPairs<S>();
        private ActionPairs<StateTransition> stateTransitionActionPairs = new ActionPairs<StateTransition>();

        public enum StateDirection
        {
            In,
            Out
        }

        public override bool TryMoveNext(C command, out S nextState)
        {
            bool result = false;

            try
            {
                // get next state
                nextState = GetNext(command, out StateTransition stateTransition);

                // invoke exit action
                exitActions.TryInvoke(CurrentState);

                // invoke transition action and set next state
                stateTransitionActionPairs.TryInvoke(stateTransition);
                CurrentState = nextState;

                // invoke entry action
                entryActions.TryInvoke(CurrentState);
                result = true;
            }
            catch (InvalidStateException e)
            {
                nextState = default;
                Debug.Log(e.Message);
            }

            return result;
        }

        public void SetStateTransitionAction(Action action, StateTransition stateTransition)
        {
            if (transitions.ContainsKey(stateTransition))
                stateTransitionActionPairs.SetAction(action, stateTransition);
        }

        public void SetInOutAction(Action action, S state, StateDirection direction)
        {
            switch (direction)
            {
                case StateDirection.In:
                    entryActions.SetAction(action, state);
                    break;
                case StateDirection.Out:
                    exitActions.SetAction(action, state);
                    break;
            }
        }
    }
}