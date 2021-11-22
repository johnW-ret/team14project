using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TeamFourteen.CoreGame
{
    public enum PatrollerType
    {
        Loop,
        FollowPath
    }

    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyMovement : Movement
    {
        [Header("References")]
        [Tooltip("List of points that the Actor patrols between.")]
        [SerializeField] [HideInInspector] private NavMeshAgent nmAgent;
        [SerializeField] private List<PatrolPoints> patrolPoints;

        [Header("Properties")]
        [Tooltip("Defines the patrol method. Does not change behaviour during runtime.")]
        [SerializeField] private PatrollerType patrollerType;

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
                
                aiProcess.TryMoveNext(EnemyAIBehaviourProcess.Command.Enable);
            }
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            // Changes to next target if Object is within 0.05 of the current target
            if (aiProcess.CurrentState == BehaviourState.Patrol && nmAgent.remainingDistance <= 0.05f)
                SetTarget(patroller.GetNextPoint());
        }

        private void SetTarget(Vector3 target)
        {
            nmAgent.SetDestination(target);
        }

        private class LoopPatroller : Patroller
        {
            public LoopPatroller(List<PatrolPoints> patrolPoints) : base(patrolPoints)
            {
                currentPPoint = -1;
            }

            public override Vector3 GetNextPoint()
            {
                if (++currentPPoint >= points.Count)
                    currentPPoint = 0;

                return points[currentPPoint].transform.position;
            }
        }

        private class PathPatroller : Patroller
        {
            public PathPatroller(List<PatrolPoints> patrolPoints) : base(patrolPoints)
            {
                currentPPoint = -1;
            }

            private bool forward = true;

            public override Vector3 GetNextPoint()
            {
                if (forward)
                {
                    if (++currentPPoint >= points.Count)
                    {
                        currentPPoint--;
                        forward = false;
                    }
                }
                else
                {
                    if (--currentPPoint < 0)
                    {
                        currentPPoint++;
                        forward = true;
                    }
                }

                return points[currentPPoint].transform.position;
            }
        }

        private abstract class Patroller
        {
            public Patroller(List<PatrolPoints> patrolPoints)
            {
                points = patrolPoints;
            }

            protected List<PatrolPoints> points;
            protected int currentPPoint;

            public abstract Vector3 GetNextPoint();
        }
    }
}
