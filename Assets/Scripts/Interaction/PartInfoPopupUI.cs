using UnityEngine;
using UnityEngine.UI;
using CompuQuest.Core;

namespace CompuQuest.Interaction
{
    /// <summary>
    /// Popup ที่ขึ้นเมื่อเก็บชิ้นส่วนได้ (สเปคข้อเสนอ: "Pop-up โมเดล 3D หมุนได้ พร้อมชื่อและหน้าที่สั้นๆ")
    /// วิธีทำโมเดลหมุนแบบง่าย: ใช้ RawImage + RenderTexture จากกล้องมินิที่ถ่ายโมเดล 3D ลอยอยู่
    /// (ดูวิธีตั้งค่าใน README) แล้วให้สคริปต์นี้หมุน pivot ของโมเดลนั้นเอง
    /// วางสคริปต์นี้บน Canvas Panel ที่เป็น Popup แล้วซ่อนไว้ (SetActive(false)) ตอนเริ่มเกม
    /// </summary>
    public class PartInfoPopupUI : MonoBehaviour
    {
        public static PartInfoPopupUI Instance { get; private set; }

        [Header("UI References")]
        public GameObject popupPanel;
        public TMPro.TMP_Text nameText;
        public TMPro.TMP_Text functionText;
        public Image iconImage;
        public Button closeButton;

        [Header("3D Model Rotation (ถ้าใช้ RenderTexture)")]
        [Tooltip("Transform ของโมเดล 3D ที่วางอยู่หน้ากล้องมินิใน Popup Scene")]
        public Transform modelPivot;
        public float rotationSpeed = 30f;

        private void Awake()
        {
            Instance = this;
            if (popupPanel != null) popupPanel.SetActive(false);
            if (closeButton != null) closeButton.onClick.AddListener(ClosePopup);
        }

        private void Update()
        {
            // หมุนโมเดลอัตโนมัติขณะ Popup เปิดอยู่
            if (modelPivot != null && popupPanel != null && popupPanel.activeSelf)
                modelPivot.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }

        public void ShowPopup(string title, string function, Sprite icon)
        {
            if (popupPanel == null) return;

            popupPanel.SetActive(true);
            if (nameText != null) nameText.text = title;
            if (functionText != null) functionText.text = function;
            if (iconImage != null && icon != null) iconImage.sprite = icon;

            // หยุดผู้เล่นเดินขณะดู Popup
            var player = GameObject.FindGameObjectWithTag("Player");
            player?.GetComponent<CompuQuest.Player.PlayerController>()?.SetInputLocked(true);

            Time.timeScale = 0f; // หยุดเวลาเกมชั่วคราว (ปิดได้ถ้าไม่ต้องการ)
        }

        public void ClosePopup()
        {
            if (popupPanel != null) popupPanel.SetActive(false);
            Time.timeScale = 1f;

            var player = GameObject.FindGameObjectWithTag("Player");
            player?.GetComponent<CompuQuest.Player.PlayerController>()?.SetInputLocked(false);
        }
    }
}
