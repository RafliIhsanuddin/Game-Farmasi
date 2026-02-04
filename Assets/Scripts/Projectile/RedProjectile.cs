using UnityEngine;

public class RedProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;

    // =========================
    // 🦠 DAMAGE ALL BACTERIA
    // =========================
    [Header("Damage Bacteria")]
    private int damagePurple = 10;
    private int damageRed = 7;
    private int damageYellow = 7;
    private int damagePink = 0;
    private int damageOrange = 0;
    private int damageGreen = 0;
    private int damageBlue = 5;

    // =========================
    // 🧬 DAMAGE VIRUS
    // =========================
    [Header("Damage Virus")]
    [SerializeField] private int damageVirus = 1;

    // =========================
    // 👾 DAMAGE OTHER ENEMIES
    // =========================
    [Header("Damage Other Enemies")]
    [SerializeField] private int damageMushroom = 1;
    [SerializeField] private int damageProtozoa = 1;
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
        // =========================
        // 🦠 ALL BACTERIA (EXPLICIT)
        // =========================

        if (other.TryGetComponent<BacteriaControllerPurple>(out var purple))
        {
            SoundManager.Instance?.PlayRedHit();
            purple.ProjectileHit(damagePurple);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerRed>(out var red))
        {
            SoundManager.Instance?.PlayRedHit();
            red.ProjectileHit(damageRed);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerYellow>(out var yellow))
        {
            SoundManager.Instance?.PlayRedHit();
            yellow.ProjectileHit(damageYellow);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerPink>(out var pink))
        {
            SoundManager.Instance?.PlayRedHit();
            pink.ProjectileHit(damagePink);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerOrange>(out var orange))
        {
            SoundManager.Instance?.PlayRedHit();
            orange.ProjectileHit(damageOrange);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerGreen>(out var green))
        {
            SoundManager.Instance?.PlayRedHit();
            green.ProjectileHit(damageGreen);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerBlue>(out var blue))
        {
            SoundManager.Instance?.PlayRedHit();
            blue.ProjectileHit(damageBlue);
            Destroy(gameObject);
            return;
        }

        // =========================
        // 🧬 VIRUS
        // =========================

        if (other.TryGetComponent<VirusController>(out var virus))
        {
            SoundManager.Instance?.PlayRedHit();
            virus.ProjectileHit(damageVirus);
            Destroy(gameObject);
            return;
        }

        // =========================
        // 👾 OTHER ENEMIES
        // =========================

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
