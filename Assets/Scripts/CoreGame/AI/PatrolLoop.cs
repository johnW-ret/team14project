using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace TeamFourteen.AI
{
    public partial class EnemyMovement
    {
        class PatrolLoop : BehaviourLoop
        {
            public PatrolLoop(EnemyMovement enemyMovement, Patroller patroller, NavMeshAgent nmAgent) : base(enemyMovement)
            {
                this.nmAgent = nmAgent;
                this.patroller = patroller;
            }

            private NavMeshAgent nmAgent;
            private Patroller patroller;
            private Coroutine patrol;

            public override void Start()
            {
                enemyMovement.SetTarget(patroller.GetNextPoint());
                patrol = enemyMovement.StartCoroutine(Patrol());
            }

            public override void Stop()
            {
                enemyMovement.StopCoroutine(patrol);
                nmAgent.enabled = true;
            }

            private IEnumerator Patrol()
            {
                yield return new WaitUntil(() => nmAgent.pathPending == false);

                while (true)
                {
                    if (nmAgent.remainingDistance <= 0.05f)
                    {
                        nmAgent.enabled = false;

                        yield return LookAround();

                        nmAgent.enabled = true;
                        enemyMovement.SetTarget(patroller.GetNextPoint());
                    }
                    yield return null;
                }
            }
        }
    }
}