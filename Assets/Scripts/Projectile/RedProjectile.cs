using UnityEngine;

public class RedProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;

    [Header("Damage Primary")]
    [SerializeField] private int damageGreen = 1;
    
    [Header("Damage Second")]
    [SerializeField] private int damagePurple = 1;
    [SerializeField] private int damageHelminth = 1;
    
    [Header("Damage Third")]
    [SerializeField] private int damageRed = 1;
    [SerializeField] private int damageMushroom = 1;
    [SerializeField] private int damageProtozoa = 1;
    

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
        // TARGET UTAMA : BACTERIA GREEN
        // ===============================
        /*if (other.TryGetComponent<BacteriaControllerGreen>(out var green))
        {
            green.ProjectileDead();   // 💀 mati langsung
            Destroy(gameObject);
            return;
        }*/
        
        if (other.TryGetComponent<BacteriaControllerGreen>(out var green))
        {
            SoundManager.Instance?.PlayRedHit();
            green.ProjectileHit(damageGreen);
            Destroy(gameObject);
            return;
        }
        
        // ===============================
        // TARGET LAIN (PAKAI DAMAGE)
        // ===============================
        if (other.TryGetComponent<BacteriaControllerRed>(out var red))
        {
            SoundManager.Instance?.PlayRedHit();
            red.ProjectileHit(damageRed);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerPurple>(out var purple))
        {
            SoundManager.Instance?.PlayRedHit();
            purple.ProjectileHit(damagePurple);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<MushroomController>(out var mushroom))
        {
            SoundManager.Instance?.PlayRedHit();
            mushroom.ProjectileHit(damageMushroom);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<ProtozoaController>(out var protozoa))
        {
            SoundManager.Instance?.PlayRedHit();
            protozoa.ProjectileHit(damageProtozoa);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<HelminthController>(out var helminth))
        {
            SoundManager.Instance?.PlayRedHit();
            helminth.ProjectileHit(damageHelminth);
            Destroy(gameObject);
            return;
        }
    }
    
    
    
}
