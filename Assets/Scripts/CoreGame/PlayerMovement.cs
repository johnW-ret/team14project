using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TeamFourteen.CoreGame
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private float hRotateSpeed;
        [SerializeField] private float vRotateSpeed;
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

        float yVelocity;
        private void Move(Vector2 direction)
        {
            float scaledMoveSpeed = moveSpeed * Time.deltaTime;

            // calculate Y velocity
            if (m_characterController.isGrounded)
                yVelocity = 0;
            else
                yVelocity += Physics.gravity.y * Time.deltaTime;

            // translate X and Z based on movement input
            Vector3 move = Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(direction.x, yVelocity, direction.y);
            m_characterController.Move(move * scaledMoveSpeed);
        }

        float cameraRotation;
        private void Look(Vector2 rotate)
        {
            // rotate the player body along the Y axis
            m_Rotation.y += rotate.x * hRotateSpeed * Time.deltaTime;
            transform.localEulerAngles = m_Rotation;

            // rotate the camera along the X axis
            cameraRotation = Mathf.Clamp(cameraRotation - rotate.y * vRotateSpeed * Time.deltaTime, -89, 89);
            m_camera.transform.localEulerAngles = new Vector2(cameraRotation, 0);
        }
    }
}