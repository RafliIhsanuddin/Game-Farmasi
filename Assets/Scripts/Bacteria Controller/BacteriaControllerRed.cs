using UnityEngine;

public class BacteriaControllerRed : MonoBehaviour
{
    public float speed = 1.5f;

    [HideInInspector] public SpawnPointSlot spawnPoint;
    [SerializeField] private GameObject BacteriaRed;
    [SerializeField] private GameObject hitParticleEffect; // prefab efek partikel
    
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
        Debug.Log($"[BacteriaControllerRed] {name} terkena serangan CapsuleBlue pada posisi {transform.position}");
        
        isHit = true;
        speed = 0;
        
        if (myCollider != null)
        {
            myCollider.enabled = false;
            Debug.Log($"[BacteriaControllerPurple] Collider dinonaktifkan untuk {name}");
        }


        if (BacteriaRed != null)
            BacteriaRed.SetActive(false);

        if (hitParticleEffect != null)
        {
            GameObject effect = Instantiate(hitParticleEffect, transform.position, Quaternion.identity);
            effect.SetActive(true); // pastikan aktif

            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
                ps.Play(); // manual trigger

            Destroy(effect, 3f); // hapus efek setelah 3 detik
            Debug.Log($"[BacteriaControllerRed] Efek partikel ditampilkan untuk {name}");
        }

        if (spawnPoint != null)
        {
            spawnPoint.ClearOccupied();
        }
        
        var manager = FindFirstObjectByType<WaveManager>();
        if (manager != null)
            manager.RegisterKill();

        Debug.Log($"[BacteriaControllerRed] {name} akan dihapus dalam 3 detik");
        Destroy(gameObject, 3f);
    }
}
