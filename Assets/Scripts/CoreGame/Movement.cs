using UnityEngine;

namespace TeamFourteen.CoreGame
{
    /// <summary>
    /// Responsible for basic Actor movement.
    /// </summary>
    public abstract class Movement : MonoBehaviour
    {
        // not const because taken from serialized data at start of game.
        private float initialMoveSpeed;
        private float slowdownPercentage = 0.65f;
        private float SlowMoveSpeed => initialMoveSpeed * slowdownPercentage;

        [SerializeField] private float moveSpeed;
        protected float MoveSpeed => moveSpeed;

        // controls selection behaviour
        private ActorSelectionHandler _aSelectionHandler;

        private void SetMovementFields()
        {
            initialMoveSpeed = moveSpeed;
        }

        // make protected virtual?
        private void Awake()
        {
            // set move speed fields based on serialized data on game start
            SetMovementFields();

            _aSelectionHandler = new ActorSelectionHandler(OnPickup, OnRelease);
        }

        protected virtual void Start()
        {
            // if HoldObject (or other selection handler container exists)
            if (TryGetComponent(out ISelectionHandlerContainer<IPickupable> handlerContainer))
            {
                // subscribe with our event holder
                handlerContainer.Subscribe(_aSelectionHandler);
            }
        }

        private void OnPickup(IPickupable pickupable)
        {
            if (pickupable is Lantern)
                UpdateMoveSpeed(SlowMoveSpeed);
        }

        private void OnRelease(IPickupable pickupable)
        {
            if (pickupable is Lantern)
                UpdateMoveSpeed(initialMoveSpeed);
        }

        // optimally, should change to state machine where different states represent different move speeds
        private void UpdateMoveSpeed(float speed)
        {
            moveSpeed = speed;
        }

        /// <summary>
        /// <see cref="ISelectionHandler{T}"/> which contains the methods for an Actor on <see cref="IPickupable"/> selection.
        /// </summary>
        class ActorSelectionHandler : ISelectionHandler<IPickupable>
        {
            private readonly OnSelectEvent<IPickupable> _onSelect;
            private readonly OnSelectEvent<IPickupable> _onDeselect;

            public ActorSelectionHandler(OnSelectEvent<IPickupable> OnSelect, OnSelectEvent<IPickupable> OnDeselect)
            {
                _onSelect = OnSelect;
                _onDeselect = OnDeselect;
            }

            public OnSelectEvent<IPickupable> OnSelect => _onSelect;
            public OnSelectEvent<IPickupable> OnDeselect => _onDeselect;
        }
    }
}