using UnityEngine;
using System.Collections;

public class BasicShooterCreamSoldier : MonoBehaviour
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
                "[BasicShooterCream] Animator belum di-assign (harus child Visual)."
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

        // 🔥 Trigger animasi saja (spawn lewat Animation Event)
        if (animator != null)
            animator.SetTrigger("Shoot");
    }

    // ==================================================
    // DIPANGGIL OLEH ShooterAnimationEventCream
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
    // COLLISION RULE
    // Shooter MATI jika nabrak enemy apa pun
    // Enemy TIDAK disentuh
    // =====================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return;

        if (IsAnyEnemy(collision))
        {
            hasTriggered = true;
            CleanupShooter();
        }
    }

    // =====================
    // UNIVERSAL ENEMY CHECK
    // =====================
    private bool IsAnyEnemy(Collider2D col)
    {
        return
            col.GetComponent<VirusController>() != null ||

            col.GetComponent<BacteriaControllerBlue>() != null ||
            col.GetComponent<BacteriaControllerRed>() != null ||
            col.GetComponent<BacteriaControllerGreen>() != null ||
            col.GetComponent<BacteriaControllerYellow>() != null ||
            col.GetComponent<BacteriaControllerPink>() != null ||
            col.GetComponent<BacteriaControllerOrange>() != null ||
            col.GetComponent<BacteriaControllerPurple>() != null ||

            col.GetComponent<MushroomController>() != null ||
            col.GetComponent<ProtozoaController>() != null ||
            col.GetComponent<HelminthController>() != null;
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
