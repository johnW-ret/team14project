using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace TeamFourteen.CoreGame
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private float rotateSpeed;
        [SerializeField] [HideInInspector] private Camera m_camera;
        [SerializeField] [HideInInspector] private CharacterController m_characterController;

        private Vector2 m_Rotation;
        private Vector2 m_Look;
        private Vector2 m_Move;

        public void OnMove(InputAction.CallbackContext context)
        {
            m_Move = context.ReadValue<Vector2>();
        }
        public void OnLook(InputAction.CallbackContext context)
        {
            m_Look = context.ReadValue<Vector2>();
        }

        private void Reset() => SetReferences();

        [ContextMenu("Set References")]
        private void SetReferences()
        {
            if (!m_camera)
                m_camera = GetComponentInChildren<Camera>();
            if (!m_characterController)
                m_characterController = GetComponentInChildren<CharacterController>();
        }

        private void Update()
        {
            Look(m_Look);
            Move(m_Move);
        }

        private void Move(Vector2 direction)
        {
            var scaledMoveSpeed = moveSpeed * Time.deltaTime;
            
            // For simplicity's sake, we just keep movement in a single plane here. Rotate
            // direction according to world Y rotation of player.
            
            Vector3 move = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(direction.x, 0, direction.y);
            m_characterController.Move(move * scaledMoveSpeed);
        }

        float cameraRotation;
        private void Look(Vector2 rotate)
        {
            float scaledRotateSpeed = rotateSpeed * Time.deltaTime;
            
            m_Rotation.y += rotate.x * scaledRotateSpeed;
            cameraRotation = Mathf.Clamp(cameraRotation - rotate.y * scaledRotateSpeed, -89, 89);

            transform.localEulerAngles = m_Rotation;
        }
    }
}