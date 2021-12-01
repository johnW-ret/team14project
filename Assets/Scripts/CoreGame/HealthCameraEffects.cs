using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_POST_PROCESSING_STACK_V2
//using UnityEngine.Rendering.PostProcessing;

#endif

// new namespace?
namespace TeamFourteen.CoreGame
{
    public class HealthCameraEffects : MonoBehaviour
    {
        // bad bad bad, interact through another interface
        [SerializeField] [HideInInspector] private Health health;
        //[SerializeField] [HideInInspector] private 

        [ContextMenu("Set References")]
        private void SetReferences()
        {
            if (!health)
                if (!GameObject.Find("Player").TryGetComponent(out health))
                    Debug.LogWarning($"Health script missing from Player. Will not update camera effects with health.");
        }

        private void Reset() => SetReferences();

        private void UpdateCameraEffects()
        {

        }
    }
}