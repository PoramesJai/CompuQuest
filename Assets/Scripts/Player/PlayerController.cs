using UnityEngine;

namespace CompuQuest.Player
{
    /// <summary>
    /// ระบบควบคุมตัวละครมุมมองบุคคลที่ 1 (First-person Player Controller)
    /// ใช้ CharacterController ของ Unity
    /// ปุ่ม: W/A/S/D เดิน, Shift วิ่ง, Space กระโดด, เมาส์หมุนมอง (จัดการโดย FirstPersonLook)
    /// วางสคริปต์นี้บน GameObject "Player" (Tag = Player) ที่มี Character Controller
    /// และมีกล้อง (ติด FirstPersonLook) เป็นลูก
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }

        [Header("Movement")]
        public float moveSpeed = 4f;
        public float sprintSpeed = 6.5f;
        public float jumpHeight = 1.2f;
        public float gravity = -9.81f;

        [Header("Ground Check")]
        public Transform groundCheck;
        public float groundDistance = 0.3f;
        public LayerMask groundMask;

        [Header("Reference")]
        [Tooltip("สคริปต์หมุนมองที่ติดบนกล้อง (ลากกล้องลูกของ Player มาใส่)")]
        public FirstPersonLook look;

        private CharacterController controller;
        private Vector3 velocity;
        private bool isGrounded;

        /// <summary>true เมื่อเปิด UI/Quiz/Popup อยู่ -> หยุดเดิน/หมุนมอง และปล่อยเคอร์เซอร์เมาส์</summary>
        public bool inputLocked { get; private set; }

        private void Awake()
        {
            Instance = this;
            controller = GetComponent<CharacterController>();
            if (look == null) look = GetComponentInChildren<FirstPersonLook>();
        }

        private void Update()
        {
            if (inputLocked) return;

            HandleGroundCheck();
            HandleMovement();
            HandleJump();
            ApplyGravity();
        }

        private void HandleGroundCheck()
        {
            isGrounded = groundCheck != null
                ? Physics.CheckSphere(groundCheck.position, groundDistance, groundMask)
                : controller.isGrounded;

            if (isGrounded && velocity.y < 0f)
                velocity.y = -2f;
        }

        private void HandleMovement()
        {
            float h = Input.GetAxis("Horizontal"); // A/D
            float v = Input.GetAxis("Vertical");   // W/S

            // มุมมองบุคคลที่ 1: ตัวละครหันตามเมาส์อยู่แล้ว จึงเดินตามทิศของตัวละครโดยตรง
            Vector3 move = transform.right * h + transform.forward * v;
            if (move.sqrMagnitude > 1f) move.Normalize();

            float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
            controller.Move(move * speed * Time.deltaTime);
        }

        private void HandleJump()
        {
            if (Input.GetButtonDown("Jump") && isGrounded)
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        private void ApplyGravity()
        {
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        /// <summary>เรียกจาก UI ต่าง ๆ (Popup, Quiz, หน้าสรุปผล) เพื่อล็อก/ปลดล็อกการควบคุม</summary>
        public void SetInputLocked(bool locked)
        {
            inputLocked = locked;
            look?.SetLocked(locked); // จัดการเคอร์เซอร์เมาส์ให้อัตโนมัติ
        }
    }
}
