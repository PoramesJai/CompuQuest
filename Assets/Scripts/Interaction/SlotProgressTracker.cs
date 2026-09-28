using UnityEngine;

namespace CompuQuest.Interaction
{
    /// <summary>
    /// ติดตามว่าใส่ครบทุกช่องแล้วหรือยัง (เช่น "ใส่ครบ 4 ช่อง -> ประตูทางออกเปิด")
    /// วางบน GameObject กลางฉาก แล้วลาก SlotSocket ทั้งหมดในฉากใส่ array
    /// </summary>
    public class SlotProgressTracker : MonoBehaviour
    {
        public static SlotProgressTracker Instance { get; private set; }

        public SlotSocket[] allSlots;

        [Header("เหตุการณ์เมื่อใส่ครบทุกช่อง")]
        public GameObject doorObject;          // ประตูที่จะเปิด
        public Animator doorAnimator;          // ถ้าประตูมี Animation "Open"
        public GameObject lightsToTurnOn;      // เช่น ไฟในเคสคอมที่สว่างขึ้นตอนด่าน 2

        private void Awake()
        {
            Instance = this;
        }

        public void NotifySlotFilled(SlotSocket slot)
        {
            foreach (var s in allSlots)
            {
                if (!s.IsFilled) return; // ยังใส่ไม่ครบ
            }

            OnAllSlotsFilled();
        }

        private void OnAllSlotsFilled()
        {
            CompuQuest.Core.AudioManager.Instance?.PlayDoorOpen();

            if (doorAnimator != null) doorAnimator.SetTrigger("Open");
            else if (doorObject != null) doorObject.SetActive(false);

            if (lightsToTurnOn != null) lightsToTurnOn.SetActive(true);

            // แจ้ง QuestManager ว่าภารกิจหลักของ mini game นี้สำเร็จแล้ว
            CompuQuest.Progress.QuestManager.Instance?.CompleteMainObjective();
        }
    }
}
