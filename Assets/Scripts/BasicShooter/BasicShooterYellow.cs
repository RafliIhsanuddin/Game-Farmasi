using UnityEngine;

public class BasicShooterYellow : MonoBehaviour
{
    [Header("Tile Ownership")]
    public Tile ownerTile;

    [Header("Shooting")]
    public GameObject projectile;
    public Transform shootOrigin;
    public float cooldown = 1f;

    [Header("Detection (LINE CHECK)")]
    public float range = 5f;
    public LayerMask shootMask;

    // =====================
    // INTERNAL STATE
    // =====================
    private bool hasTriggered = false;   // 🔒 GLOBAL LOCK
    private bool canShoot = true;
    private Collider2D myCollider;

    // =====================
    // RUNTIME DEBUG DATA
    // =====================
    private RaycastHit2D runtimeHit;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        // ❌ Sudah trigger / mati
        if (hasTriggered)
            return;

        // ❌ Belum ditempatkan di tile
        if (ownerTile == null)
            return;

        // ❌ Masih cooldown
        if (!canShoot)
            return;

        runtimeHit = Physics2D.Raycast(
            transform.position,
            Vector2.right,
            range,
            shootMask
        );

        if (runtimeHit.collider != null)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // 🔒 DOUBLE GUARD
        if (!canShoot || hasTriggered)
            return;

        canShoot = false;

        Instantiate(
            projectile,
            shootOrigin.position,
            Quaternion.identity
        );

        Invoke(nameof(ResetCooldown), cooldown);
    }

    private void ResetCooldown()
    {
        // ❌ Jangan reset kalau sudah trigger
        if (hasTriggered)
            return;

        canShoot = true;
    }

    // =====================
    // TRIGGER (ONE TIME)
    // =====================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return;

        // ===============================
        // TARGET UTAMA: BACTERIA PURPLE
        // ===============================
        if (collision.TryGetComponent<BacteriaControllerPurple>(out BacteriaControllerPurple purple))
        {
            hasTriggered = true;

            purple.Hit();          // 🔥 MATI SEKALI
            CleanupShooter();
            return;
        }

        // ===============================
        // OBJEK LAIN (TETAP MATI SHOOTER)
        // ===============================
        if (collision.TryGetComponent<BacteriaControllerRed>(out _) ||
            collision.TryGetComponent<BacteriaControllerGreen>(out _) ||
            collision.TryGetComponent<MushroomController>(out _) ||
            collision.TryGetComponent<ProtozoaController>(out _) ||
            collision.TryGetComponent<HelminthController>(out _))
        {
            hasTriggered = true;
            CleanupShooter();
        }
    }

    private void CleanupShooter()
    {
        // 🔒 MATIKAN SEMUA AKSI
        canShoot = false;

        if (myCollider != null)
            myCollider.enabled = false;

        if (ownerTile != null)
        {
            ownerTile.hasCapsule = false;
            ownerTile.currentCapsule = null;
        }

        Destroy(gameObject);
    }

    // =====================
    // GIZMOS
    // =====================
    private void OnDrawGizmos()
    {
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.right * range;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(start, end);

        if (Application.isPlaying && runtimeHit.collider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(start, runtimeHit.point);
            Gizmos.DrawSphere(runtimeHit.point, 0.1f);
        }
    }
}
