using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace TeamFourteen.AI
{
    public partial class EnemyMovement
    {
        class FollowLoop : BehaviourLoop
        {
            public FollowLoop(EnemyMovement enemyMovement, Transform followPosition, NavMeshAgent nmAgent) : base(enemyMovement)
            {
                this.nmAgent = nmAgent;

                followTraget = followPosition;
            }

            public Transform followTraget;
            private NavMeshAgent nmAgent;
            private Coroutine follow;
            private const float minimumWaitTime = 3.0f;
            private const float lostDistance = 3.5f;

            public override void Start()
            {
                enemyMovement.SetTarget(followTraget);
                follow = enemyMovement.StartCoroutine(Follow());
            }

            public override void Stop()
            {
                enemyMovement.StopCoroutine(follow);
                nmAgent.enabled = true;

                enemyMovement.targetSelector.Deselect();
            }

            private bool InFollowRange() => Vector3.Distance(nmAgent.transform.position, followTraget.transform.position) < lostDistance;

            private IEnumerator Follow()
            {
                yield return new WaitUntil(() => nmAgent.pathPending == false);

                yield return new WaitForSeconds(minimumWaitTime);

                while (nmAgent.remainingDistance > 0.05f && InFollowRange())
                {
                    yield return null;
                }

                enemyMovement.LoseTrack(followTraget.position);
            }
        }
    }
}