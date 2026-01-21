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

    // 🌟 WRONG SFX
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

    // ==================================
    // VOLUME CONTROLLERS
    // ==================================
    [Header("Wave SFX Volume (0-1)")]
    [Range(0f, 1f)] [SerializeField] private float bigWaveVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float sirenVolume = 1f;

    // ==================================
    // WIN SFX
    // ==================================
    [Header("Win SFX")]
    [SerializeField] private AudioClip winSfx;

    [Tooltip("Volume dasar win SFX (0-3)")]
    [Range(0f, 3f)]
    [SerializeField] private float winVolume = 1.5f; // default lebih keras

    [Tooltip("Multiplier extra saat win (1-3)")]
    [Range(1f, 3f)]
    [SerializeField] private float winBoostMultiplier = 1.8f; // boost final


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
    // PROJECTILE API (KEEP)
    // ======================
    public void PlayYellowHit() => PlayClip(hitYellowProjectileSfx);
    public void PlayBlueHit()   => PlayClip(hitBlueProjectileSfx);
    public void PlayCreamHit()  => PlayClip(hitCreamProjectileSfx);
    public void PlayRedHit()    => PlayClip(hitRedProjectileSfx);
    public void PlayGreenHit()  => PlayClip(hitGreenProjectileSfx);
    public void PlayOrangeHit() => PlayClip(hitOrangeProjectileSfx);

    // ======================
    // WAVE API (KEEP)
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
    // ATOM API (KEEP)
    // ======================
    public void PlayAtomClick() => PlayClip(atomClickSfx);

    // ======================
    // WRONG API (KEEP)
    // ======================
    public void PlayWrong() => PlayClip(wrongSfx);

    // ======================
    // WIN SFX (BOOSTED)
    // ======================
    public void PlayWin()
    {
        if (winSfx == null || audioSourcePrefab == null) return;

        GameObject go = Instantiate(audioSourcePrefab, transform);
        AudioSource src = go.GetComponent<AudioSource>();

        float boosted = Mathf.Clamp(winVolume * winBoostMultiplier, 0f, 3f);
        src.volume = boosted;

        src.PlayOneShot(winSfx);
        Destroy(go, destroyDelay);
    }

    // ======================
    // BGM LOOP API (KEEP UPDATED)
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

    private IEnumerator BgmCoroutine()
    {
        while (true)
        {
            bgmLoopSource.clip = bgmLoopClip;
            bgmLoopSource.Play();

            yield return new WaitForSeconds(bgmLoopClip.length);

            if (useBgmLoopDelay && bgmLoopDelaySeconds > 0f)
                yield return new WaitForSeconds(bgmLoopDelaySeconds);
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
