using UnityEngine;

namespace CampusNavigator
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        public float walkSpeed = 4f;
        public float sprintSpeed = 7f;
        public float gravity = -18f;
        public float mouseSensitivity = 2f;
        public Transform cameraPivot;

        private CharacterController controller;
        private float verticalVelocity;
        private float yaw;
        private float pitch;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (cameraPivot == null)
            {
                cameraPivot = Camera.main != null ? Camera.main.transform : null;
            }
        }

        private void Update()
        {
            HandleMove();
            HandleLook();
        }

        private void HandleMove()
        {
            float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * speed * Time.deltaTime);

            if (controller.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }
            verticalVelocity += gravity * Time.deltaTime;
            controller.Move(new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);
        }

        private void HandleLook()
        {
            if (cameraPivot == null)
            {
                return;
            }

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, -75f, 75f);

            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }
}
