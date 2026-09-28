using UnityEngine;
using CompuQuest.Interaction;
using CompuQuest.Core;

namespace CompuQuest.Player
{
    /// <summary>
    /// ตรวจจับวัตถุที่ผู้เล่นมองอยู่ด้วย Raycast จากกล้อง (ข้อ 6.4 Interaction System)
    /// กด E = Interact() (เก็บของ/เปิดประตู/อ่านป้าย)
    /// กด F = หยิบ/วางชิ้นส่วนที่ถืออยู่ลงช่องว่าง (ใช้คู่กับ PlacementController)
    /// วางสคริปต์นี้บนกล้อง หรือบนตัวผู้เล่นแล้วลาก interactionCamera เข้ามา
    /// </summary>
    public class PlayerInteractor : MonoBehaviour
    {
        [Header("Raycast Settings")]
        public Camera interactionCamera;
        public float interactRange = 3f;
        public LayerMask interactableLayer;

        [Header("UI (ลาก Text/TMP ที่โชว์ prompt เช่น 'กด E เพื่อเก็บ ALU')")]
        public GameObject promptPanel;
        public TMPro.TMP_Text promptText;

        private IInteractable currentTarget;

        private void Update()
        {
            DetectInteractable();

            if (currentTarget != null && Input.GetKeyDown(KeyCode.E))
            {
                currentTarget.Interact(gameObject);
                AudioManager.Instance?.PlayPickup();
            }
        }

        private void DetectInteractable()
        {
            if (interactionCamera == null) interactionCamera = Camera.main;

            Ray ray = new Ray(interactionCamera.transform.position, interactionCamera.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    currentTarget = interactable;
                    ShowPrompt(interactable.GetPrompt());
                    return;
                }
            }

            currentTarget = null;
            HidePrompt();
        }

        private void ShowPrompt(string text)
        {
            if (promptPanel != null) promptPanel.SetActive(true);
            if (promptText != null) promptText.text = text;
        }

        private void HidePrompt()
        {
            if (promptPanel != null) promptPanel.SetActive(false);
        }
    }
}
