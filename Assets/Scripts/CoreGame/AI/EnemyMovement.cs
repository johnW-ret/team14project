using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TeamFourteen.Selection;
using TeamFourteen.CoreGame;

namespace TeamFourteen.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyMovement : Movement
    {
        public enum PatrollerType
        {
            Loop,
            FollowPath
        }

        [SerializeField] [HideInInspector] private NavMeshAgent nmAgent;
        private ObjectContainer<Transform> targetSelector = new ObjectContainer<Transform>();

        [Header("References")]
        [Tooltip("List of points that the Actor patrols between.")]
        [SerializeField] private List<PatrolPoints> patrolPoints;

        [Header("Properties")]
        [Tooltip("Defines the patrol method. Does not change behaviour during runtime.")]
        [SerializeField] private PatrollerType patrollerType;
        [SerializeField] private AnimationCurve lookBehaviourCurve;

        private EnemyAIBehaviourProcess aiProcess = new EnemyAIBehaviourProcess();
        private Patroller patroller;

        [ContextMenu("Set References")]
        private void SetRefernces()
        {
            if (!nmAgent)
                nmAgent = GetComponent<NavMeshAgent>();
        }

        private void Reset()
        {
            SetRefernces();
        }

        private bool CheckPatrolPoints()
        {
            //Checks to see if there are points in the list and that there are at least 2 of those points
            if (patrolPoints != null && patrolPoints.Count >= 2)
                return true;
            else
                Debug.Log("Issue in number of Patrol Points.");

            return false;
        }

        protected override void Start()
        {
            base.Start();

            if (CheckPatrolPoints())
            {
                // define patroller
                switch (patrollerType)
                {
                    case PatrollerType.FollowPath:
                        patroller = new PathPatroller(patrolPoints);
                        break;

                    case PatrollerType.Loop:
                        patroller = new LoopPatroller(patrolPoints);
                        break;
                }

                PatrolLoop patrolLoop = new PatrolLoop(this, patroller, nmAgent);

                aiProcess.SetInOutAction(() => { patrolLoop.Start(); }, BehaviourState.Patrol, Core.ActionProcess<BehaviourState, EnemyAIBehaviourProcess.Command>.StateDirection.In);
                aiProcess.SetInOutAction(() => { patrolLoop.Stop(); }, BehaviourState.Patrol, Core.ActionProcess<BehaviourState, EnemyAIBehaviourProcess.Command>.StateDirection.Out);

                aiProcess.SetInOutAction(() => { inspectLoop.Start(); }, BehaviourState.Investigate, Core.ActionProcess<BehaviourState, EnemyAIBehaviourProcess.Command>.StateDirection.In);
                aiProcess.SetInOutAction(() => { inspectLoop.Stop(); inspectLoop = null; }, BehaviourState.Investigate, Core.ActionProcess<BehaviourState, EnemyAIBehaviourProcess.Command>.StateDirection.Out);

                aiProcess.SetInOutAction(() => { followLoop.Start(); }, BehaviourState.Follow, Core.ActionProcess<BehaviourState, EnemyAIBehaviourProcess.Command>.StateDirection.In);
                aiProcess.SetInOutAction(() => { followLoop.Stop(); followLoop = null; }, BehaviourState.Follow, Core.ActionProcess<BehaviourState, EnemyAIBehaviourProcess.Command>.StateDirection.Out);

                if (!aiProcess.TryMoveNext(EnemyAIBehaviourProcess.Command.Enable))
                    Debug.LogError($"Error starting Enemy AI {gameObject.name} state machine.");
            }
        }

        private void Update()
        {
            // set position target each frame if gameobject is not static
            if (nmAgent.enabled && (!targetSelector.Selected?.gameObject.isStatic ?? false))
            {
                nmAgent.SetDestination(targetSelector.Selected.position);
            } 
        }

        private void FixedUpdate()
        {
            if (aiProcess.CurrentState != BehaviourState.Investigate && aiProcess.CurrentState != BehaviourState.Follow)
                LongLineOfSight();
            else if (aiProcess.CurrentState == BehaviourState.Investigate)
                ShortLineOfSight();

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position + (1.05f * transform.forward), transform.forward * 7);
        }

        RaycastHit hit;
        private void LongLineOfSight()
        {
            // search for long-range
            Ray longLineOfSight = new Ray(transform.position + (1.05f * transform.forward), transform.forward);
            if (Physics.Raycast(longLineOfSight, out hit, 7)
                && hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))   // later, only detect player if holding lantern
            {
                Debug.Log("Long: " + hit.transform.gameObject.name);
                Notice(hit.point);
            }
        }

        private void ShortLineOfSight()
        {
            Ray shortLineOfSight = new Ray(transform.position + (1.05f * transform.forward), transform.forward);
            if (Physics.Raycast(shortLineOfSight, out hit, 2)
                && hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Debug.Log("Short: " + hit.transform.gameObject.name);
                See(hit.transform);
            }
        }

        private void Notice(Vector3 inspectPosition)
        {
            inspectLoop = new InspectLoop(this, inspectPosition, nmAgent);
            aiProcess.TryMoveNext(EnemyAIBehaviourProcess.Command.Notice);
        }

        private void See(Transform followPosition)
        {
            followLoop = new FollowLoop(this, followPosition, nmAgent);
            aiProcess.TryMoveNext(EnemyAIBehaviourProcess.Command.See);
        }

        private void LoseTrack(Vector3 inspectPosition)
        {
            inspectLoop = new InspectLoop(this, inspectPosition, nmAgent);
            aiProcess.TryMoveNext(EnemyAIBehaviourProcess.Command.LoseTrack);
        }

        /// <summary>
        /// Sets the target and sets an initial destination for the NavMeshAgent.
        /// </summary>
        /// <param name="target">The Transform to target.</param>
        private void SetTarget(Transform target)
        {
            targetSelector.Deselect();
            targetSelector.Select(target);
            nmAgent.SetDestination(target.position);
        }

        private FollowLoop followLoop;

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
            private float lostDistance = 3.5f;

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

            private IEnumerator Follow()
            {
                yield return new WaitUntil(() => nmAgent.pathPending == false);

                while (nmAgent.remainingDistance > 0.05f && nmAgent.remainingDistance < lostDistance)
                {
                    yield return null;
                }

                if (nmAgent.remainingDistance > lostDistance)
                {
                    enemyMovement.LoseTrack(followTraget.position);
                }
            }
        }

        private InspectLoop inspectLoop;

        class InspectLoop : BehaviourLoop
        {
            public InspectLoop(EnemyMovement enemyMovement, Vector3 inspectPosition, NavMeshAgent nmAgent) : base(enemyMovement)
            {
                this.nmAgent = nmAgent;
                insPosition = inspectPosition;

                inspectTargetGO = new GameObject("Inspection Target");
                inspectTargetGO.transform.position = insPosition;
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

        abstract class BehaviourLoop
        {
            public BehaviourLoop(EnemyMovement enemyMovement)
            {
                this.enemyMovement = enemyMovement;
            }

            protected readonly EnemyMovement enemyMovement;

            public abstract void Start();
            public abstract void Stop();

            protected IEnumerator LookAround()
            {
                float initialYRotation = enemyMovement.transform.localEulerAngles.y;
                for (float i = 0; i < 1; i += 0.00225f)
                {
                    enemyMovement.transform.localEulerAngles = new Vector3(0, initialYRotation + enemyMovement.lookBehaviourCurve.Evaluate(i) * 360, 0);
                    yield return null;
                }

                enemyMovement.transform.localEulerAngles = new Vector3(0, initialYRotation, 0);
            }
        }
    }
}