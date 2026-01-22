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

    [Header("Wave Music / Start")]
    [SerializeField] private AudioClip waveStartMusic;

    [Header("UI / Validation")]
    [SerializeField] private AudioClip wrongSfx;

    [Header("Atom SFX")]
    [SerializeField] private AudioClip atomClickSfx;

    [Header("Destroy Delay")]
    [SerializeField] private float destroyDelay = 3f;

    // ==================================
    // BGM LOOP SYSTEM (UPDATED)
    // ==================================
    [Header("BGM Loop (UPDATED)")]
    [SerializeField] private AudioClip bgmLoopClip;

    [Tooltip("Jika ON → pakai delay setelah clip selesai. Jika OFF → langsung loop tanpa delay.")]
    [SerializeField] private bool useBgmLoopDelay = true;

    [SerializeField] private float bgmLoopDelaySeconds = 2f;

    private AudioSource bgmLoopSource;
    private Coroutine bgmCoroutine;

    // VOLUME CONTROLLERS
    [Header("Wave SFX Volume (0-1)")]
    [Range(0f, 1f)] [SerializeField] private float bigWaveVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float sirenVolume = 1f;

    // WIN SFX
    [Header("Win SFX")]
    [SerializeField] private AudioClip winSfx;

    [Range(0f, 3f)]
    [SerializeField] private float winVolume = 1.5f;

    [Range(1f, 3f)]
    [SerializeField] private float winBoostMultiplier = 1.8f;

    // CONTROL FLAGS
    private bool isGamePaused = false;
    private bool isGameWin = false;

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

    // ======================
    // PROJECTILE API
    // ======================
    public void PlayYellowHit() => PlayClip(hitYellowProjectileSfx);
    public void PlayBlueHit()   => PlayClip(hitBlueProjectileSfx);
    public void PlayCreamHit()  => PlayClip(hitCreamProjectileSfx);
    public void PlayRedHit()    => PlayClip(hitRedProjectileSfx);
    public void PlayGreenHit()  => PlayClip(hitGreenProjectileSfx);
    public void PlayOrangeHit() => PlayClip(hitOrangeProjectileSfx);

    // ======================
    // WAVE API
    // ======================
    public void PlayBigWaveWarning()
    {
        if (bigWaveWarningSfx == null) return;
        GameObject go = Instantiate(audioSourcePrefab, transform);
        AudioSource src = go.GetComponent<AudioSource>();
        src.volume = bigWaveVolume;
        src.PlayOneShot(bigWaveWarningSfx);
        Destroy(go, destroyDelay);
    }

    public void PlayWaveStartMusic()
    {
        if (waveStartMusic == null) return;
        GameObject go = Instantiate(audioSourcePrefab, transform);
        AudioSource src = go.GetComponent<AudioSource>();
        src.volume = sirenVolume;
        src.PlayOneShot(waveStartMusic);
        Destroy(go, destroyDelay);
    }

    // ======================
    // ATOM API
    // ======================
    public void PlayAtomClick() => PlayClip(atomClickSfx);

    // ======================
    // WRONG API
    // ======================
    public void PlayWrong() => PlayClip(wrongSfx);

    // ======================
    // WIN SFX STOP BGM
    // ======================
    public void PlayWin()
    {
        if (isGameWin) return;
        isGameWin = true;

        // Stop loop total (sesuai pilihan B)
        StopBgmLoop();

        if (winSfx == null || audioSourcePrefab == null) return;

        GameObject go = Instantiate(audioSourcePrefab, transform);
        AudioSource src = go.GetComponent<AudioSource>();

        float boosted = Mathf.Clamp(winVolume * winBoostMultiplier, 0f, 3f);
        src.volume = boosted;
        src.PlayOneShot(winSfx);

        Destroy(go, destroyDelay);
    }

    // ======================
    // BGM LOOP CONTROL
    // ======================
    public void PlayBgmLoop()
    {
        if (bgmLoopClip == null || bgmLoopSource == null) return;
        if (bgmCoroutine != null) return;

        bgmCoroutine = StartCoroutine(BgmCoroutine());
    }

    public void StopBgmLoop()
    {
        if (bgmCoroutine != null)
        {
            StopCoroutine(bgmCoroutine);
            bgmCoroutine = null;
        }

        if (bgmLoopSource != null)
            bgmLoopSource.Stop();
    }

    // ======================
    // BGM PAUSE / RESUME
    // ======================
    public void PauseBgm()
    {
        isGamePaused = true;
        if (bgmLoopSource != null && bgmLoopSource.isPlaying)
            bgmLoopSource.Pause();
    }

    public void ResumeBgm()
    {
        isGamePaused = false;
        if (!isGameWin && bgmLoopSource != null)
            bgmLoopSource.UnPause();
    }

    // ======================
    // BGM COROUTINE (FIXED)
    // ======================
    private IEnumerator BgmCoroutine()
    {
        while (true)
        {
            if (isGameWin) yield break;

            // tunggu sampai tidak di-pause
            if (isGamePaused)
                yield return new WaitUntil(() => !isGamePaused);

            bgmLoopSource.clip = bgmLoopClip;
            bgmLoopSource.Play();

            // hitung durasi clip realtime, tapi patuh pause
            float t = 0f;
            while (t < bgmLoopClip.length)
            {
                if (!isGamePaused && !isGameWin)
                    t += Time.unscaledDeltaTime;

                yield return null;
            }

            if (isGameWin) yield break;

            if (useBgmLoopDelay && bgmLoopDelaySeconds > 0f)
            {
                float d = 0f;
                while (d < bgmLoopDelaySeconds)
                {
                    if (!isGamePaused && !isGameWin)
                        d += Time.unscaledDeltaTime;

                    yield return null;
                }
            }
        }
    }

    // ======================
    // INTERNAL ONE-SHOT
    // ======================
    private void PlayClip(AudioClip clip)
    {
        if (audioSourcePrefab == null || clip == null) return;

        GameObject go = Instantiate(audioSourcePrefab, transform);
        AudioSource src = go.GetComponent<AudioSource>();
        src.PlayOneShot(clip);

        Destroy(go, destroyDelay);
    }
}
