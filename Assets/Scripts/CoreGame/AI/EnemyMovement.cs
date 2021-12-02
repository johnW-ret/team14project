using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TeamFourteen.Selection;
using TeamFourteen.CoreGame;

namespace TeamFourteen.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public partial class EnemyMovement : Movement
    {
        public enum PatrollerType
        {
            Loop,
            FollowPath
        }

        [SerializeField] private NavMeshAgent nmAgent;
        private ObjectContainer<Transform> targetSelector = new ObjectContainer<Transform>();

        [Header("References")]
        [Tooltip("List of points that the Actor patrols between.")]
        [SerializeField] private List<PatrolPoint> patrolPoints;

        [Header("Properties")]
        [Tooltip("Defines the patrol method. Does not change behaviour during runtime.")]
        [SerializeField] private PatrollerType patrollerType;
        [SerializeField] private AnimationCurve lookBehaviourCurve;

        private const int VisionLayerMask = (int)LayerManager.Layers.Player;
        private const int longRangeLength = 10;
        private const int shortRangeLength = 3;
        // prefer getters and setters, fields do for now
        private Vector3 investigationPoint;
        private Transform followTarget;

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

        private BehaviourLoop currentLoop;

        private void InitializeStateMachine()
        {
            PatrolLoop patrolLoop = new PatrolLoop(this, patroller, nmAgent);

            // would like to embed this stuff into a new inheriting type of state machine. Alas, 時間がない
            aiProcess.SetInOutAction(() => { currentLoop = GetLoop(BehaviourState.Patrol); currentLoop.Start(); }, BehaviourState.Patrol, Core.ActionProcess<BehaviourState, EnemyAIBehaviourProcess.Command>.StateDirection.In);
            aiProcess.SetInOutAction(() => { currentLoop.Stop(); currentLoop = null; }, BehaviourState.Patrol, Core.ActionProcess<BehaviourState, EnemyAIBehaviourProcess.Command>.StateDirection.Out);

            aiProcess.SetInOutAction(() => { currentLoop = GetLoop(BehaviourState.Investigate); currentLoop.Start(); }, BehaviourState.Investigate, Core.ActionProcess<BehaviourState, EnemyAIBehaviourProcess.Command>.StateDirection.In);
            aiProcess.SetInOutAction(() => { currentLoop.Stop(); currentLoop = null; }, BehaviourState.Investigate, Core.ActionProcess<BehaviourState, EnemyAIBehaviourProcess.Command>.StateDirection.Out);

            aiProcess.SetInOutAction(() => { currentLoop = GetLoop(BehaviourState.Follow); currentLoop.Start(); }, BehaviourState.Follow, Core.ActionProcess<BehaviourState, EnemyAIBehaviourProcess.Command>.StateDirection.In);
            aiProcess.SetInOutAction(() => { currentLoop.Stop(); currentLoop = null; }, BehaviourState.Follow, Core.ActionProcess<BehaviourState, EnemyAIBehaviourProcess.Command>.StateDirection.Out);

            if (!aiProcess.TryMoveNext(EnemyAIBehaviourProcess.Command.Enable))
                Debug.LogError($"Error starting Enemy AI {gameObject.name} state machine.");
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
            }

            InitializeStateMachine();
        }

        private void Update()
        {
            // set position target each frame if gameobject is not static
            if (nmAgent.enabled && targetSelector.Selected != null && !targetSelector.Selected.gameObject.isStatic)
            {
                nmAgent.SetDestination(targetSelector.Selected.position);
            } 
        }

        private void FixedUpdate()
        {
            // try short line, then long line if we hit nothing
            switch (aiProcess.CurrentState)
            {
                default:
                case BehaviourState.Off:
                case BehaviourState.Follow:
                    return;

                case BehaviourState.Patrol:
                    if (!ShortLineOfSight())
                        LongLineOfSight();
                    break;

                // would prefer to still have long range on investigate limited by elapse of 5 second timer starting after initial notice
                // however, this is best for simplicity
                case BehaviourState.Investigate:
                    ShortLineOfSight();
                    break;
            }
        }

        private Ray GetVisionRay() => new Ray(transform.position + (1.05f * transform.forward), transform.forward);

        // compiler directives #if UNITY_EDITOR blah blah
        private void DrawRayGizmo(Ray ray, float length)
        {
            Gizmos.DrawRay(ray.origin, ray.direction * length);
        }

        private void OnDrawGizmos()
        {
            // (short ray actually gets cast first, but if drawn in other order, long gizmo will draw over all of short)

            // draw long ray
            Gizmos.color = Color.yellow;
            DrawRayGizmo(GetVisionRay(), longRangeLength);

            // draw short ray
            Gizmos.color = Color.cyan;
            DrawRayGizmo(GetVisionRay(), shortRangeLength);
        }

        RaycastHit hit;
        private bool ShortLineOfSight()
        {
            // search for short range
            Ray shortLineOfSight = GetVisionRay();
            if (Physics.Raycast(shortLineOfSight, out hit, shortRangeLength)
                && hit.transform.gameObject.layer == VisionLayerMask)
            {
                Debug.Log("Short: " + hit.transform.gameObject.name);
                return See(hit.transform);
            }

            return false;
        }

        private bool LongLineOfSight()
        {
            // search for long-range
            Ray longLineOfSight = GetVisionRay();
            if (Physics.Raycast(longLineOfSight, out hit, longRangeLength)
                && hit.transform.gameObject.layer == VisionLayerMask)   // later, only detect player if holding lantern
            {
                Debug.Log("Long: " + hit.transform.gameObject.name);
                return Notice(hit.point);
            }

            return false;
        }

        private bool TryIssueCommand(EnemyAIBehaviourProcess.Command command)
        {
            bool success = aiProcess.TryMoveNext(command);

            if (!success)
                Debug.LogError($"Error issuing {command} command on {gameObject.name}");

            return aiProcess.TryMoveNext(command);
        }

        private BehaviourLoop GetLoop(BehaviourState destinationState)
        {
            BehaviourLoop loop = null;

            switch (destinationState)
            {
                case BehaviourState.Off:
                    Debug.LogError($"Potential error on {gameObject.name}. Transitioning to {destinationState} but attempting to get new loop.");
                    return loop;

                case BehaviourState.Patrol:
                    loop = new PatrolLoop(this, patroller, nmAgent);
                    break;

                case BehaviourState.Investigate:
                    loop = new InspectLoop(this, investigationPoint, nmAgent);
                    break;

                case BehaviourState.Follow:
                    loop = new FollowLoop(this, followTarget, nmAgent);
                    break;
            }

            return loop;
        }

        private bool Notice(Vector3 inspectPosition)
        {
            investigationPoint = inspectPosition;

            return TryIssueCommand(EnemyAIBehaviourProcess.Command.Notice);
        }

        private bool See(Transform followTarget)
        {
            this.followTarget = followTarget;

            return TryIssueCommand(EnemyAIBehaviourProcess.Command.See);
        }

        private void LoseTrack(Vector3 inspectPosition)
        {
            investigationPoint = inspectPosition;

            TryIssueCommand(EnemyAIBehaviourProcess.Command.LoseTrack);
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
    }
}