using UnityEngine;

namespace CompuQuest.Interaction
{
    /// <summary>
    /// ชิ้นส่วนที่ผู้เล่นหยิบแล้วนำไปใส่ในช่อง (ทำงานคู่กับ SlotSocket + PlacementController)
    /// วางสคริปต์นี้บนโมเดลชิ้นส่วนหลังจากผู้เล่นเก็บเข้ากระเป๋าแล้ว หรือบนโมเดลที่วางอยู่ในฉากรอให้หยิบ
    /// </summary>
    public class DraggablePart : MonoBehaviour
    {
        [Tooltip("ต้องตรงกับ SlotSocket.acceptedPartId ของช่องที่ถูกต้อง")]
        public string partId;

        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private bool isLocked = false;

        private void Start()
        {
            originalPosition = transform.position;
            originalRotation = transform.rotation;
        }

        public void SetLocked(bool locked)
        {
            isLocked = locked;
            // ปิด Collider หรือ physics เพิ่มเติมได้ตามต้องการเมื่อ locked = true
        }

        public bool IsLocked => isLocked;

        /// <summary>"ใส่ผิด → เด้งกลับ" ตามสเปคข้อเสนอ</summary>
        public void ReturnToOriginalPosition()
        {
            if (isLocked) return;
            transform.position = originalPosition;
            transform.rotation = originalRotation;
        }
    }
}
