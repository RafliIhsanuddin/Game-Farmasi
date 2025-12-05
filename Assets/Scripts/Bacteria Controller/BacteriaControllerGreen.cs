using UnityEngine;

public class BacteriaControllerGreen : MonoBehaviour
{
    public float speed = 1.5f;

    [HideInInspector] public SpawnPointSlot spawnPoint;
    [SerializeField] private GameObject BacteriaGreen;
    [SerializeField] private GameObject hitParticleEffect; // prefab efek partikel
    
    private bool isHit = false;

    private void FixedUpdate()
    {
        
        if (isHit) return;
        
        transform.position -= new Vector3(speed, 0, 0);
    }

    public void Hit()
    {
        Debug.Log($"[BacteriaControllerGreen] {name} terkena serangan CapsuleRed pada posisi {transform.position}");

        isHit = true;
        speed = 0;

        if (BacteriaGreen != null)
            BacteriaGreen.SetActive(false);

        if (hitParticleEffect != null)
        {
            GameObject effect = Instantiate(hitParticleEffect, transform.position, Quaternion.identity);
            effect.SetActive(true); // pastikan aktif

            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
                ps.Play(); // manual trigger

            Destroy(effect, 3f); // hapus efek setelah 3 detik
            Debug.Log($"[BacteriaControllerGreen] Efek partikel ditampilkan untuk {name}");
        }

        if (spawnPoint != null)
        {
            spawnPoint.ClearOccupied();
        }

        Debug.Log($"[BacteriaControllerGreen] {name} akan dihapus dalam 3 detik");
        Destroy(gameObject, 3f);
    }
}
