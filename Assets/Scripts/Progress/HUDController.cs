using UnityEngine;
using UnityEngine.UI;
using CompuQuest.Core;

namespace CompuQuest.Progress
{
    /// <summary>
    /// UI & HUD System (ข้อ 6.4) - แสดงคะแนน, สถานะภารกิจ, และ Progress Bar แบบเรียลไทม์
    /// วางสคริปต์นี้บน Canvas HUD หลักของแต่ละฉากด่าน แล้วลาก Text/Slider เข้ามา
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("Score & Quest")]
        public TMPro.TMP_Text scoreText;
        public TMPro.TMP_Text questText;

        [Header("Progress Bar (เช่น จำนวนชิ้นส่วนที่เก็บ/ใส่ครบ)")]
        public Slider progressBar;
        public TMPro.TMP_Text progressLabel;

        private void OnEnable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnScoreChanged += UpdateScore;

            if (QuestManager.Instance != null)
                QuestManager.Instance.OnQuestUpdated += UpdateQuest;

            if (CompuQuest.Interaction.PartInventory.Instance != null)
                CompuQuest.Interaction.PartInventory.Instance.OnPartCollected += UpdateProgress;

            RefreshAll();
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnScoreChanged -= UpdateScore;

            if (QuestManager.Instance != null)
                QuestManager.Instance.OnQuestUpdated -= UpdateQuest;

            if (CompuQuest.Interaction.PartInventory.Instance != null)
                CompuQuest.Interaction.PartInventory.Instance.OnPartCollected -= UpdateProgress;
        }

        private void RefreshAll()
        {
            if (GameManager.Instance != null) UpdateScore(GameManager.Instance.totalScore);
            UpdateQuest();
        }

        private void UpdateScore(int newScore)
        {
            if (scoreText != null) scoreText.text = $"คะแนน: {newScore}";
        }

        private void UpdateQuest()
        {
            if (questText != null && QuestManager.Instance != null)
                questText.text = QuestManager.Instance.GetHudSummary();
        }

        private void UpdateProgress(string partId, int collected, int total)
        {
            if (progressBar != null)
            {
                progressBar.maxValue = total;
                progressBar.value = collected;
            }
            if (progressLabel != null)
                progressLabel.text = $"{collected}/{total}";
        }
    }
}
