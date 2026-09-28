using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using CompuQuest.Core;

namespace CompuQuest.Quiz
{
    /// <summary>
    /// ระบบทดสอบความรู้ (Boss Quiz ท้ายด่าน + Pre-test/Post-test - ข้อ 6.3)
    /// เกณฑ์ผ่าน: 3/5 ข้อ (60%) ตอบถูก +10 คะแนน/ข้อ, ตอบถูกครบ = Perfect Bonus +20
    /// คำถามสลับลำดับทุกครั้ง (Shuffle) / ตอบผิดลองใหม่ได้ไม่จำกัด
    /// วางสคริปต์นี้บน GameObject "QuizManager" ในฉาก แล้วลาก Canvas Quiz UI เข้ามา
    /// </summary>
    public class QuizManager : MonoBehaviour
    {
        public static QuizManager Instance { get; private set; }

        [Header("Question Pool")]
        [Tooltip("ลากไฟล์ QuestionSO ทั้งหมดของ Boss Quiz ด่านนี้ (หรือ Pre/Post-test 10 ข้อ)")]
        public QuestionSO[] questionPool;

        [Tooltip("จำนวนข้อที่จะสุ่มมาใช้จริง เช่น 5 สำหรับ Boss Quiz, 10 สำหรับ Pre/Post-test")]
        public int questionsToUse = 5;

        [Range(0f, 1f)] public float passThreshold = 0.6f; // 60%

        [Header("UI References")]
        public GameObject quizPanel;
        public TMPro.TMP_Text questionText;
        public Button[] choiceButtons;      // ลาก 4 ปุ่มเข้ามา
        public TMPro.TMP_Text[] choiceLabels;
        public TMPro.TMP_Text progressText; // เช่น "ข้อ 2/5"
        public GameObject resultPanel;
        public TMPro.TMP_Text resultText;

        [Header("Mode")]
        public bool isPreTest = false;
        public bool isPostTest = false;

        private List<QuestionSO> activeQuestions;
        private int currentIndex;
        private int correctCount;

        private void Awake()
        {
            Instance = this;
            if (quizPanel != null) quizPanel.SetActive(false);
            if (resultPanel != null) resultPanel.SetActive(false);
        }

        /// <summary>เรียกเพื่อเริ่มควิซ เช่น จากปุ่ม "เริ่ม Boss Quiz" หรือตอนโหลดฉาก Pre-test</summary>
        public void StartQuiz()
        {
            activeQuestions = questionPool.OrderBy(_ => Random.value) // สลับลำดับคำถามทุกครั้ง
                                           .Take(Mathf.Min(questionsToUse, questionPool.Length))
                                           .ToList();
            currentIndex = 0;
            correctCount = 0;

            quizPanel?.SetActive(true);
            resultPanel?.SetActive(false);
            ShowQuestion();
        }

        private void ShowQuestion()
        {
            QuestionSO q = activeQuestions[currentIndex];
            questionText.text = q.questionText;
            if (progressText != null) progressText.text = $"ข้อ {currentIndex + 1}/{activeQuestions.Count}";

            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (i >= q.choices.Length)
                {
                    choiceButtons[i].gameObject.SetActive(false);
                    continue;
                }

                choiceButtons[i].gameObject.SetActive(true);
                choiceLabels[i].text = q.choices[i];

                int choiceIndex = i; // ป้องกัน closure bug ใน loop
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(choiceIndex));
            }
        }

        private void OnChoiceSelected(int choiceIndex)
        {
            AudioManager.Instance?.PlayUiClick();
            QuestionSO q = activeQuestions[currentIndex];

            if (choiceIndex == q.correctIndex)
            {
                correctCount++;
                GameManager.Instance?.AddScore(10);   // ตอบถูก +10 คะแนน
                AudioManager.Instance?.PlayCorrect();
            }
            else
            {
                HintFeedbackController.Instance?.RegisterWrongAnswer(q);
                AudioManager.Instance?.PlayWrong();
            }

            currentIndex++;
            if (currentIndex < activeQuestions.Count)
                ShowQuestion();
            else
                FinishQuiz();
        }

        private void FinishQuiz()
        {
            quizPanel?.SetActive(false);

            float percent = (float)correctCount / activeQuestions.Count;
            bool passed = percent >= passThreshold;

            if (correctCount == activeQuestions.Count)
                GameManager.Instance?.AddScore(20); // Perfect Bonus +20

            if (isPreTest) SaveSystem.SaveTestScore(true, correctCount);
            if (isPostTest) SaveSystem.SaveTestScore(false, correctCount);

            resultPanel?.SetActive(true);
            if (resultText != null)
            {
                resultText.text = passed
                    ? $"ผ่าน! ตอบถูก {correctCount}/{activeQuestions.Count} ข้อ ({percent:P0})"
                    : $"ยังไม่ผ่าน ตอบถูก {correctCount}/{activeQuestions.Count} ข้อ ({percent:P0}) ลองใหม่อีกครั้ง";
            }

            if (passed && !isPreTest && !isPostTest)
                CompuQuest.Progress.QuestManager.Instance?.CompleteMainObjective();
        }

        /// <summary>ปุ่ม "ลองใหม่" เมื่อสอบไม่ผ่าน (ลองใหม่ได้ไม่จำกัดครั้งตามสเปค)</summary>
        public void RetryQuiz() => StartQuiz();
    }
}
