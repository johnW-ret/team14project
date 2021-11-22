// Written by John Whatley for personal project (Retruate). Used with permission (from self).
using System;
using System.Collections;
using System.Collections.Generic;

namespace TeamFourteen.Core
{
    public abstract class Process<S, C>
        where S : Enum
        where C : Enum
    {
        public Process() { }

        public Process(S startingState) : this()
        {
            CurrentState = startingState;
        }

        public Process(S startingState, Dictionary<StateTransition, S> dictionary) : this(startingState)
        {
            transitions = new Dictionary<StateTransition, S>(dictionary);
        }

        protected Dictionary<StateTransition, S> transitions { get; set; }
        public S CurrentState { get; protected set; } = default;

        public S GetNext(C command, out StateTransition stateTransition)
        {
            stateTransition = new StateTransition(CurrentState, command);
            S nextState;

            if (!transitions.TryGetValue(stateTransition, out nextState))
                throw new InvalidStateException(stateTransition);

            return nextState;
        }

        public bool TryMoveNext(C command) => TryMoveNext(command, out _);

        public abstract bool TryMoveNext(C command, out S nextState);

        public class StateTransition : IStateTransition
        {
            private readonly S CurrentState;
            private readonly C Command;

            public Type StateType => typeof(S);
            public Type CommandType => typeof(C);

            public StateTransition(S currentState, C command)
            {
                CurrentState = currentState;
                Command = command;
            }

            public override string ToString() => string.Format("{0}({1}, {2})", GetType().ToString(), CurrentState.ToString(), Command.ToString());

            public override int GetHashCode() => 13 + 37 * Command.GetHashCode() + 37 * CurrentState.GetHashCode();

            public override bool Equals(object obj)
            {
                StateTransition other = obj as StateTransition;
                return other != null && CurrentState.Equals(other.CurrentState) && Command.Equals(other.Command);
            }
        }
    }
}