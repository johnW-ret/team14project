using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamFourteen.CoreGame
{
    public class RemainingText : MonoBehaviour
    {
        [SerializeField] [HideInInspector] private IFloatPublisher columnnCounter;  // does not serialize for some reason. we call SetReferences() on Awake()
        [SerializeField] [HideInInspector] private TextMesh textMesh;
        private readonly Color defaultColor = Color.white;
        private readonly Color winColor = Color.green;

        [ContextMenu("Set References")]
        private void SetReferences()
        {
            if (!textMesh)
                if (!TryGetComponent(out textMesh))
                    Debug.LogWarning($"TextMesh missing from {gameObject.name}. Will not text mesh with columns remaining.");
        }

        private void Reset() => SetReferences();

        private void Update()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            int remaining = Column.NumberOfColumnsRemaining;
            int total = Column.NumberOfColumns;

            textMesh.text = string.Format("{0}/{1}", remaining, total);
            textMesh.color = Color.Lerp(defaultColor, winColor, remaining / (float)total);
        }
    }
}