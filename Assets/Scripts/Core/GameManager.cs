using UnityEngine;
using UnityEngine.SceneManagement;

namespace CompuQuest.Core
{
    /// <summary>
    /// ตัวจัดการกลางของเกม (Singleton) - เก็บสถานะด่านปัจจุบัน, คะแนนรวม,
    /// สถานะการผ่าน/ไม่ผ่านของแต่ละด่าน และเชื่อมระบบอื่น ๆ เข้าด้วยกัน
    /// ผูกกับ GameObject ชื่อ "GameManager" ที่มีอยู่แค่ฉากเดียว (DontDestroyOnLoad)
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Stage State")]
        [Tooltip("ด่านปัจจุบัน: 1 = CPU, 2 = ซ่อมคอม, 3 = I/O")]
        public int currentStage = 1;

        [Tooltip("สถานะผ่านด่าน index 0..2 สำหรับด่าน 1..3")]
        public bool[] stageCompleted = new bool[3];

        [Header("Global Score")]
        public int totalScore = 0;

        public delegate void ScoreChanged(int newScore);
        public event ScoreChanged OnScoreChanged;

        public delegate void StageCompleted(int stageIndex);
        public event StageCompleted OnStageCompleted;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            SaveData data = SaveSystem.Load();
            if (data != null)
            {
                totalScore = data.totalScore;
                stageCompleted = data.stageCompleted;
                currentStage = data.currentStage;
            }
        }

        public void AddScore(int amount)
        {
            totalScore += amount;
            OnScoreChanged?.Invoke(totalScore);
        }

        public void CompleteStage(int stageIndex)
        {
            if (stageIndex < 0 || stageIndex >= stageCompleted.Length) return;

            stageCompleted[stageIndex] = true;
            OnStageCompleted?.Invoke(stageIndex);
            SaveSystem.Save(this);
        }

        public bool IsStageUnlocked(int stageIndex)
        {
            if (stageIndex == 0) return true;
            return stageCompleted[stageIndex - 1];
        }

        public void LoadStageScene(string sceneName, int stageIndex)
        {
            currentStage = stageIndex + 1;
            SceneManager.LoadScene(sceneName);
        }

        public void SaveProgress()
        {
            SaveSystem.Save(this);
        }
    }
}
