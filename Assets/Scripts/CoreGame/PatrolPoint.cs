using UnityEngine;

namespace TeamFourteen.CoreGame
{
    public class PatrolPoint : MonoBehaviour
    {
        //Adjust the radius of the Gizmos of the given Game Object
        [SerializeField] private float PointRadius = 0.5f;

        public virtual void OnDrawGizmos()
        {
            //Color and draws a sphere at the location of the Game Object
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, PointRadius);
        }
    }
}