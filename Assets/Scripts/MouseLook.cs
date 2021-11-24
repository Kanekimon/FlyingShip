using UnityEngine;

namespace Assets.Scripts.Player
{
    public class MouseLook : MonoBehaviour
    {

        public float mouseSensitivity = 100f;

        public Transform playerBody;

        private float xRotation = 0f;
        private float yRotation = 0f;

        public static MouseLook Instance;

        private bool _isLocked = true;


        public bool IsLocked
        {
            get { return _isLocked; }
            set { _isLocked = value; }
        }


        private void Awake()
        {
            if (Instance != null)
                Destroy(this);
            else
                Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Update is called once per frame
        void Update()
        {
            if (_isLocked)
            {
                float mouseX = NormalizeSpeed(Input.GetAxis("Mouse X"));
                float mouseY = NormalizeSpeed(Input.GetAxis("Mouse Y"));

                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, -90f, 90f);


                transform.localRotation = Quaternion.Euler(xRotation, -180, 0f);
                playerBody.Rotate(Vector3.up * mouseX);
            }
        }

        private float NormalizeSpeed(float val)
        {
            return val * mouseSensitivity * Time.deltaTime;
        }

    }
}
