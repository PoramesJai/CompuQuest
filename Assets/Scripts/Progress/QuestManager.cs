using System;
using System.Collections.Generic;
using UnityEngine;
using CompuQuest.Core;

namespace CompuQuest.Progress
{
    [Serializable]
    public class SideQuest
    {
        public string questName;
        [TextArea(1, 3)] public string description;
        public bool isCompleted;
        public int bonusScore = 5; // ภารกิจรองสำเร็จ = +5 คะแนน (ข้อ 6.3)
    }

    /// <summary>
    /// ระบบภารกิจ (Quest System - ข้อ 6.3)
    /// ภารกิจหลัก 1 อัน/ด่าน (เพื่อผ่านด่าน) + ภารกิจรอง 2 อัน/ด่าน (คะแนนโบนัส)
    /// วางบน GameObject "QuestManager" ต่อฉาก แล้วตั้งค่าภารกิจรองใน Inspector
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        [Header("Main Quest")]
        public string mainQuestName = "ซ่อมภายใน CPU เพื่อออกจากคอมพิวเตอร์";
        public bool mainQuestCompleted = false;

        [Header("Side Quests (ด่านละ 2 ภารกิจ)")]
        public List<SideQuest> sideQuests = new List<SideQuest>();

        public delegate void QuestUpdated();
        public event QuestUpdated OnQuestUpdated;

        public delegate void MainQuestDone();
        public event MainQuestDone OnMainQuestDone;

        private void Awake()
        {
            Instance = this;
        }

        public void CompleteMainObjective()
        {
            if (mainQuestCompleted) return;
            mainQuestCompleted = true;
            OnQuestUpdated?.Invoke();
            OnMainQuestDone?.Invoke();
        }

        public void CompleteSideQuest(string questName)
        {
            SideQuest quest = sideQuests.Find(q => q.questName == questName);
            if (quest == null || quest.isCompleted) return;

            quest.isCompleted = true;
            GameManager.Instance?.AddScore(quest.bonusScore);
            OnQuestUpdated?.Invoke();
        }

        /// <summary>ใช้แสดงบน HUD: "ภารกิจ: ซ่อมภายใน CPU (0/2 ภารกิจรอง)"</summary>
        public string GetHudSummary()
        {
            int done = sideQuests.FindAll(q => q.isCompleted).Count;
            string mainStatus = mainQuestCompleted ? "✓" : "…";
            return $"[{mainStatus}] {mainQuestName}\nภารกิจรอง: {done}/{sideQuests.Count}";
        }
    }
}
