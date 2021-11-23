using System.Collections.Generic;
using UnityEngine;

namespace TeamFourteen.AI
{
    public class LoopPatroller : Patroller
    {
        public LoopPatroller(List<PatrolPoints> patrolPoints) : base(patrolPoints)
        {
            currentPPoint = -1;
        }

        public override Transform GetNextPoint()
        {
            if (++currentPPoint >= points.Count)
                currentPPoint = 0;

            return points[currentPPoint].transform;
        }
    }
}