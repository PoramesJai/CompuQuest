using UnityEngine;

namespace CompuQuest.Interaction
{
    /// <summary>
    /// Interface กลางสำหรับวัตถุทุกชนิดที่ผู้เล่นโต้ตอบได้ด้วยปุ่ม E
    /// (ชิ้นส่วน CPU, ชิ้นส่วนคอม, อุปกรณ์ I/O, ป้าย, ประตู ฯลฯ)
    /// ข้อ 6.4 Interaction System - ใช้ Raycast ตรวจจับ แล้วเรียก Interact()
    /// </summary>
    public interface IInteractable
    {
        /// <summary>ข้อความที่ขึ้นบนหน้าจอ เช่น "กด E เพื่อเก็บ ALU"</summary>
        string GetPrompt();

        /// <summary>ทำงานเมื่อผู้เล่นกด E ขณะมองวัตถุนี้อยู่ในระยะ</summary>
        void Interact(GameObject player);
    }
}
