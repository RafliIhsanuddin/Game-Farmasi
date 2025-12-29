using UnityEngine;

public class BasicShooterBlue : MonoBehaviour
{
    [Header("Tile Ownership")]
    public Tile ownerTile;   // diisi GameManager saat spawn

    [Header("Shooting")]
    public GameObject projectile;
    public Transform shootOrigin;
    public float cooldown = 1f;
    private bool canShoot = true;

    [Header("Detection (LINE CHECK)")]
    public float range = 5f;
    public LayerMask shootMask;

    // =====================
    // INTERNAL STATE
    // =====================
    private bool hasTriggered = false;
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
        // ❌ Belum ditempatkan di tile
        if (ownerTile == null)
            return;

        // ❌ Masih cooldown
        if (!canShoot)
            return;

        // =====================
        // LINE DETECTION
        // =====================
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
        if (!canShoot)
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
        canShoot = true;
    }

    // =====================
    // TRIGGER CLEANUP
    // =====================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered)
            return;

        if (collision.TryGetComponent<BacteriaControllerGreen>(out _) ||
            collision.TryGetComponent<BacteriaControllerPurple>(out _) ||
            collision.TryGetComponent<MushroomController>(out _) ||
            collision.TryGetComponent<ProtozoaController>(out _) ||
            collision.TryGetComponent<BacteriaControllerRed>(out _) ||
            collision.TryGetComponent<HelminthController>(out _))
        {
            hasTriggered = true;
            CleanupShooter();
        }
    }

    private void CleanupShooter()
    {
        // Matikan collider supaya tidak double trigger
        if (myCollider != null)
            myCollider.enabled = false;

        // Lepaskan tile
        if (ownerTile != null)
        {
            ownerTile.hasCapsule = false;
            ownerTile.currentCapsule = null;
        }

        Destroy(gameObject);
    }

    // =====================
    // GIZMOS (REAL-TIME)
    // =====================
    private void OnDrawGizmos()
    {
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.right * range;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(start, end);

        if (Application.isPlaying && runtimeHit.collider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(start, runtimeHit.point);
            Gizmos.DrawSphere(runtimeHit.point, 0.1f);
        }
    }
}
