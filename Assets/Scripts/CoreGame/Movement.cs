using UnityEngine;

namespace TeamFourteen.CoreGame
{
    public abstract class Movement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        protected float MoveSpeed => moveSpeed;
    }
}