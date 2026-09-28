using UnityEngine;

namespace CompuQuest.Player
{
    /// <summary>
    /// กล้องมุมมองบุคคลที่ 1 (First-person Camera) หมุนมองด้วยเมาส์
    /// เมาส์แกน X = หมุนตัวละครซ้าย/ขวา (yaw), แกน Y = เงยหน้า/ก้มหน้า (pitch)
    /// วางสคริปต์นี้บน Main Camera ที่เป็นลูกของ Player (ตั้ง Local Position ประมาณ 0, 0.7, 0
    /// หรือความสูงระดับสายตา ~1.6 เมตรจากพื้น) และลาก playerBody = Player
    /// </summary>
    public class FirstPersonLook : MonoBehaviour
    {
        [Header("Reference")]
        public Transform playerBody;

        [Header("Look Settings")]
        public float mouseSensitivity = 2f;
        public float minPitch = -80f;
        public float maxPitch = 80f;

        private float pitch;
        private bool locked = false;

        private void Start()
        {
            if (playerBody == null) playerBody = transform.parent;
            SetLocked(false); // เริ่มเกม: ซ่อนและล็อกเคอร์เซอร์เพื่อหมุนมอง
        }

        private void Update()
        {
            if (locked) return;

            // กด ESC เพื่อปล่อยเคอร์เซอร์ชั่วคราว (เช่น เปิดเมนู) กดคลิกซ้ายเพื่อกลับมาควบคุมต่อ
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if (Cursor.lockState != CursorLockMode.Locked && Input.GetMouseButtonDown(0)
                     && (UnityEngine.EventSystems.EventSystem.current == null
                         || !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (Cursor.lockState != CursorLockMode.Locked) return;

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            pitch = Mathf.Clamp(pitch - mouseY, minPitch, maxPitch);
            transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }

        /// <summary>true = ล็อกการหมุนมองและปล่อยเคอร์เซอร์ (ใช้ตอนเปิด UI) / false = กลับมาหมุนมองได้</summary>
        public void SetLocked(bool value)
        {
            locked = value;
            Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = value;
        }
    }
}
