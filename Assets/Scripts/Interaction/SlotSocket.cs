using UnityEngine;
using CompuQuest.Core;

namespace CompuQuest.Interaction
{
    /// <summary>
    /// ช่องว่างสำหรับใส่ชิ้นส่วน (Mini Game 2 ทุกด่าน: ใส่ CPU ลงช่อง / ประกอบเคสคอม / เสียบ I/O)
    /// แต่ละช่องรับได้เฉพาะ acceptedPartId ที่ตรงกันเท่านั้น
    /// วางสคริปต์นี้บนโมเดล "ช่องว่าง/เบ้า" แต่ละช่อง ตั้ง Collider เป็น Is Trigger = true
    /// </summary>
    public class SlotSocket : MonoBehaviour
    {
        [Tooltip("ไอดีของชิ้นส่วนที่ช่องนี้รับได้ ต้องตรงกับ DraggablePart.partId เช่น 'part_alu'")]
        public string acceptedPartId;

        [Tooltip("ตำแหน่งที่ชิ้นส่วนจะไปล็อกอยู่พอดี (ปกติคือตำแหน่งของช่องนี้เอง)")]
        public Transform snapPoint;

        [Header("Visual Feedback")]
        public GameObject correctGlowEffect;
        public ParticleSystem lockInParticles;

        public bool IsFilled { get; private set; } = false;

        private void Awake()
        {
            if (snapPoint == null) snapPoint = transform;
        }

        /// <summary>เรียกจาก PlacementController เมื่อผู้เล่นพยายามใส่ชิ้นส่วนลงช่องนี้</summary>
        public bool TryPlacePart(DraggablePart part)
        {
            if (IsFilled) return false;

            bool correct = part.partId == acceptedPartId;

            if (correct)
            {
                LockPartInPlace(part);
                AudioManager.Instance?.PlayLockIn();
                if (correctGlowEffect != null) correctGlowEffect.SetActive(true);
                lockInParticles?.Play();
                IsFilled = true;
                SlotProgressTracker.Instance?.NotifySlotFilled(this);
            }
            else
            {
                AudioManager.Instance?.PlayWrong();
                HintFeedbackController.Instance?.RegisterWrongAttempt(acceptedPartId, part.partId);
                part.ReturnToOriginalPosition(); // "ใส่ผิด → เด้งกลับ"
            }

            return correct;
        }

        private void LockPartInPlace(DraggablePart part)
        {
            part.transform.position = snapPoint.position;
            part.transform.rotation = snapPoint.rotation;
            part.SetLocked(true);
        }
    }
}
