using UnityEngine;

namespace CompuQuest.Interaction
{
    /// <summary>
    /// ควบคุมการหยิบ/วางชิ้นส่วนด้วยปุ่ม F (ข้อ 6.2: "ปุ่ม F ใช้สำหรับการหยิบจับหรือวางชิ้นส่วน")
    /// วิธีทำงาน: มองไปที่ DraggablePart แล้วกด F เพื่อ "หยิบ" (ชิ้นส่วนจะลอยตามกล้อง)
    /// กด F อีกครั้งขณะมองช่อง SlotSocket เพื่อ "วาง" ลงช่องนั้น
    /// วางสคริปต์นี้บนตัวผู้เล่นหรือกล้อง (จุดเดียวกับ PlayerInteractor)
    /// </summary>
    public class PlacementController : MonoBehaviour
    {
        [Header("Raycast")]
        public Camera interactionCamera;
        public float interactRange = 3f;
        public LayerMask partLayer;
        public LayerMask slotLayer;

        [Header("Carry Settings")]
        [Tooltip("ตำแหน่งหน้ากล้องที่ชิ้นส่วนจะลอยตามระหว่างถืออยู่")]
        public Transform carryPoint;

        private DraggablePart carriedPart;

        private void Update()
        {
            if (interactionCamera == null) interactionCamera = Camera.main;

            if (carriedPart == null)
            {
                TryPickUp();
            }
            else
            {
                // ชิ้นส่วนลอยตามตำแหน่งถือ
                carriedPart.transform.position = carryPoint.position;

                if (Input.GetKeyDown(KeyCode.F))
                    TryPlaceOrDrop();
            }
        }

        private void TryPickUp()
        {
            if (!Input.GetKeyDown(KeyCode.F)) return;

            Ray ray = new Ray(interactionCamera.transform.position, interactionCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, partLayer))
            {
                DraggablePart part = hit.collider.GetComponent<DraggablePart>();
                if (part != null && !part.IsLocked)
                {
                    carriedPart = part;
                }
            }
        }

        private void TryPlaceOrDrop()
        {
            Ray ray = new Ray(interactionCamera.transform.position, interactionCamera.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, slotLayer))
            {
                SlotSocket slot = hit.collider.GetComponent<SlotSocket>();
                if (slot != null)
                {
                    slot.TryPlacePart(carriedPart);
                    carriedPart = null;
                    return;
                }
            }

            // ไม่ได้เล็งไปที่ช่องใด ๆ -> วางลงพื้นตรงหน้า (drop) แทนที่จะบังคับใส่ผิดช่อง
            carriedPart.transform.SetParent(null);
            carriedPart = null;
        }
    }
}
