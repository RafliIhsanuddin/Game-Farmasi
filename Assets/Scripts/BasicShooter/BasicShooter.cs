using UnityEngine;

public class BasicShooter : MonoBehaviour
{
    [Header("Tile Ownership")]
    public Tile ownerTile;   // diisi oleh GameManager saat spawn

    private bool hasTriggered = false;
    private Collider2D myCollider;

    [Header("Shooting")]
    public GameObject projectile;
    public Transform shootOrigin;
    public float cooldown = 1f;
    private bool canShoot = true;

    [Header("Detection")]
    public float range = 5f;
    public LayerMask shootMask;
    public float detectRadius = 0.25f;   // radius overlap kecil & aman

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        // ❌ Tidak boleh menembak jika belum ditempatkan di tile
        if (ownerTile == null)
            return;

        if (!canShoot)
            return;

        // =====================================
        // OVERLAP DETECTION (AMAN UNTUK TRIGGER)
        // =====================================
        Vector2 detectPos = (Vector2)transform.position + Vector2.right * range;

        Collider2D hit = Physics2D.OverlapCircle(
            detectPos,
            detectRadius,
            shootMask
        );

        if (hit != null)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (!canShoot)
            return;

        canShoot = false;
        Instantiate(projectile, shootOrigin.position, Quaternion.identity);
        Invoke(nameof(ResetCooldown), cooldown);
    }

    private void ResetCooldown()
    {
        canShoot = true;
    }

    // ==================================
    // HANCUR JIKA TERKENA OBJECT
    // ==================================
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
        // 🔒 Matikan collider agar tidak trigger ulang
        if (myCollider != null)
            myCollider.enabled = false;

        // 🔹 Beri tahu tile bahwa shooter hilang
        if (ownerTile != null)
        {
            ownerTile.hasCapsule = false;
            ownerTile.currentCapsule = null;
        }

        Destroy(gameObject);
    }

    // =============================
    // Gizmos (Range Visualization)
    // =============================
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.right * range;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.1f);
    }
}
