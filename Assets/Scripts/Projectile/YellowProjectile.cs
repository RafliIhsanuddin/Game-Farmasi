using UnityEngine;

public class YellowProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;

    // =========================
    // 🦠 DAMAGE ALL BACTERIA
    // =========================
    [Header("Damage Bacteria")]
    private int damagePurple = 20;
    private int damageRed = 4;
    private int damageYellow = 10;
    private int damagePink = 4;
    private int damageOrange = 0;
    private int damageGreen = 1;
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
            SoundManager.Instance?.PlayYellowHit();
            purple.ProjectileHit(damagePurple);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerRed>(out var red))
        {
            SoundManager.Instance?.PlayYellowHit();
            red.ProjectileHit(damageRed);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerYellow>(out var yellow))
        {
            SoundManager.Instance?.PlayYellowHit();
            yellow.ProjectileHit(damageYellow);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerPink>(out var pink))
        {
            SoundManager.Instance?.PlayYellowHit();
            pink.ProjectileHit(damagePink);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerOrange>(out var orange))
        {
            SoundManager.Instance?.PlayYellowHit();
            orange.ProjectileHit(damageOrange);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerGreen>(out var green))
        {
            SoundManager.Instance?.PlayYellowHit();
            green.ProjectileHit(damageGreen);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerBlue>(out var blue))
        {
            SoundManager.Instance?.PlayYellowHit();
            blue.ProjectileHit(damageBlue);
            Destroy(gameObject);
            return;
        }

        // =========================
        // 🧬 VIRUS
        // =========================

        if (other.TryGetComponent<VirusController>(out var virus))
        {
            SoundManager.Instance?.PlayYellowHit();
            virus.ProjectileHit(damageVirus);
            Destroy(gameObject);
            return;
        }

        // =========================
        // 👾 OTHER ENEMIES
        // =========================

        if (other.TryGetComponent<MushroomController>(out var mushroom))
        {
            SoundManager.Instance?.PlayYellowHit();
            mushroom.ProjectileHit(damageMushroom);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<ProtozoaController>(out var protozoa))
        {
            SoundManager.Instance?.PlayYellowHit();
            protozoa.ProjectileHit(damageProtozoa);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<HelminthController>(out var helminth))
        {
            SoundManager.Instance?.PlayYellowHit();
            helminth.ProjectileHit(damageHelminth);
            Destroy(gameObject);
            return;
        }
    }
}
