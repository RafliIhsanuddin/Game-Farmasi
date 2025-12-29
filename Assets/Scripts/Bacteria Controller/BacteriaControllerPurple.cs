using UnityEngine;

public class BacteriaControllerPurple : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 1.5f;

    [Header("Spawn Reference")]
    [HideInInspector] public SpawnPointSlot spawnPoint;

    [Header("Visual")]
    [SerializeField] private GameObject bacteriaVisual;

    [Tooltip("Particle VFX child (awal NONAKTIF)")]
    [SerializeField] private GameObject hitParticleEffect;

    [Header("Stats")]
    public int health = 3;

    // =========================
    // INTERNAL STATE
    // =========================
    private bool isHit = false;
    private Collider2D myCollider;
    private ParticleSystem hitPS;

    // =========================
    // UNITY EVENTS
    // =========================
    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();

        if (hitParticleEffect != null)
        {
            hitPS = hitParticleEffect.GetComponent<ParticleSystem>();
            hitParticleEffect.SetActive(false); // 🔒 mati di awal
        }

        Debug.Log($"[DEBUG][Purple] Awake → VFX ready: {hitPS != null}");
    }

    private void FixedUpdate()
    {
        if (isHit) return;
        transform.position -= new Vector3(speed, 0f, 0f);
    }

    // =========================
    // MAIN DEATH HANDLER
    // =========================
    public void Hit()
    {
        if (isHit)
        {
            Debug.Log("[DEBUG][Purple] Hit() diabaikan (isHit == true)");
            return;
        }

        Debug.Log($"[DEBUG][Purple] Hit() dieksekusi untuk {name}");

        isHit = true;
        speed = 0f;

        // Disable collider
        if (myCollider != null)
            myCollider.enabled = false;

        // Hide visual sprite
        if (bacteriaVisual != null)
            bacteriaVisual.SetActive(false);

        // =========================
        // AKTIFKAN & PLAY VFX
        // =========================
        if (hitParticleEffect != null && hitPS != null)
        {
            hitParticleEffect.SetActive(true);

            hitPS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            hitPS.Play(true);

            Debug.Log($"[DEBUG][Purple] VFX DISET ACTIVE & PLAY → isPlaying={hitPS.isPlaying}");
        }
        else
        {
            Debug.LogError("[DEBUG][Purple] VFX tidak valid / belum di-assign!");
        }

        // Clear spawn slot
        if (spawnPoint != null)
            spawnPoint.ClearOccupied();

        // Register kill
        WaveManager manager = FindFirstObjectByType<WaveManager>();
        if (manager != null)
            manager.RegisterKill();

        Destroy(gameObject, 3f);
        Debug.Log("[DEBUG][Purple] Bakteri akan dihancurkan 3 detik lagi");
    }

    // =========================
    // PROJECTILE EVENTS
    // =========================
    public void ProjectileHit(int damage)
    {
        if (isHit) return;

        health -= damage;
        Debug.Log($"[DEBUG][Purple] Damage={damage}, HP sisa={health}");

        if (health <= 0)
            Hit();
    }

    public void ProjectileDead()
    {
        if (isHit) return;

        Debug.Log("[DEBUG][Purple] ProjectileDead()");
        Hit();
    }
}
