using UnityEngine;
using System.Collections;

public class BasicShooterGreenSoldier : MonoBehaviour
{
    [Header("Tile Ownership")]
    public Tile ownerTile;

    [Header("Shooting")]
    public GameObject projectile;
    public Transform shootOrigin;
    public float cooldown = 1f;

    [Header("Animation")]
    [SerializeField] private Animator animator; // Animator di CHILD (Visual)

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

        if (animator == null)
        {
            Debug.LogWarning(
                "[BasicShooterGreen] Animator belum di-assign (harus child Visual)."
            );
        }
    }

    private void Update()
    {
        if (hasTriggered) return;
        if (ownerTile == null) return;
        if (!canShoot) return;

        runtimeHit = Physics2D.Raycast(
            transform.position,
            Vector2.right,
            range,
            shootMask
        );

        if (runtimeHit.collider != null)
        {
            TryShoot();
        }
    }

    // =====================
    // SHOOT FLOW
    // =====================
    private void TryShoot()
    {
        if (!canShoot || hasTriggered)
            return;

        canShoot = false;

        // 🔥 Hanya trigger animasi
        if (animator != null)
            animator.SetTrigger("Shoot");
    }

    // ==================================================
    // DIPANGGIL OLEH ShooterAnimationEventGreen
    // ==================================================
    public void FireProjectileFromAnimation()
    {
        if (hasTriggered)
            return;

        Instantiate(
            projectile,
            shootOrigin.position,
            Quaternion.identity
        );

        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(cooldown);

        if (!hasTriggered)
            canShoot = true;
    }

    // =====================
    // TRIGGER (ONE TIME)
    // =====================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return;

        // ===============================
        // TARGET UTAMA: PROTOZOA
        // ===============================
        if (collision.TryGetComponent<ProtozoaController>(out var protozoa))
        {
            hasTriggered = true;
            protozoa.Hit();        // 🔥 PROTOZOA MATI
            CleanupShooter();
            return;
        }

        // ===============================
        // OBJEK LAIN (SHOOTER TETAP MATI)
        // ===============================
        if (collision.TryGetComponent<BacteriaControllerRed>(out _) ||
            collision.TryGetComponent<BacteriaControllerGreen>(out _) ||
            collision.TryGetComponent<BacteriaControllerPurple>(out _) ||
            collision.TryGetComponent<MushroomController>(out _) ||
            collision.TryGetComponent<HelminthController>(out _))
        {
            hasTriggered = true;
            CleanupShooter();
        }
    }

    private void CleanupShooter()
    {
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

        Gizmos.color = Color.green;
        Gizmos.DrawLine(start, end);

        if (Application.isPlaying && runtimeHit.collider != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(start, runtimeHit.point);
            Gizmos.DrawSphere(runtimeHit.point, 0.1f);
        }
    }
}
