using System.Collections.Generic;
using UnityEngine;

namespace TeamFourteen.AI
{
    public class PathPatroller : Patroller
    {
        public PathPatroller(List<PatrolPoints> patrolPoints) : base(patrolPoints)
        {
            currentPPoint = -1;
        }

        private bool forward = true;

        public override Transform GetNextPoint()
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

            return points[currentPPoint].transform;
        }
    }
}