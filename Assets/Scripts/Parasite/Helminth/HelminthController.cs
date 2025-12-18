using UnityEngine;

public class HelminthController : MonoBehaviour
{
    public float speed = 1.2f; // helminth lebih lambat dari protozoa

    [HideInInspector] public SpawnPointSlot spawnPoint;

    [SerializeField] private GameObject helminthVisual;     // body / sprite helminth
    [SerializeField] private GameObject hitParticleEffect; // efek saat terkena kapsul

    private bool isHit = false;
    private Collider2D myCollider;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (isHit) return;

        // Gerak ke kiri (seperti PVZ)
        transform.position -= new Vector3(speed, 0, 0);
    }

    public void Hit()
    {
        if (isHit) return; // guard clause

        Debug.Log($"[HelminthController] {name} terkena serangan pada posisi {transform.position}");

        isHit = true;
        speed = 0;

        // Matikan collider
        if (myCollider != null)
        {
            myCollider.enabled = false;
            Debug.Log($"[HelminthController] Collider dinonaktifkan untuk {name}");
        }

        // Matikan visual helminth
        if (helminthVisual != null)
            helminthVisual.SetActive(false);

        // Spawn efek partikel (SAMA PERSIS DENGAN PROTOZOA & MUSHROOM)
        if (hitParticleEffect != null)
        {
            GameObject effect = Instantiate(
                hitParticleEffect,
                transform.position,
                Quaternion.identity
            );

            effect.SetActive(true); // 🔑 WAJIB (penting!)

            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
                ps.Play();

            Destroy(effect, 3f);
            Debug.Log($"[HelminthController] Efek partikel ditampilkan untuk {name}");
        }

        // Bebaskan spawn point
        if (spawnPoint != null)
        {
            spawnPoint.ClearOccupied();
        }

        // Register kill ke WaveManager
        var manager = FindFirstObjectByType<WaveManager>();
        if (manager != null)
            manager.RegisterKill();

        Debug.Log($"[HelminthController] {name} akan dihapus dalam 3 detik");
        Destroy(gameObject, 3f);
    }
    
}
