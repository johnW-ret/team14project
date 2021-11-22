using System.Collections.Generic;
using TeamFourteen.Core;

namespace TeamFourteen.CoreGame
{
    public enum BehaviourState
    {
        Off,
        Patrol,
        Investigate,
        Follow
    }

    public class EnemyAIBehaviourProcess : ActionProcess<BehaviourState, EnemyAIBehaviourProcess.Command>
    {
        public EnemyAIBehaviourProcess() : base()
        {
            transitions = new Dictionary<StateTransition, BehaviourState>
            {
                // on / off state transitions
                { new StateTransition(BehaviourState.Off, Command.Enable), BehaviourState.Patrol },
                { new StateTransition(BehaviourState.Patrol, Command.Disable), BehaviourState.Off },
                { new StateTransition(BehaviourState.Investigate, Command.Disable), BehaviourState.Off },
                { new StateTransition(BehaviourState.Follow, Command.Disable), BehaviourState.Off },

                { new StateTransition(BehaviourState.Patrol, Command.See), BehaviourState.Follow },
                { new StateTransition(BehaviourState.Patrol, Command.Notice), BehaviourState.Investigate },

                { new StateTransition(BehaviourState.Investigate, Command.See), BehaviourState.Follow },
                { new StateTransition(BehaviourState.Investigate, Command.Notice), BehaviourState.Investigate },
                { new StateTransition(BehaviourState.Investigate, Command.Forget), BehaviourState.Patrol },

                { new StateTransition(BehaviourState.Follow, Command.See), BehaviourState.Follow },             // See & Notice transitions, maybe not necessary
                { new StateTransition(BehaviourState.Follow, Command.Notice), BehaviourState.Follow },          //
                { new StateTransition(BehaviourState.Follow, Command.LoseTrack), BehaviourState.Investigate },  // save last known position and investigate there
                { new StateTransition(BehaviourState.Follow, Command.Forget), BehaviourState.Patrol },          // should never happen. in the case it does, we should just go back to patrolling
            };
        }

        public enum Command
        {
            Enable,
            Disable,
            See,
            Notice,
            LoseTrack,
            Forget
        }
    }
}
