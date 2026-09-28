using System.Collections.Generic;
using UnityEngine;

namespace CompuQuest.Quiz
{
    /// <summary>
    /// ระบบคำใบ้และข้อมูลย้อนกลับ (Hint & Feedback System - ข้อ 6.3)
    /// ผิดครั้งที่ 1 = คำใบ้บางส่วน / ครั้งที่ 2 = คำอธิบายสั้น+ภาพ / ครั้งที่ 3 = เฉลยเต็ม
    /// ใช้ได้ทั้งกับ Quiz (ตอบผิดข้อ) และ SlotSocket (ใส่ผิดช่อง)
    /// วางบน GameObject เดียวกับ GameManager หรือแยกเป็น Object ใหม่ก็ได้ (Singleton)
    /// </summary>
    public class HintFeedbackController : MonoBehaviour
    {
        public static HintFeedbackController Instance { get; private set; }

        // นับจำนวนครั้งที่ตอบผิด แยกตามคีย์ (เช่น partId หรือ questionId)
        private readonly Dictionary<string, int> wrongAttempts = new Dictionary<string, int>();

        [Header("UI (ลาก Panel/Text ที่ใช้โชว์ Hint)")]
        public GameObject hintPanel;
        public TMPro.TMP_Text hintText;
        public UnityEngine.UI.Image hintImage;

        private void Awake()
        {
            Instance = this;
            if (hintPanel != null) hintPanel.SetActive(false);
        }

        /// <summary>
        /// เรียกเมื่อผู้เล่นตอบผิดหรือใส่ผิดช่อง ระบบจะเพิ่ม level คำใบ้ให้อัตโนมัติ
        /// </summary>
        public void RegisterWrongAttempt(string correctKey, string wrongGivenKey)
        {
            if (!wrongAttempts.ContainsKey(correctKey)) wrongAttempts[correctKey] = 0;
            wrongAttempts[correctKey]++;

            int level = Mathf.Min(wrongAttempts[correctKey], 3);
            ShowHintForLevel(correctKey, level);
        }

        /// <summary>เรียกจาก QuizManager เมื่อตอบคำถามข้อใดข้อหนึ่งผิด</summary>
        public void RegisterWrongAnswer(QuestionSO question)
        {
            RegisterWrongAttempt(question.name, "");

            int level = wrongAttempts[question.name];
            string message = level switch
            {
                1 => "ลองใหม่อีกครั้ง ลองสังเกตหน้าที่หลักของอุปกรณ์นี้ดูนะ",
                2 => question.explanation,           // คำอธิบายสั้นพร้อม (ควรมีภาพประกอบใน UI เอง)
                _ => $"เฉลย: {question.choices[question.correctIndex]}\n{question.explanation}",
            };

            DisplayHint(message);
        }

        private void ShowHintForLevel(string key, int level)
        {
            string message = level switch
            {
                1 => "ลองสังเกตรูปทรงหรือหน้าที่ของชิ้นส่วนนี้อีกครั้ง",
                2 => "คำใบ้: ลองเทียบตำแหน่งกับเบ้าที่มีรูปทรงใกล้เคียงที่สุด",
                _ => "เฉลย: ลองสังเกตป้ายชื่อใต้ช่องเสียบ แล้วจับคู่ให้ตรงกัน",
            };
            DisplayHint(message);
        }

        private void DisplayHint(string message)
        {
            if (hintPanel == null || hintText == null) return;

            hintPanel.SetActive(true);
            hintText.text = message;
            CancelInvoke(nameof(HideHint));
            Invoke(nameof(HideHint), 4f); // ซ่อนอัตโนมัติหลัง 4 วินาที
        }

        private void HideHint()
        {
            if (hintPanel != null) hintPanel.SetActive(false);
        }

        public void ResetAttempts(string key)
        {
            if (wrongAttempts.ContainsKey(key)) wrongAttempts[key] = 0;
        }
    }
}
