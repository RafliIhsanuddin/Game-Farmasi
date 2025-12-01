using UnityEngine;

public class BacteriaController : MonoBehaviour
{

    public float speed = 1.5f;

    [HideInInspector] public SpawnPointSlot spawnPoint;
    [SerializeField] private GameObject BacteriaGreen;
    [SerializeField] private GameObject BacteriaRed;
    [SerializeField] private GameObject hitParticleEffect; // prefab efek partikel

    private void FixedUpdate()
    {
        transform.position -= new Vector3(speed, 0, 0);
    }

    public void Hit()
    {
        Debug.Log($"[BacteriaController] {name} terkena serangan CapsuleRed pada posisi {transform.position}");

        if (BacteriaGreen != null)
            BacteriaGreen.SetActive(false);
        if (BacteriaRed != null)
            BacteriaRed.SetActive(false);

        if (hitParticleEffect != null)
        {
            GameObject effect = Instantiate(hitParticleEffect, transform.position, Quaternion.identity);
            Destroy(effect, 3f);
            if (spawnPoint != null)
            {
                spawnPoint.ClearOccupied();
            }
            Debug.Log($"[BacteriaController] Efek partikel ditampilkan untuk {name}");
        }

        Debug.Log($"[BacteriaController] {name} akan dihapus dalam 3 detik");
        Destroy(gameObject, 3f);
        if (spawnPoint != null)
        {
            spawnPoint.ClearOccupied();
        }
    }
}
