using UnityEngine;

public class MushroomController : MonoBehaviour
{
    public float speed = 1.5f;

    [HideInInspector] public SpawnPointSlot spawnPoint;
    [SerializeField] private GameObject mushroomVisual;     // sprite / body jamur
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

        Debug.Log($"[MushroomController] {name} terkena serangan pada posisi {transform.position}");

        isHit = true;
        speed = 0;

        // Matikan collider
        if (myCollider != null)
        {
            myCollider.enabled = false;
            Debug.Log($"[MushroomController] Collider dinonaktifkan untuk {name}");
        }

        // Matikan visual jamur
        if (mushroomVisual != null)
            mushroomVisual.SetActive(false);

        // Spawn efek partikel
        if (hitParticleEffect != null)
        {
            GameObject effect = Instantiate(hitParticleEffect, transform.position, Quaternion.identity);
            effect.SetActive(true);

            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
                ps.Play();

            Destroy(effect, 3f);
            Debug.Log($"[MushroomController] Efek partikel ditampilkan untuk {name}");
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

        Debug.Log($"[MushroomController] {name} akan dihapus dalam 3 detik");
        Destroy(gameObject, 3f);
    }
}
