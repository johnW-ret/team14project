using System.Collections.Generic;
using UnityEngine;

namespace TeamFourteen.AI
{
    public abstract class Patroller
    {
        public Patroller(List<PatrolPoint> patrolPoints)
        {
            points = patrolPoints;
        }

        protected List<PatrolPoint> points;
        protected int currentPPoint;

        public abstract Transform GetNextPoint();
    }
}