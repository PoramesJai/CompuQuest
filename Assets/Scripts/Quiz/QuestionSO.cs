using UnityEngine;

namespace CompuQuest.Quiz
{
    /// <summary>
    /// ข้อมูลคำถามแบบ ScriptableObject - สร้างเป็นไฟล์ Asset แยกต่อข้อ
    /// วิธีสร้าง: คลิกขวาใน Project window > Create > CompuQuest > Question
    /// ใช้สำหรับ Boss Quiz ท้ายด่าน และ Pre-test/Post-test (10 ข้อ ตามข้อเสนอข้อ 6.3)
    /// </summary>
    [CreateAssetMenu(fileName = "NewQuestion", menuName = "CompuQuest/Question")]
    public class QuestionSO : ScriptableObject
    {
        [TextArea(2, 4)] public string questionText;

        [Tooltip("ตัวเลือกคำตอบ (แนะนำ 4 ตัวเลือก)")]
        public string[] choices = new string[4];

        [Tooltip("index ของคำตอบที่ถูกต้องใน choices (เริ่มที่ 0)")]
        public int correctIndex;

        [TextArea(2, 4)]
        [Tooltip("คำอธิบายเฉลย แสดงตอนตอบผิด/หลังตอบ (ใช้กับ Hint & Feedback System)")]
        public string explanation;

        [Tooltip("หมวดหมู่ เช่น 'ด่าน1_ALU', 'PreTest', 'PostTest' เพื่อกรองชุดคำถาม")]
        public string category;
    }
}
