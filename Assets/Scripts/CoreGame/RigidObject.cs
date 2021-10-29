using System;
using System.Collections;
using UnityEngine;

namespace TeamFourteen.CoreGame
{
    public class RigidObject : MonoBehaviour, IPickupable
    {
        // replace with class to manage null/not null data (similar to or same as ObjectContainer class)
        private Transform _parent;

        public void Pickup(Transform holder, Action OnComplete)
        {
            _parent = transform.parent;
            transform.SetParent(holder);

            StartCoroutine(LerpToTransform(holder, 0.025f, OnComplete));
        }

        /// <summary>
        /// Lerps to transform position by <paramref name="deltaT"/>.
        /// </summary>
        /// <param name="otherTransform">The transform to lerp position to.</param>
        /// <param name="deltaT">The rate per interation with which to lerp.</param>
        /// <param name="FinishedCallback">The event to call upon lerp completion <paramref name="deltaT"/> >= 1.</param>
        /// <returns></returns>
        private IEnumerator LerpToTransform(Transform otherTransform, float deltaT, Action FinishedCallback)
        {
            for (float t = 0; t <= 1; t += deltaT)
            {
                // replace with Rigidbody.position?
                transform.position = Vector3.Lerp(transform.position, otherTransform.position, t);
                yield return null;
            }

            FinishedCallback.Invoke();
        }

        public void Release()
        {
            Debug.Log("Release");
            transform.SetParent(_parent);
            _parent = null;
        }
    }
}
