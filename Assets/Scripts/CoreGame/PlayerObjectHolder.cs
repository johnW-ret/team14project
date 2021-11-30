using UnityEngine;
using UnityEngine.InputSystem;
using TeamFourteen.Selection;

namespace TeamFourteen.CoreGame
{
    // replace with ObjectHolder that communicates with Player input?
    public partial class PlayerObjectHolder : ObjectHolder, ISelectionHandlerContainer<IPickupable>
    {
        private Camera _camera;
        
        private Ray selectRay;

        [Header("Values")]
        [SerializeField] private float grabDistance = 2;

        private void Start()
        {
            _camera = Camera.main;
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (pickupableContainer.Selected == null)
                {
                    if (lookingAt != null)
                        TrySelect(lookingAt);
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
