using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    // TRACK ALL ACTIVE AUDIO SOURCES
    // =========================
    private readonly List<AudioSource> activeSources = new();

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
    // STATE
    // =========================
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

        activeSources.Add(bgmLoopSource);
    }

    // =====================================================
    // RESET
    // =====================================================
    public void ResetForNewGame()
    {
        isGameWin = false;
        isGamePaused = false;

        StopAllCoroutines();

        foreach (var src in activeSources)
        {
            if (src != null)
                src.Stop();
        }

        activeSources.Clear();
        activeSources.Add(bgmLoopSource);
    }

    // =====================================================
    // PAUSE ALL AUDIO
    // =====================================================
    public void PauseBgm()
    {
        if (isGamePaused) return;

        isGamePaused = true;

        foreach (var src in activeSources)
        {
            if (src != null && src.isPlaying)
                src.Pause();
        }
    }

    public void ResumeBgm()
    {
        if (!isGamePaused) return;

        isGamePaused = false;

        foreach (var src in activeSources)
        {
            if (src != null)
                src.UnPause();
        }
    }

    // =====================================================
    // PROJECTILE
    // =====================================================
    public void PlayYellowHit() => PlayClip(hitYellowProjectileSfx);
    public void PlayBlueHit() => PlayClip(hitBlueProjectileSfx);
    public void PlayCreamHit() => PlayClip(hitCreamProjectileSfx);
    public void PlayRedHit() => PlayClip(hitRedProjectileSfx);
    public void PlayGreenHit() => PlayClip(hitGreenProjectileSfx);
    public void PlayOrangeHit() => PlayClip(hitOrangeProjectileSfx);

    // =====================================================
    // BIG WAVE
    // =====================================================
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

    // =====================================================
    // UI
    // =====================================================
    public void PlayWrong() => PlayClip(wrongSfx);
    public void PlayAtomClick() => PlayClip(atomClickSfx);

    // =====================================================
    // WIN
    // =====================================================
    public void PlayWin()
    {
        if (isGameWin) return;

        isGameWin = true;

        StopBgmLoop();

        float boosted = Mathf.Clamp(winVolume * winBoostMultiplier, 0f, 3f);

        PlayOneShot(winSfx, boosted);
    }

    // =====================================================
    // BGM LOOP
    // =====================================================
    public void PlayBgmLoop()
    {
        if (bgmLoopClip == null || bgmCoroutine != null) return;

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
            if (isGameWin) yield break;

            bgmLoopSource.clip = bgmLoopClip;
            bgmLoopSource.Play();

            yield return new WaitForSecondsRealtime(bgmLoopClip.length);

            if (useBgmLoopDelay)
                yield return new WaitForSecondsRealtime(bgmLoopDelaySeconds);
        }
    }

    // =====================================================
    // CORE AUDIO SPAWN SYSTEM
    // =====================================================
    private void PlayClip(AudioClip clip)
    {
        if (clip == null) return;
        PlayOneShot(clip, 1f);
    }

    private void PlayOneShot(AudioClip clip, float volume)
    {
        if (audioSourcePrefab == null) return;

        GameObject go = Instantiate(audioSourcePrefab, transform);

        AudioSource src = go.GetComponent<AudioSource>();

        src.volume = volume;
        src.clip = clip;

        activeSources.Add(src);

        src.Play();

        StartCoroutine(RemoveAfterPlay(src, go));
    }

    private IEnumerator RemoveAfterPlay(AudioSource src, GameObject go)
    {
        yield return new WaitForSecondsRealtime(src.clip.length + destroyDelay);

        activeSources.Remove(src);

        Destroy(go);
    }
}
