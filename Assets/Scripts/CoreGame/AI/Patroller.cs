using System.Collections.Generic;
using UnityEngine;

namespace TeamFourteen.AI
{
    public abstract class Patroller
    {
        public Patroller(List<PatrolPoints> patrolPoints)
        {
            points = patrolPoints;
        }

        protected List<PatrolPoints> points;
        protected int currentPPoint;

        public abstract Transform GetNextPoint();
    }
}