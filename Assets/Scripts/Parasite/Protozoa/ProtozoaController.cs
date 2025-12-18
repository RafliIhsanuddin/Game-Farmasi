using UnityEngine;

public class ProtozoaController : MonoBehaviour
{
    public float speed = 1.5f;

    [HideInInspector] public SpawnPointSlot spawnPoint;

    [SerializeField] private GameObject protozoaVisual;     // body / sprite protozoa
    [SerializeField] private GameObject hitParticleEffect;  // efek saat terkena kapsul

    private bool isHit = false;
    private Collider2D myCollider;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        if (isHit) return;

        transform.position -= new Vector3(speed, 0, 0);
    }

    public void Hit()
    {
        if (isHit) return; // guard clause

        Debug.Log($"[ProtozoaController] {name} terkena serangan pada posisi {transform.position}");

        isHit = true;
        speed = 0;

        // Matikan collider
        if (myCollider != null)
        {
            myCollider.enabled = false;
            Debug.Log($"[ProtozoaController] Collider dinonaktifkan untuk {name}");
        }

        // Matikan visual protozoa
        if (protozoaVisual != null)
            protozoaVisual.SetActive(false);

        // Spawn efek partikel (SAMA PERSIS DENGAN MUSHROOM)
        if (hitParticleEffect != null)
        {
            GameObject effect = Instantiate(
                hitParticleEffect,
                transform.position,
                Quaternion.identity
            );

            effect.SetActive(true); // 🔑 WAJIB

            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
                ps.Play();

            Destroy(effect, 3f);
            Debug.Log($"[ProtozoaController] Efek partikel ditampilkan untuk {name}");
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

        Debug.Log($"[ProtozoaController] {name} akan dihapus dalam 3 detik");
        Destroy(gameObject, 3f);
    }
}
