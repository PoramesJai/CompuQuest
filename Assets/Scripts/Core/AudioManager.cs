using UnityEngine;

namespace CompuQuest.Core
{
    /// <summary>
    /// ระบบจัดการเสียง (Audio Management System - ข้อ 6.4)
    /// คุม BGM 1 แหล่งเสียง + SFX แบบ one-shot หลายเสียง
    /// วาง GameObject ชื่อ "AudioManager" พร้อม AudioSource สองตัว (bgmSource, sfxSource)
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Common SFX (ลากไฟล์เสียงใส่ตาม index ที่ใช้)")]
        public AudioClip correctSfx;
        public AudioClip wrongSfx;
        public AudioClip pickupSfx;
        public AudioClip lockInSfx;
        public AudioClip doorOpenSfx;
        public AudioClip uiClickSfx;

        [Range(0f, 1f)] public float bgmVolume = 0.6f;
        [Range(0f, 1f)] public float sfxVolume = 1f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (bgmSource != null) bgmSource.volume = bgmVolume;
            if (sfxSource != null) sfxSource.volume = sfxVolume;
        }

        public void PlayBGM(AudioClip clip, bool loop = true)
        {
            if (bgmSource == null || clip == null) return;
            if (bgmSource.clip == clip && bgmSource.isPlaying) return;

            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
        }

        public void StopBGM() => bgmSource?.Stop();

        public void PlaySfx(AudioClip clip)
        {
            if (sfxSource == null || clip == null) return;
            sfxSource.PlayOneShot(clip, sfxVolume);
        }

        // Shortcut สำหรับ SFX ที่ใช้บ่อยในทุก mini game
        public void PlayCorrect() => PlaySfx(correctSfx);
        public void PlayWrong() => PlaySfx(wrongSfx);
        public void PlayPickup() => PlaySfx(pickupSfx);
        public void PlayLockIn() => PlaySfx(lockInSfx);
        public void PlayDoorOpen() => PlaySfx(doorOpenSfx);
        public void PlayUiClick() => PlaySfx(uiClickSfx);
    }
}
