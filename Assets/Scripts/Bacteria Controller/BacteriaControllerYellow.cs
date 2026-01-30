using UnityEngine;

public class BacteriaControllerYellow : MonoBehaviour
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
    // 🟡 ATOM DROP SETTINGS
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

        Debug.Log($"[DEBUG][Yellow] Awake → VFX ready: {hitPS != null}");
    }

    private void FixedUpdate()
    {
        if (isHit) return;

        // ❗ MOVEMENT TIDAK DIUBAH
        transform.position -= new Vector3(speed, 0f, 0f);
    }

    // =========================
    // MAIN DEATH HANDLER
    // =========================
    public void Hit()
    {
        if (isHit)
        {
            Debug.Log("[DEBUG][Yellow] Hit() diabaikan (isHit == true)");
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
        // 🟡 DROP ATOM (TERINTEGRASI)
        // =========================
        TrySpawnAtom();

        // Clear spawn slot
        if (spawnPoint != null)
            spawnPoint.ClearOccupied();

        // Register kill
        WaveManager manager = FindFirstObjectByType<WaveManager>();
        if (manager != null)
            manager.RegisterKill();

        Destroy(gameObject, 3f);
        Debug.Log("[DEBUG][Yellow] Bakteri akan dihancurkan 3 detik lagi");
    }

    // =========================
    // 🟡 ATOM DROP LOGIC (KUNCI)
    // =========================
    private void TrySpawnAtom()
    {
        if (atomPrefab == null)
        {
            Debug.LogWarning("[DEBUG][Yellow] atomPrefab belum di-assign");
            return;
        }

        if (Random.value > atomDropChance)
        {
            Debug.Log("[DEBUG][Yellow] Atom tidak drop (chance gagal)");
            return;
        }

        // Spawn atom di posisi bakteri mati
        GameObject atom = Instantiate(
            atomPrefab,
            transform.position,
            Quaternion.identity
        );

        // 🔑 KORELASI DENGAN SCRIPT Atom
        Atom atomScript = atom.GetComponent<Atom>();
        if (atomScript != null)
        {
            atomScript.useRandomSpawn = false;
        }

        Debug.Log("[DEBUG][Yellow] Atom drop berhasil (mode bakteri)");
    }

    // =========================
    // PROJECTILE EVENTS
    // =========================
    public void ProjectileHit(int damage)
    {
        if (isHit) return;

        health -= damage;
        Debug.Log($"[DEBUG][Yellow] Damage={damage}, HP sisa={health}");

        if (health <= 0)
            Hit();
    }

    public void ProjectileDead()
    {
        if (isHit) return;

        Debug.Log("[DEBUG][Yellow] ProjectileDead()");
        Hit();
    }
}
