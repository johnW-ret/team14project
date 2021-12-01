using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamFourteen.Selection;

namespace TeamFourteen.CoreGame
{
    public abstract class ObjectHolder : MonoBehaviour, ISelectionHandlerContainer<IPickupable>
    {
        [Header("Object References")]
        [SerializeField] private Transform objectHolderTransform;

        protected ActionObjectContainer<IPickupable> pickupableContainer;

        protected virtual void SetReferences()
        {
            if (!transform.Find("Object Holder"))
                Debug.LogWarning($"Cannot find Object Holder on {gameObject.name}. Object holder will not be assigned");
            else 
                objectHolderTransform = transform.Find("Object Holder");
        }

        private void Reset() => SetReferences();

        private void Awake()
        {
            pickupableContainer = new ActionObjectContainer<IPickupable>(TryPickup, TryRelease);
        }

        public bool Subscribe(ISelectionHandler<IPickupable> handler)
        {
            return pickupableContainer.Subscribe(handler);
        }

        private void TryPickup(IPickupable pickupable)
        {
            if (pickupable != null)
                pickupable.Pickup(objectHolderTransform, OnPickupComplete);
            else
                Debug.LogWarning($"Attempted to pickup null object on {gameObject.name}");
        }

        private void TryRelease(IPickupable pickupable)
        {
            if (pickupable != null)
                pickupable.Release();
            else
                Debug.LogWarning($"Attempted to release null object on {gameObject.name}");
        }

        protected virtual void OnPickupComplete()
        {
            // do thing with state machine
        }

        protected void TrySelect(IPickupable pickupable)
        {
            if (pickupable is IOwnedPickupable ownedPickupable)
            {
                if (ownedPickupable.CanPickup || ownedPickupable.RequestRelease(this))
                    pickupableContainer.Select(ownedPickupable);
            }
        }

        public void Release()
        {
            pickupableContainer.Deselect();
        }
    }
}
