using UnityEngine;

public class CreamProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;

    // =========================
    // 🦠 DAMAGE ALL BACTERIA
    // =========================
    [Header("Damage Bacteria")]
    private int damagePurple = 4;
    private int damageRed = 7; 
    private int damageYellow = 4;
    private int damagePink = 20;
    private int damageOrange = 0;
    private int damageGreen = 1;
    private int damageBlue = 10;

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
        // 🦠 ALL BACTERIA
        // =========================

        if (other.TryGetComponent<BacteriaControllerPurple>(out var purple))
        {
            SoundManager.Instance?.PlayCreamHit();
            purple.ProjectileHit(damagePurple);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerRed>(out var red))
        {
            SoundManager.Instance?.PlayCreamHit();
            red.ProjectileHit(damageRed);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerYellow>(out var yellow))
        {
            SoundManager.Instance?.PlayCreamHit();
            yellow.ProjectileHit(damageYellow);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerPink>(out var pink))
        {
            SoundManager.Instance?.PlayCreamHit();
            pink.ProjectileHit(damagePink);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerOrange>(out var orange))
        {
            SoundManager.Instance?.PlayCreamHit();
            orange.ProjectileHit(damageOrange);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerGreen>(out var green))
        {
            SoundManager.Instance?.PlayCreamHit();
            green.ProjectileHit(damageGreen);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<BacteriaControllerBlue>(out var blue))
        {
            SoundManager.Instance?.PlayCreamHit();
            blue.ProjectileHit(damageBlue);
            Destroy(gameObject);
            return;
        }

        // =========================
        // 🧬 VIRUS
        // =========================
        if (other.TryGetComponent<VirusController>(out var virus))
        {
            SoundManager.Instance?.PlayCreamHit();
            virus.ProjectileHit(damageVirus);
            Destroy(gameObject);
            return;
        }

        // =========================
        // 👾 OTHER ENEMIES
        // =========================

        if (other.TryGetComponent<MushroomController>(out var mushroom))
        {
            SoundManager.Instance?.PlayCreamHit();
            mushroom.ProjectileHit(damageMushroom);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<ProtozoaController>(out var protozoa))
        {
            SoundManager.Instance?.PlayCreamHit();
            protozoa.ProjectileHit(damageProtozoa);
            Destroy(gameObject);
            return;
        }

        if (other.TryGetComponent<HelminthController>(out var helminth))
        {
            SoundManager.Instance?.PlayCreamHit();
            helminth.ProjectileHit(damageHelminth);
            Destroy(gameObject);
            return;
        }
    }
}
