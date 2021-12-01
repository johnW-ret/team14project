using UnityEngine;

namespace TeamFourteen.CoreGame
{
    public class HealthText : MonoBehaviour
    {
        [SerializeField] [HideInInspector] private IFloatPublisher health;  // does not serialize for some reason. we call SetReferences() on Awake()
        [SerializeField] [HideInInspector] private TextMesh textMesh;

        [ContextMenu("Set References")]
        private void SetReferences()
        {
            if (health == null)
                if (!GameObject.Find("Player").TryGetComponent(out health))
                    Debug.LogWarning($"Health script missing from Player. Will not update camera effects with health.");

            if (!textMesh)
                if (!TryGetComponent(out textMesh))
                    Debug.LogWarning($"TextMesh missing from {gameObject.name}. Will not text mesh with health value.");
        }

        private void Reset() => SetReferences();

        private void Awake()
        {
            SetReferences();

            if (health != null)
                health.ValueUpdateEvent += UpdateText;
        }

        private void UpdateText(float newValue)
        {
            textMesh.text = string.Format("{0:F1}", newValue);
        }
    }
}