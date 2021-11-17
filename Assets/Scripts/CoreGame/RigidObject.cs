using System;
using System.Collections;
using UnityEngine;

namespace TeamFourteen.CoreGame
{
    public class RigidObject : MonoBehaviour, IPickupable
    {
        [SerializeField] private Rigidbody _rigidbody;
        private Transform _parent = null;
        // maybe replace with state machine
        bool held = false;

        [ContextMenu("Set References")]
        private void SetReferences()
        {
            if (!_rigidbody)
                _rigidbody = GetComponent<Rigidbody>();
        }

        Coroutine pickupTransition;
        public void Pickup(Transform holder, Action OnComplete)
        {
            held = true;

            _rigidbody.isKinematic = true;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            _parent = transform.parent;
            transform.SetParent(holder);

            pickupTransition = StartCoroutine(LerpToOrigin(0.025f, OnComplete));
            pickupTransition = null;
        }

        /// <summary>
        /// Lerps to transform position by <paramref name="deltaT"/>.
        /// </summary>
        /// <param name="otherTransform">The transform to lerp position to.</param>
        /// <param name="deltaT">The rate per interation with which to lerp.</param>
        /// <param name="FinishedCallback">The event to call upon lerp completion <paramref name="deltaT"/> >= 1.</param>
        /// <returns></returns>
        private IEnumerator LerpToOrigin(float deltaT, Action FinishedCallback)
        {
            for (float t = 0; t <= 1; t += deltaT)
            {
                if (!held)
                    StopCoroutine(pickupTransition);

                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, t);
                yield return null;
            }

            transform.localPosition = Vector3.zero;

            // invoke is never called if lerp does not finish
            FinishedCallback.Invoke();
        }

        public void Release()
        {
            held = false;

            Transform holder = transform.parent;

            transform.SetParent(_parent);
            transform.position = holder.position;

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = false;

            _parent = null;
        }
    }
}
