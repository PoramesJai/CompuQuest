using UnityEngine;
using CompuQuest.Core;

namespace CompuQuest.Progress
{
    /// <summary>
    /// หน้าจอสรุปคะแนนท้ายด่าน (ข้อ 6.3 Score System: "หน้าจอสรุปผลคะแนนท้ายด่าน")
    /// เรียกใช้เมื่อ QuestManager.OnMainQuestDone ถูกยิง (ใส่ครบทุกช่อง + Boss Quiz ผ่าน)
    /// วางบน Canvas ที่ซ่อนไว้ตอนเริ่มฉาก แล้วลาก field ต่าง ๆ เข้ามา
    /// </summary>
    public class StageResultPanel : MonoBehaviour
    {
        [Header("ตั้งค่าด่านนี้")]
        public int stageIndex; // 0, 1, 2
        [Tooltip("ข้อความสรุปความรู้ที่ได้เรียนในด่านนี้ เช่น หน้าที่ของ ALU/Control Unit/...")]
        [TextArea(3, 8)] public string knowledgeSummaryText;

        [Header("UI")]
        public GameObject panel;
        public TMPro.TMP_Text scoreText;
        public TMPro.TMP_Text summaryText;

        [Header("Time Bonus (ข้อ 6.3: คะแนนโบนัสจากเวลาที่ใช้)")]
        public bool useTimeBonus = true;
        public float parTimeSeconds = 300f; // เวลาที่ "มาตรฐาน" สำหรับด่านนี้
        public int maxTimeBonus = 20;

        private float stageStartTime;

        private void OnEnable()
        {
            stageStartTime = Time.time;
            if (panel != null) panel.SetActive(false);

            if (QuestManager.Instance != null)
                QuestManager.Instance.OnMainQuestDone += ShowResult;
        }

        private void OnDisable()
        {
            if (QuestManager.Instance != null)
                QuestManager.Instance.OnMainQuestDone -= ShowResult;
        }

        private void ShowResult()
        {
            if (useTimeBonus)
            {
                float elapsed = Time.time - stageStartTime;
                int bonus = Mathf.RoundToInt(Mathf.Clamp01((parTimeSeconds - elapsed) / parTimeSeconds) * maxTimeBonus);
                if (bonus > 0) GameManager.Instance?.AddScore(bonus);
            }

            GameManager.Instance?.CompleteStage(stageIndex);

            if (panel != null) panel.SetActive(true);
            if (scoreText != null) scoreText.text = $"คะแนนรวม: {GameManager.Instance?.totalScore}";
            if (summaryText != null) summaryText.text = knowledgeSummaryText;

            Time.timeScale = 0f;
        }

        /// <summary>ปุ่ม "ไปต่อ" บนหน้าจอสรุปผล</summary>
        public void OnContinuePressed()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene("StageSelect");
        }
    }
}
