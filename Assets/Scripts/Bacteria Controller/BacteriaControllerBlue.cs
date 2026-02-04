using UnityEngine;

public class BacteriaControllerBlue : MonoBehaviour
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
    // 🔵 ATOM DROP SETTINGS
    // =========================
    [Header("Atom Drop On Death")]
    [SerializeField] private GameObject atomPrefab;
    [Range(0f, 1f)]
    [SerializeField] private float atomDropChance = 1f;

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
            hitParticleEffect.SetActive(false);
        }

        Debug.Log($"[DEBUG][Blue] Awake → VFX ready: {hitPS != null}");
    }

    private void FixedUpdate()
    {
        if (isHit) return;

        // ✅ SAMA SEPERTI YANG SEHARUSNYA (aman)
        transform.position -= new Vector3(
            speed * Time.fixedDeltaTime,
            0f,
            0f
        );
    }

    // =========================
    // MAIN DEATH HANDLER
    // =========================
    public void Hit()
    {
        if (isHit)
        {
            Debug.Log("[DEBUG][Blue] Hit() diabaikan (isHit == true)");
            return;
        }

        isHit = true;
        speed = 0f;

        // Disable collider
        if (myCollider != null)
            myCollider.enabled = false;

        // Hide visual
        if (bacteriaVisual != null)
            bacteriaVisual.SetActive(false);

        // =========================
        // PLAY HIT VFX
        // =========================
        if (hitParticleEffect != null && hitPS != null)
        {
            hitParticleEffect.SetActive(true);
            hitPS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            hitPS.Play(true);
        }

        // =========================
        // 🔵 DROP ATOM
        // =========================
        TrySpawnAtom();

        // Clear spawn slot (PERSIS SEPERTI RED & PURPLE)
        if (spawnPoint != null)
            spawnPoint.ClearOccupied();

        // Register kill (HANYA DI SINI)
        WaveManager manager = FindFirstObjectByType<WaveManager>();
        if (manager != null)
            manager.RegisterKill();

        Destroy(gameObject, 3f);
        Debug.Log("[DEBUG][Blue] Bakteri akan dihancurkan 3 detik lagi");
    }

    // =========================
    // 🔵 ATOM DROP LOGIC
    // =========================
    private void TrySpawnAtom()
    {
        if (atomPrefab == null)
        {
            Debug.LogWarning("[DEBUG][Blue] atomPrefab belum di-assign");
            return;
        }

        if (Random.value > atomDropChance)
        {
            Debug.Log("[DEBUG][Blue] Atom tidak drop (chance gagal)");
            return;
        }

        GameObject atom = Instantiate(
            atomPrefab,
            transform.position,
            Quaternion.identity
        );

        Atom atomScript = atom.GetComponent<Atom>();
        if (atomScript != null)
            atomScript.useRandomSpawn = false;

        Debug.Log("[DEBUG][Blue] Atom drop berhasil (mode bakteri)");
    }

    // =========================
    // PROJECTILE EVENTS
    // =========================
    public void ProjectileHit(int damage)
    {
        if (isHit) return;

        health -= damage;
        Debug.Log($"[DEBUG][Blue] Damage={damage}, HP sisa={health}");

        if (health <= 0)
            Hit();
    }

    public void ProjectileDead()
    {
        if (isHit) return;

        Debug.Log("[DEBUG][Blue] ProjectileDead()");
        Hit();
    }
}
