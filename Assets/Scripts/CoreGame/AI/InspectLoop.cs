using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TeamFourteen.CoreGame;

namespace TeamFourteen.AI
{
    public partial class EnemyMovement
    {
        class InspectLoop : BehaviourLoop
        {
            public InspectLoop(EnemyMovement enemyMovement, Vector3 inspectPosition, NavMeshAgent nmAgent) : base(enemyMovement)
            {
                this.nmAgent = nmAgent;
                insPosition = inspectPosition;

                inspectTargetGO = new GameObject("Inspection Target");
                inspectTargetGO.transform.position = insPosition;
                inspectTargetGO.isStatic = true;
            }

            private readonly GameObject inspectTargetGO;
            public Transform inspectTarget => inspectTargetGO.transform;
            private NavMeshAgent nmAgent;
            private Vector3 insPosition;
            private Coroutine inspect;

            public override void Start()
            {
                enemyMovement.SetTarget(inspectTarget);
                inspect = enemyMovement.StartCoroutine(Inspect());
            }

            public override void Stop()
            {
                enemyMovement.StopCoroutine(inspect);
                nmAgent.enabled = true;

                enemyMovement.targetSelector.Deselect();
                Destroy(inspectTargetGO);
            }

            private IEnumerator Inspect()
            {
                yield return new WaitUntil(() => nmAgent.pathPending == false);

                while (nmAgent.remainingDistance > 0.05f)
                {
                    yield return null;
                }

                nmAgent.enabled = false;
                yield return LookAround();
                nmAgent.enabled = true;

                // return to patrol
                enemyMovement.aiProcess.TryMoveNext(EnemyAIBehaviourProcess.Command.Forget);
            }
        }
    }
}