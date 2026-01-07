using UnityEngine;

namespace Anvil.WebBuilderPro.Demo.DisableMouseAccelerationDemo
{
    [RequireComponent(typeof(Camera))]
    public class MouseLook : MonoBehaviour
    {
        private float xRotation = 0f; // Accumulate pitch changes to apply constraints
        private Camera _camera;

        public float sensitivity = 2f;
        public float zoomSensitivity = 2f;

        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _camera = GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamp to prevent over-rotation

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Apply pitch rotation locally to the camera

            transform.parent.Rotate(Vector3.up * mouseX); // Apply yaw rotation to the parent, assuming this script is attached to a camera that's a child of the player object

            // change camera fov based on mwheel up or down
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0)
            {
                _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView - zoomSensitivity, 30, 100);
            }
            else if (scroll < 0)
            {
                _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView + zoomSensitivity, 30, 100);
            }
        }
    }
}
