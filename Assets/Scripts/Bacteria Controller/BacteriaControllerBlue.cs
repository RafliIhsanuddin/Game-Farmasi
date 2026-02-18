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

    [Header("Atom Drop On Death")]
    [SerializeField] private GameObject atomPrefab;

    [Range(0f, 1f)]
    [SerializeField] private float atomDropChance = 1f;

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
    }

    private void FixedUpdate()
    {
        if (isHit) return;

        transform.position -= new Vector3(speed, 0f, 0f);
    }

    // =========================
    // HIT / DEATH
    // =========================

    public void Hit()
    {
        if (isHit) return;

        isHit = true;

        speed = 0f;

        if (myCollider != null)
            myCollider.enabled = false;

        if (bacteriaVisual != null)
            bacteriaVisual.SetActive(false);

        if (hitParticleEffect != null && hitPS != null)
        {
            hitParticleEffect.SetActive(true);
            hitPS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            hitPS.Play(true);
        }

        TrySpawnAtom();

        WaveManager manager = FindFirstObjectByType<WaveManager>();
        if (manager != null)
            manager.RegisterKill();

        Destroy(gameObject, 3f);
    }

    // =========================
    // CLEAR SLOT SAFELY
    // =========================

    private void OnDestroy()
    {
        if (spawnPoint != null)
        {
            spawnPoint.ClearOccupied();
            spawnPoint = null;
        }
    }

    // =========================
    // ATOM DROP
    // =========================

    private void TrySpawnAtom()
    {
        if (atomPrefab == null)
            return;

        if (Random.value > atomDropChance)
            return;

        GameObject atom = Instantiate(
            atomPrefab,
            transform.position,
            Quaternion.identity
        );

        Atom atomScript = atom.GetComponent<Atom>();

        if (atomScript != null)
            atomScript.useRandomSpawn = false;
    }

    // =========================
    // PROJECTILE EVENTS
    // =========================

    public void ProjectileHit(int damage)
    {
        if (isHit) return;

        health -= damage;

        if (health <= 0)
            Hit();
    }

    public void ProjectileDead()
    {
        if (isHit) return;

        Hit();
    }
}
