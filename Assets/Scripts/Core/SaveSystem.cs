using System;
using System.IO;
using UnityEngine;

namespace CompuQuest.Core
{
    /// <summary>
    /// ข้อมูลที่ต้องการบันทึกลงไฟล์ (Progress & Save System - ข้อ 6.3)
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public int totalScore;
        public bool[] stageCompleted = new bool[3];
        public int currentStage;
        public int preTestScore = -1;   // -1 = ยังไม่ทำ
        public int postTestScore = -1;
    }

    /// <summary>
    /// บันทึก/โหลดความคืบหน้าเป็นไฟล์ JSON ที่ Application.persistentDataPath
    /// เรียกใช้แบบ static ได้จากทุกที่ เช่น SaveSystem.Save(GameManager.Instance)
    /// </summary>
    public static class SaveSystem
    {
        private static string SavePath => Path.Combine(Application.persistentDataPath, "compuquest_save.json");

        public static void Save(GameManager gm)
        {
            SaveData data = new SaveData
            {
                totalScore = gm.totalScore,
                stageCompleted = gm.stageCompleted,
                currentStage = gm.currentStage
            };

            // เก็บผล pre/post test เดิมไว้ ถ้ามีอยู่แล้ว
            SaveData existing = Load();
            if (existing != null)
            {
                data.preTestScore = existing.preTestScore;
                data.postTestScore = existing.postTestScore;
            }

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"[SaveSystem] Saved to {SavePath}");
        }

        public static void SaveTestScore(bool isPreTest, int score)
        {
            SaveData data = Load() ?? new SaveData();
            if (isPreTest) data.preTestScore = score;
            else data.postTestScore = score;

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
        }

        public static SaveData Load()
        {
            if (!File.Exists(SavePath)) return null;
            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<SaveData>(json);
        }

        public static void DeleteSave()
        {
            if (File.Exists(SavePath)) File.Delete(SavePath);
        }
    }
}
