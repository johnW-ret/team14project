using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TeamFourteen.CoreGame
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyMovement : Movement
    {
        [SerializeField] [HideInInspector] private NavMeshAgent nmAgent;

        //List of Patrol Points the GameObject can move to
        [SerializeField] List<PatrolPoints> patrolPoints;

        //Patrol Points Variables
        int currentPPoint;
        bool patrolNext;
        bool travelling;

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

        private void CheckPatrolPoints()
        {
            //Checks to see if there are points in the list and that there are at least 2 of those points
            if (patrolPoints != null && patrolPoints.Count >= 2)
            {
                //Sets the first target destination
                currentPPoint = 0;
                SetTarget();
                patrolNext = true;
                travelling = true;
                //Debug.Log("Current Point in list: " + currentPPoint);
            }
            else
                Debug.Log("Issue in number of Patrol Points.");
        }

        protected override void Start()
        {
            base.Start();

            CheckPatrolPoints();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            //Changes to next target if Object is within 0.05 of the current target
            if (travelling && nmAgent.remainingDistance <= 0.05f)
            {
                travelling = false;
                ChangePoint();
                //Debug.Log("Current Point in list: " + currentPPoint);
                SetTarget();
            }
        }

        private void ChangePoint()
        {
            //Checks if the GameObject is going forward or backwards in the list of points
            if (patrolNext)
            {
                //Increments to the next point
                currentPPoint++;

                //Checks if the increment is over the list.
                if (currentPPoint >= patrolPoints.Count)
                {
                    //Sets the patrol backwards
                    currentPPoint = currentPPoint - 2;
                    patrolNext = false;
                }
            }
            else
            {
                //Moving backwards through the list of points
                currentPPoint--;

                //If increment is less than 0 sets patrol to forward position
                if (currentPPoint < 0)
                {
                    currentPPoint = 1;
                    patrolNext = true;
                }
            }
        }

        private void SetTarget()
        {
            //Checks to see if there are points in the list
            if (patrolPoints != null)
            {
                //Sets the current index of the list transform to variable target
                Vector3 target = patrolPoints[currentPPoint].transform.position;
                //Sets the GameObjects destination to target location
                nmAgent.SetDestination(target);
                travelling = true;
            }
            else
                Debug.Log("There are no patrol points in the list");
        }
    }
}
