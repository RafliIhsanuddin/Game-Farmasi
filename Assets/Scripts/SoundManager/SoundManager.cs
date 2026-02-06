using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Source Prefab")]
    [SerializeField] private GameObject audioSourcePrefab;

    [Header("Projectile Hit Clips")]
    [SerializeField] private AudioClip hitYellowProjectileSfx;
    [SerializeField] private AudioClip hitBlueProjectileSfx;
    [SerializeField] private AudioClip hitCreamProjectileSfx;
    [SerializeField] private AudioClip hitRedProjectileSfx;
    [SerializeField] private AudioClip hitGreenProjectileSfx;
    [SerializeField] private AudioClip hitOrangeProjectileSfx;

    [Header("Big Wave SFX")]
    [SerializeField] private AudioClip bigWaveWarningSfx;

    [Header("Wave Start Music")]
    [SerializeField] private AudioClip waveStartMusic;

    [Header("UI / Validation")]
    [SerializeField] private AudioClip wrongSfx;

    [Header("Atom SFX")]
    [SerializeField] private AudioClip atomClickSfx;

    [Header("Destroy Delay")]
    [SerializeField] private float destroyDelay = 3f;

    // =========================
    // BGM LOOP
    // =========================
    [Header("BGM Loop")]
    [SerializeField] private AudioClip bgmLoopClip;
    [SerializeField] private bool useBgmLoopDelay = true;
    [SerializeField] private float bgmLoopDelaySeconds = 2f;

    private AudioSource bgmLoopSource;
    private Coroutine bgmCoroutine;

    // =========================
    // VOLUME
    // =========================
    [Header("Volumes")]
    [Range(0f, 1f)] [SerializeField] private float bigWaveVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float sirenVolume = 1f;

    // =========================
    // WIN
    // =========================
    [Header("Win SFX")]
    [SerializeField] private AudioClip winSfx;
    [Range(0f, 3f)] [SerializeField] private float winVolume = 1.5f;
    [Range(1f, 3f)] [SerializeField] private float winBoostMultiplier = 1.8f;

    // =========================
    // INTERNAL STATE
    // =========================
    private bool isGamePaused = false;
    private bool isGameWin = false;
    private bool bgmWasPlayingBeforePause = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        bgmLoopSource = gameObject.AddComponent<AudioSource>();
        bgmLoopSource.loop = false;
        bgmLoopSource.playOnAwake = false;
        bgmLoopSource.volume = 1f;
    }

    // =====================================================
    // RESET (dipanggil WaveManager saat mulai game baru)
    // =====================================================
    public void ResetForNewGame()
    {
        isGameWin = false;
        isGamePaused = false;
        bgmWasPlayingBeforePause = false;

        if (bgmCoroutine != null)
        {
            StopCoroutine(bgmCoroutine);
            bgmCoroutine = null;
        }

        if (bgmLoopSource != null)
            bgmLoopSource.Stop();
    }

    // =========================
    // 🔴 PAUSE / RESUME BGM (INI YANG HILANG)
    // =========================
    public void PauseBgm()
    {
        if (isGamePaused) return;
        if (isGameWin) return;

        isGamePaused = true;

        if (bgmLoopSource != null && bgmLoopSource.isPlaying)
        {
            bgmWasPlayingBeforePause = true;
            bgmLoopSource.Pause();
        }
        else
        {
            bgmWasPlayingBeforePause = false;
        }
    }

    public void ResumeBgm()
    {
        if (!isGamePaused) return;
        if (isGameWin) return;

        isGamePaused = false;

        if (bgmWasPlayingBeforePause && bgmLoopSource != null)
            bgmLoopSource.UnPause();
    }

    // =========================
    // PROJECTILE API
    // =========================
    public void PlayYellowHit() => PlayClip(hitYellowProjectileSfx);
    public void PlayBlueHit()   => PlayClip(hitBlueProjectileSfx);
    public void PlayCreamHit()  => PlayClip(hitCreamProjectileSfx);
    public void PlayRedHit()    => PlayClip(hitRedProjectileSfx);
    public void PlayGreenHit()  => PlayClip(hitGreenProjectileSfx);
    public void PlayOrangeHit() => PlayClip(hitOrangeProjectileSfx);

    // =========================
    // WAVE API
    // =========================
    public void PlayBigWaveWarning()
    {
        if (bigWaveWarningSfx == null) return;
        PlayOneShot(bigWaveWarningSfx, bigWaveVolume);
    }

    public void PlayWaveStartMusic()
    {
        if (waveStartMusic == null) return;
        PlayOneShot(waveStartMusic, sirenVolume);
    }

    // =========================
    // UI / ATOM
    // =========================
    public void PlayWrong() => PlayClip(wrongSfx);
    public void PlayAtomClick() => PlayClip(atomClickSfx);

    // =========================
    // WIN
    // =========================
    public void PlayWin()
    {
        if (isGameWin) return;
        isGameWin = true;

        StopBgmLoop();

        if (winSfx == null) return;
        float boosted = Mathf.Clamp(winVolume * winBoostMultiplier, 0f, 3f);
        PlayOneShot(winSfx, boosted);
    }

    // =========================
    // BGM LOOP
    // =========================
    public void PlayBgmLoop()
    {
        if (bgmLoopClip == null || bgmCoroutine != null) return;
        bgmCoroutine = StartCoroutine(BgmCoroutine());
    }

    public void StopBgmLoop()
    {
        bgmWasPlayingBeforePause = false;

        if (bgmCoroutine != null)
        {
            StopCoroutine(bgmCoroutine);
            bgmCoroutine = null;
        }

        if (bgmLoopSource != null)
            bgmLoopSource.Stop();
    }

    private IEnumerator BgmCoroutine()
    {
        while (true)
        {
            if (isGameWin) yield break;

            bgmLoopSource.clip = bgmLoopClip;
            bgmLoopSource.Play();

            float t = 0f;
            while (t < bgmLoopClip.length)
            {
                if (!isGamePaused)
                    t += Time.unscaledDeltaTime;

                yield return null;
            }

            if (useBgmLoopDelay && bgmLoopDelaySeconds > 0f)
            {
                float d = 0f;
                while (d < bgmLoopDelaySeconds)
                {
                    if (!isGamePaused)
                        d += Time.unscaledDeltaTime;

                    yield return null;
                }
            }
        }
    }

    // =========================
    // INTERNAL ONE SHOT
    // =========================
    private void PlayClip(AudioClip clip)
    {
        if (clip == null || audioSourcePrefab == null) return;
        PlayOneShot(clip, 1f);
    }

    private void PlayOneShot(AudioClip clip, float volume)
    {
        GameObject go = Instantiate(audioSourcePrefab, transform);
        AudioSource src = go.GetComponent<AudioSource>();
        src.volume = volume;
        src.PlayOneShot(clip);
        Destroy(go, destroyDelay);
    }
}
