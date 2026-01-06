using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource shootSfxSource;

    [Header("Default Clips")]
    [SerializeField] private AudioClip defaultBgm;
    [SerializeField] private AudioClip defaultShootSfx;

    private void Awake()
    {
        // Singleton sederhana
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
        // Auto play BGM jika ada
        if (bgmSource != null && defaultBgm != null)
        {
            bgmSource.clip = defaultBgm;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    // =====================
    // BGM
    // =====================
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (bgmSource == null || clip == null) return;

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource == null) return;
        bgmSource.Stop();
    }

    // =====================
    // SFX UMUM
    // =====================
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    // =====================
    // SHOOT SFX (KHUSUS)
    // =====================
    public void PlayShootSFX(AudioClip clip = null)
    {
        if (shootSfxSource == null) return;

        // Kalau tidak dikirim clip → pakai default
        AudioClip finalClip = clip != null ? clip : defaultShootSfx;
        if (finalClip == null) return;

        shootSfxSource.PlayOneShot(finalClip);
    }
}
