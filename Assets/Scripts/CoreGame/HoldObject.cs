using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TeamFourteen.CoreGame
{
    public partial class HoldObject : MonoBehaviour
    {
        [SerializeField] private Transform objectHolderTransform;
        private Camera _camera;
        private ObjectContainer<IPickupable> pickupableContainer = new ObjectContainer<IPickupable>();
        private bool doSelect;

        private void Start()
        {
            _camera = Camera.main;
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.started)
                doSelect = context.ReadValueAsButton();
        }

        RaycastHit raycastHit;
        private void Update()
        {
            // if we are not holding anything
            if (pickupableContainer.Selected == null)
            {
                // if we hit something
                if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out raycastHit))
                {
                    // if said thing is a pickupable
                    if (raycastHit.transform.TryGetComponent<IPickupable>(out IPickupable pickupable))
                    {
                        if (doSelect)
                            pickupableContainer.Select(pickupable);
                    }
                }
            }
            else
            {
                if (doSelect)
                    pickupableContainer.Deselect();
            }

            Debug.Log(pickupableContainer.Selected?.ToString() ?? "None");
        }
    }
}
