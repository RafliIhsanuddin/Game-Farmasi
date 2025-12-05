using UnityEngine;

public class BacteriaControllerRed : MonoBehaviour
{
    public float speed = 1.5f;

    [HideInInspector] public SpawnPointSlot spawnPoint;
    [SerializeField] private GameObject BacteriaRed;
    [SerializeField] private GameObject hitParticleEffect; // prefab efek partikel
    
    private bool isHit = false;

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

        Debug.Log($"[BacteriaControllerRed] {name} akan dihapus dalam 3 detik");
        Destroy(gameObject, 3f);
    }
}
