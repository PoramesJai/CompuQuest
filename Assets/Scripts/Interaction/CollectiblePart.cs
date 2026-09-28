using UnityEngine;
using CompuQuest.Core;

namespace CompuQuest.Interaction
{
    /// <summary>
    /// ชิ้นส่วนที่เก็บได้ เช่น ALU, Control Unit, Register, Cache Memory (ด่าน 1 Mini Game 1)
    /// หรือ SSD, GPU, Mainboard, PSU (ด่าน 2) หรืออุปกรณ์ I/O (ด่าน 3)
    /// วางสคริปต์นี้บนโมเดล 3D แต่ละชิ้น (.fbx ที่ทีม Blender ส่งมา) ที่มี Collider (Is Trigger ปิด)
    /// และตั้ง Layer เป็น "Interactable"
    /// </summary>
    public class CollectiblePart : MonoBehaviour, IInteractable
    {
        [Header("Part Info (แสดงใน Popup)")]
        public string partName = "ALU";
        [TextArea(2, 5)] public string partFunction = "หน่วยคำนวณทางคณิตศาสตร์และตรรกะของ CPU";
        public Sprite partIcon;

        [Header("Inventory Key")]
        [Tooltip("ไอดีไม่ซ้ำกันของชิ้นส่วนนี้ ใช้ตรวจสอบว่าเก็บครบหรือยัง เช่น 'part_alu'")]
        public string partId;

        private bool collected = false;

        public string GetPrompt() => collected ? "" : $"กด E เพื่อเก็บ {partName}";

        public void Interact(GameObject player)
        {
            if (collected) return;
            collected = true;

            // แจ้ง PartInventory ว่าเก็บชิ้นนี้แล้ว (ตรวจสอบครบ 4 ชิ้นหรือยัง)
            PartInventory.Instance?.CollectPart(partId);

            // เปิด Popup โมเดล 3D หมุนได้ พร้อมชื่อและหน้าที่ (ตามสเปคข้อเสนอ)
            PartInfoPopupUI.Instance?.ShowPopup(partName, partFunction, partIcon);

            // ซ่อนโมเดลออกจากฉาก (หรือจะเล่น animation ดูดหายก็ได้)
            gameObject.SetActive(false);
        }
    }
}
