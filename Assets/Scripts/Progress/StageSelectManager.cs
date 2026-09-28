using UnityEngine;
using UnityEngine.UI;
using CompuQuest.Core;

namespace CompuQuest.Progress
{
    [System.Serializable]
    public class StageButtonEntry
    {
        public Button button;
        public GameObject lockIcon;
        public GameObject completedIcon;
        public string sceneName;
        public int stageIndex; // 0, 1, 2
    }

    /// <summary>
    /// เมนูเลือกด่าน (Stage Selection - ข้อ 6.4)
    /// แสดงสถานะ: ผ่านแล้ว / ยังไม่ผ่าน / ล็อคด่าน ตามข้อมูลใน GameManager
    /// วางสคริปต์นี้บน Canvas ฉากเมนูเลือกด่าน แล้วตั้งค่า stageButtons ทั้ง 3 ด่านใน Inspector
    /// </summary>
    public class StageSelectManager : MonoBehaviour
    {
        public StageButtonEntry[] stageButtons;

        private void Start()
        {
            RefreshButtons();
        }

        private void RefreshButtons()
        {
            for (int i = 0; i < stageButtons.Length; i++)
            {
                StageButtonEntry entry = stageButtons[i];
                bool unlocked = GameManager.Instance == null || GameManager.Instance.IsStageUnlocked(entry.stageIndex);
                bool completed = GameManager.Instance != null && GameManager.Instance.stageCompleted[entry.stageIndex];

                entry.button.interactable = unlocked;
                if (entry.lockIcon != null) entry.lockIcon.SetActive(!unlocked);
                if (entry.completedIcon != null) entry.completedIcon.SetActive(completed);

                string sceneToLoad = entry.sceneName;
                int stageIdx = entry.stageIndex;
                entry.button.onClick.RemoveAllListeners();
                entry.button.onClick.AddListener(() =>
                {
                    AudioManager.Instance?.PlayUiClick();
                    GameManager.Instance?.LoadStageScene(sceneToLoad, stageIdx);
                });
            }
        }
    }
}
