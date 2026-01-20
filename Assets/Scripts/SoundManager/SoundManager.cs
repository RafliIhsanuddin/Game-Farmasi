using UnityEngine;

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

    // 🌟 ADD: WRONG SFX
    [Header("UI / Validation")]
    [SerializeField] private AudioClip wrongSfx;
    
    [Header("Atom SFX")]
    [SerializeField] private AudioClip atomClickSfx;

    [Header("Destroy Delay")]
    [SerializeField] private float destroyDelay = 3f;

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
    public void PlayBigWaveWarning() => PlayClip(bigWaveWarningSfx);
    public void PlayWaveStartMusic() => PlayClip(waveStartMusic);
    
    // ======================
    // ATOM API (ONLY CLICK)
    // ======================
    public void PlayAtomClick() => PlayClip(atomClickSfx);
    
    // ======================
    // WRONG UI SFX
    // ======================
    public void PlayWrong() => PlayClip(wrongSfx);

    // ======================
    // INTERNAL
    // ======================
    private void PlayClip(AudioClip clip)
    {
        if (audioSourcePrefab == null || clip == null) return;

        GameObject go = Instantiate(audioSourcePrefab, transform);
        AudioSource src = go.GetComponent<AudioSource>();

        if (src == null)
        {
            Destroy(go);
            return;
        }

        src.Stop();
        src.clip = null;
        src.PlayOneShot(clip);

        Destroy(go, destroyDelay);
    }
}
