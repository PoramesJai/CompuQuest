using System.Collections.Generic;
using UnityEngine;

namespace CompuQuest.Interaction
{
    /// <summary>
    /// เก็บรายการชิ้นส่วนที่ผู้เล่นเก็บได้แล้วในด่านปัจจุบัน (ใช้ตรวจว่าเก็บครบ 4 ชิ้นหรือยัง)
    /// วางบน GameObject "PartInventory" ในแต่ละฉากด่าน (ไม่ต้อง DontDestroyOnLoad
    /// เพราะแต่ละด่านมีชุดชิ้นส่วนต่างกัน)
    /// </summary>
    public class PartInventory : MonoBehaviour
    {
        public static PartInventory Instance { get; private set; }

        [Tooltip("จำนวนชิ้นทั้งหมดที่ต้องเก็บให้ครบในด่านนี้ เช่น 4")]
        public int requiredPartCount = 4;

        private readonly HashSet<string> collectedParts = new HashSet<string>();

        public delegate void AllPartsCollected();
        public event AllPartsCollected OnAllPartsCollected;

        public delegate void PartCollected(string partId, int collectedCount, int total);
        public event PartCollected OnPartCollected;

        private void Awake()
        {
            Instance = this;
        }

        public void CollectPart(string partId)
        {
            if (string.IsNullOrEmpty(partId) || collectedParts.Contains(partId)) return;

            collectedParts.Add(partId);
            OnPartCollected?.Invoke(partId, collectedParts.Count, requiredPartCount);

            if (collectedParts.Count >= requiredPartCount)
                OnAllPartsCollected?.Invoke();
        }

        public bool HasPart(string partId) => collectedParts.Contains(partId);

        public bool IsComplete() => collectedParts.Count >= requiredPartCount;

        public int CollectedCount => collectedParts.Count;
    }
}
