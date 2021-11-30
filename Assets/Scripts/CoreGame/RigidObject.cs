using System;
using System.Collections;
using UnityEngine;

namespace TeamFourteen.CoreGame
{
    public class RigidObject : MonoBehaviour, IOwnedPickupable
    {
        [SerializeField] private Rigidbody _rigidbody;
        private Transform _parent = null;
        private ObjectHolder objectHolder = null;
        // maybe replace with state machine
        bool held = false;

        [ContextMenu("Set References")]
        private void SetReferences()
        {
            if (!_rigidbody)
                _rigidbody = GetComponent<Rigidbody>();
        }

        private void Reset()
        {
            SetReferences();
        }

        Coroutine pickupTransition;

        public bool CanPickup => !held;

        public void Pickup(Transform holder, Action OnComplete)
        {
            held = true;

            _rigidbody.isKinematic = true;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            _parent = transform.parent;
            transform.SetParent(holder);
            holder.parent?.TryGetComponent(out objectHolder);

            pickupTransition = StartCoroutine(LerpToOrigin(0.025f, OnComplete));
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
                {
                    StopCoroutine(pickupTransition);
                    goto Exit;
                }
                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, t);
                yield return null;
            }

            transform.localPosition = Vector3.zero;

            // invoke is never called if lerp does not finish
            FinishedCallback.Invoke();

            Exit:;
            // TODO maybe callback whether lerp to origin was success
        }

        public void Release()
        {
            held = false;

            Transform holder = transform.parent;

            transform.SetParent(_parent);
            transform.position = holder.position;
            objectHolder = null;

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = false;

            _parent = null;
        }

        protected virtual bool CanReleaseTo(ObjectHolder holder) => true;

        public virtual bool RequestRelease(ObjectHolder holder)
        {
            if (CanReleaseTo(holder))
            {
                // we call holder.Release(), not Release(), because holder manages object
                // admit this isn't the greatest design. May have IPickupable defer to holder
                // for all public facing methods
                if (objectHolder != null)
                    objectHolder.Release();
                return true;
            }

            return false;
        }
    }
}
