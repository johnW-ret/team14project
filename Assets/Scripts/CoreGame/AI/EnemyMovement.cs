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

        [Header("References")]
        [Tooltip("List of points that the Actor patrols between.")]
        [SerializeField] [HideInInspector] private NavMeshAgent nmAgent;
        private ObjectContainer<Transform> targetSelector = new ObjectContainer<Transform>();

        [SerializeField] private List<PatrolPoints> patrolPoints;
        [SerializeField] [HideInInspector] private Transform root;

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
            if (!root)
                root = transform.GetChild(0);
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

                aiProcess.TryMoveNext(EnemyAIBehaviourProcess.Command.Enable);
            }
        }

        private void Update()
        {
            // set position target each frame if gameobject is not static
            if (!targetSelector.Selected?.gameObject.isStatic ?? false)
            {
                nmAgent.SetDestination(targetSelector.Selected.position);
            }

            // DEBUG, temporary
            // Press "I" on the keyboard to exit the Patrol state and move to the Notice state.
            if (UnityEngine.InputSystem.Keyboard.current.iKey.wasPressedThisFrame)
                aiProcess.TryMoveNext(EnemyAIBehaviourProcess.Command.Notice);
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

        class PatrolLoop : BehaviourLoop
        {
            public PatrolLoop(EnemyMovement enemyMovement, Patroller patroller, NavMeshAgent nmAgent)
            {
                this.enemyMovement = enemyMovement;
                this.nmAgent = nmAgent;
                this.patroller = patroller;
            }

            private EnemyMovement enemyMovement;
            private NavMeshAgent nmAgent;
            private Patroller patroller;
            private Coroutine patrol;

            public override void Start()
            {
                patrol = enemyMovement.StartCoroutine(Patrol());
            }

            public override void Stop()
            {
                enemyMovement.StopCoroutine(patrol);
                nmAgent.enabled = true;
            }

            private IEnumerator Patrol()
            {
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

            private IEnumerator LookAround()
            {
                float initialYRotation = enemyMovement.transform.localEulerAngles.y;
                for (float i = 0; i < 1; i += 0.002f)
                {
                    enemyMovement.transform.localEulerAngles = new Vector3(0, initialYRotation + enemyMovement.lookBehaviourCurve.Evaluate(i) * 360, 0);
                    yield return null;
                }

                enemyMovement.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }

        abstract class BehaviourLoop
        {
            public abstract void Start();
            public abstract void Stop();
        }
    }
}