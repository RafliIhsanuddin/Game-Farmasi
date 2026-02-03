using UnityEngine;

public class OrangeProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;

    // =========================
    // 🦠 DAMAGE ALL BACTERIA
    // =========================
    [Header("Damage Bacteria")]
    [SerializeField] private int damagePurple = 1;
    [SerializeField] private int damageRed = 1;
    [SerializeField] private int damageYellow = 1;
    [SerializeField] private int damagePink = 1;
    [SerializeField] private int damageOrange = 1;
    [SerializeField] private int damageGreen = 1;
    [SerializeField] private int damageBlue = 1;

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
            SoundManager.Instance?.PlayOrangeHit();
            purple.ProjectileHit(damagePurple);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerRed>(out var red))
        {
            SoundManager.Instance?.PlayOrangeHit();
            red.ProjectileHit(damageRed);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerYellow>(out var yellow))
        {
            SoundManager.Instance?.PlayOrangeHit();
            yellow.ProjectileHit(damageYellow);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerPink>(out var pink))
        {
            SoundManager.Instance?.PlayOrangeHit();
            pink.ProjectileHit(damagePink);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerOrange>(out var orange))
        {
            SoundManager.Instance?.PlayOrangeHit();
            orange.ProjectileHit(damageOrange);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerGreen>(out var green))
        {
            SoundManager.Instance?.PlayOrangeHit();
            green.ProjectileHit(damageGreen);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerBlue>(out var blue))
        {
            SoundManager.Instance?.PlayOrangeHit();
            blue.ProjectileHit(damageBlue);
            Destroy(gameObject);
            return;
        }

        // =========================
        // 🧬 VIRUS
        // =========================

        if (other.TryGetComponent<VirusController>(out var virus))
        {
            SoundManager.Instance?.PlayOrangeHit();
            virus.ProjectileHit(damageVirus);
            Destroy(gameObject);
            return;
        }

        // =========================
        // 👾 OTHER ENEMIES
        // =========================

        if (other.TryGetComponent<MushroomController>(out var mushroom))
        {
            SoundManager.Instance?.PlayOrangeHit();
            mushroom.ProjectileHit(damageMushroom);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<ProtozoaController>(out var protozoa))
        {
            SoundManager.Instance?.PlayOrangeHit();
            protozoa.ProjectileHit(damageProtozoa);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<HelminthController>(out var helminth))
        {
            SoundManager.Instance?.PlayOrangeHit();
            helminth.ProjectileHit(damageHelminth);
            Destroy(gameObject);
            return;
        }
    }
}
