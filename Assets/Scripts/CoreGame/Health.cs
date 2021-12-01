using UnityEngine;

namespace TeamFourteen.CoreGame
{
    public class Health : MonoBehaviour, IFloatPublisher
    {
        private const float maxHealth = 7.0f;

        // arbitrary range
        [Range(0.0f, maxHealth)]
        [SerializeField] private float health = maxHealth;
        private bool damageFlag;

        public OnValueUpdate<float> ValueUpdateEvent { get; set; }

        private void Update()
        {
            if (damageFlag)
                TakeDamage(Time.deltaTime);

            ValueUpdateEvent?.Invoke(health);
        }

        private void TakeDamage(float damage)
        {
            SetHealth(health - damage);
        }

        private void SetHealth(float amount)
        {
            health = Mathf.Clamp(amount, 0, maxHealth);

            if (health < Mathf.Epsilon)
                Die();
        }

        private void Die()
        {
            // not sure about inheritance structure and don't want to make that decision yet, so I'm leaving this here
            if (gameObject.name == "Player")
                GameManager.LoadLatestCheckpoint();
        }

        public void ResetHealth()
        {
            SetHealth(maxHealth);
        }

        private void FixedUpdate()
        {
            damageFlag = false;
        }

        // for future, change responsibility away from Health maybe
        private void OnTriggerStay(Collider other)
        {
            damageFlag = other.tag == "Damage";
        }
    }
}