using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Source Prefab (One-Shot SFX)")]
    [SerializeField] private GameObject audioSourcePrefab;

    // =====================================================
    // PROJECTILE SFX
    // =====================================================
    [Header("Projectile Hit Clips")]
    [SerializeField] private AudioClip hitYellowProjectileSfx;
    [SerializeField] private AudioClip hitBlueProjectileSfx;
    [SerializeField] private AudioClip hitCreamProjectileSfx;
    [SerializeField] private AudioClip hitRedProjectileSfx;
    [SerializeField] private AudioClip hitGreenProjectileSfx;
    [SerializeField] private AudioClip hitOrangeProjectileSfx;

    // =====================================================
    // UI / SYSTEM SFX
    // =====================================================
    [Header("UI / System Clips")]
    [SerializeField] private AudioClip wrongSfx;
    [SerializeField] private AudioClip atomClickSfx;
    [SerializeField] private AudioClip bigWaveWarningSfx;
    [SerializeField] private AudioClip waveStartMusic;

    // =====================================================
    // WIN SFX
    // =====================================================
    [Header("Win SFX")]
    [SerializeField] private AudioClip winSfx;
    [SerializeField] private float winVolume = 1.5f;
    [SerializeField] private float winBoostMultiplier = 1.8f;

    // =====================================================
    // BGM LOOP (BEST PRACTICE)
    // =====================================================
    [Header("BGM Loop")]
    [SerializeField] private AudioClip bgmLoopClip;
    [SerializeField][Range(0f, 1f)] private float bgmVolume = 1f;

    private AudioSource bgmSource;

    // =====================================================
    // TRACK ACTIVE SOURCES
    // =====================================================
    private readonly List<AudioSource> activeSources = new();

    [Header("Destroy Delay")]
    [SerializeField] private float destroyDelay = 2f;

    // =====================================================
    // STATE
    // =====================================================
    private bool isGamePaused = false;
    private bool isGameWin = false;

    // =====================================================
    // INIT
    // =====================================================
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        CreateBgmSource();
    }

    private void CreateBgmSource()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();

        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = bgmVolume;

        activeSources.Add(bgmSource);
    }

    // =====================================================
    // RESET FOR NEW GAME
    // =====================================================
    public void ResetForNewGame()
    {
        isGamePaused = false;
        isGameWin = false;

        StopAllCoroutines();

        foreach (var src in activeSources)
        {
            if (src != null)
                src.Stop();
        }

        activeSources.Clear();

        if (bgmSource != null)
            activeSources.Add(bgmSource);
    }

    // =====================================================
    // BGM CONTROL (BEST PRACTICE)
    // =====================================================
    public void PlayBgmLoop()
    {
        if (bgmLoopClip == null) return;
        if (bgmSource.isPlaying) return;

        bgmSource.clip = bgmLoopClip;
        bgmSource.volume = bgmVolume;
        bgmSource.loop = true;

        bgmSource.Play();
    }

    public void StopBgmLoop()
    {
        if (bgmSource == null) return;

        bgmSource.Stop();
    }

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
    // WIN
    // =====================================================
    public void PlayWin()
    {
        if (isGameWin) return;

        isGameWin = true;

        StopBgmLoop();

        float boostedVolume =
            Mathf.Clamp(winVolume * winBoostMultiplier, 0f, 3f);

        PlayOneShot(winSfx, boostedVolume);
    }

    // =====================================================
    // PROJECTILE SFX
    // =====================================================
    public void PlayYellowHit() => PlayClip(hitYellowProjectileSfx);
    public void PlayBlueHit() => PlayClip(hitBlueProjectileSfx);
    public void PlayCreamHit() => PlayClip(hitCreamProjectileSfx);
    public void PlayRedHit() => PlayClip(hitRedProjectileSfx);
    public void PlayGreenHit() => PlayClip(hitGreenProjectileSfx);
    public void PlayOrangeHit() => PlayClip(hitOrangeProjectileSfx);

    // =====================================================
    // UI SFX
    // =====================================================
    public void PlayWrong() => PlayClip(wrongSfx);

    public void PlayAtomClick() => PlayClip(atomClickSfx);

    public void PlayBigWaveWarning()
        => PlayOneShot(bigWaveWarningSfx, 1f);

    public void PlayWaveStartMusic()
        => PlayOneShot(waveStartMusic, 1f);

    // =====================================================
    // CORE ONE-SHOT SYSTEM
    // =====================================================
    private void PlayClip(AudioClip clip)
    {
        if (clip == null) return;

        PlayOneShot(clip, 1f);
    }

    private void PlayOneShot(AudioClip clip, float volume)
    {
        if (clip == null) return;
        if (audioSourcePrefab == null) return;

        GameObject go =
            Instantiate(audioSourcePrefab, transform);

        AudioSource src =
            go.GetComponent<AudioSource>();

        src.clip = clip;
        src.volume = volume;
        src.loop = false;

        activeSources.Add(src);

        src.Play();

        StartCoroutine(RemoveAfterPlay(src, go));
    }

    private IEnumerator RemoveAfterPlay(
        AudioSource src,
        GameObject go)
    {
        yield return new WaitForSecondsRealtime(
            src.clip.length + destroyDelay);

        activeSources.Remove(src);

        Destroy(go);
    }
}
