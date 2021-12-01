using UnityEngine;

namespace TeamFourteen.CoreGame
{
    public class Health : MonoBehaviour, IFloatPublisher
    {
        private const float maxHealth = 10.0f;

        // arbitrary range
        [Range(0.0f, maxHealth)]
        [SerializeField] private float health = maxHealth;

        public OnValueUpdate<float> ValueUpdateEvent { get; set; }

        private void Update()
        {
            ValueUpdateEvent?.Invoke(health);
        }
    }
}