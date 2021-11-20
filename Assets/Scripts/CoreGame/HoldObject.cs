﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TeamFourteen.Selection;

namespace TeamFourteen.CoreGame
{
    public partial class HoldObject : MonoBehaviour, ISelectionHandlerContainer<IPickupable>
    {
        [Header("Object References")]
        [SerializeField] private Transform objectHolderTransform;
        private Camera _camera;
        
        private ActionObjectContainer<IPickupable> pickupableContainer;
        private Ray selectRay;

        [Header("Values")]
        [SerializeField] private float grabDistance = 2;

        private void Awake()
        {
            pickupableContainer = new ActionObjectContainer<IPickupable>(
                (pickupable) => pickupable.Pickup(objectHolderTransform, OnPickupComplete),
                (pickupable) => pickupable.Release());
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        public bool Subscribe(ISelectionHandler<IPickupable> handler)
        {
            return pickupableContainer.Subscribe(handler);
        }

        private void OnPickupComplete()
        {
            // do thing with state machine
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (pickupableContainer.Selected == null)
                {
                    if (lookingAt != null)
                        pickupableContainer.Select(lookingAt);
                }
                else
                    pickupableContainer.Deselect();
            }
        }

        RaycastHit raycastHit;
        IPickupable lookingAt = null;
        private void Update()
        {
            lookingAt = null;

            // if we are not holding anything
            if (pickupableContainer.Selected == null)
            {
                selectRay = new Ray(_camera.transform.position, _camera.transform.forward);
                
                // if we hit something
                if (Physics.Raycast(selectRay, out raycastHit, grabDistance))
                {
                    // if said thing is a pickupable
                    if (raycastHit.transform.TryGetComponent<IPickupable>(out IPickupable pickupable))
                    {
                        lookingAt = pickupable;
                    }
                }
            }
        }
    }
}
