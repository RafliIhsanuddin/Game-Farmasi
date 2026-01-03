using UnityEngine;

public class GreenProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;

    [Header("Damage Settings")]
    [SerializeField] private int damageRed = 1;
    [SerializeField] private int damagePurple = 1;
    [SerializeField] private int damageGreen = 1;
    [SerializeField] private int damageMushroom = 1;
    [SerializeField] private int damageHelminth = 1;

    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 25f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ===============================
        // TARGET UTAMA : PROTOZOA
        // ===============================
        if (other.TryGetComponent<ProtozoaController>(out var protozoa))
        {
            protozoa.ProjectileDead();   // 💀 mati langsung
            Destroy(gameObject);
            return;
        }

        // ===============================
        // TARGET LAIN (PAKAI DAMAGE)
        // ===============================
        if (other.TryGetComponent<BacteriaControllerRed>(out var red))
        {
            red.ProjectileHit(damageRed);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerPurple>(out var purple))
        {
            purple.ProjectileHit(damagePurple);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerGreen>(out var green))
        {
            green.ProjectileHit(damageGreen);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<MushroomController>(out var mushroom))
        {
            mushroom.ProjectileHit(damageMushroom);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<HelminthController>(out var helminth))
        {
            helminth.ProjectileHit(damageHelminth);
            Destroy(gameObject);
            return;
        }
    }
}
