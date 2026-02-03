using UnityEngine;

public class Nucleus : MonoBehaviour
{
    private bool hasTriggered = false;   // 🔒 pengaman satu kali trigger

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return;

        // ===============================
        // BAKTERI
        // ===============================
        if (collision.TryGetComponent<BacteriaControllerGreen>(out var green))
        {
            hasTriggered = true;
            green.Hit();
            Destroy(gameObject);
            return;
        }

        if (collision.TryGetComponent<BacteriaControllerYellow>(out var yellow))
        {
            hasTriggered = true;
            yellow.Hit();
            Destroy(gameObject);
            return;
        }

        if (collision.TryGetComponent<BacteriaControllerPink>(out var pink))
        {
            hasTriggered = true;
            pink.Hit();
            Destroy(gameObject);
            return;
        }

        if (collision.TryGetComponent<BacteriaControllerOrange>(out var orange))
        {
            hasTriggered = true;
            orange.Hit();
            Destroy(gameObject);
            return;
        }

        if (collision.TryGetComponent<BacteriaControllerBlue>(out var blue))
        {
            hasTriggered = true;
            blue.Hit();
            Destroy(gameObject);
            return;
        }

        if (collision.TryGetComponent<BacteriaControllerRed>(out var red))
        {
            hasTriggered = true;
            red.Hit();
            Destroy(gameObject);
            return;
        }

        if (collision.TryGetComponent<BacteriaControllerPurple>(out var purple))
        {
            hasTriggered = true;
            purple.Hit();
            Destroy(gameObject);
            return;
        }

        // ===============================
        // MUSUH LAIN
        // ===============================
        if (collision.TryGetComponent<MushroomController>(out var mushroom))
        {
            hasTriggered = true;
            mushroom.Hit();
            Destroy(gameObject);
            return;
        }

        if (collision.TryGetComponent<ProtozoaController>(out var protozoa))
        {
            hasTriggered = true;
            protozoa.Hit();
            Destroy(gameObject);
            return;
        }

        if (collision.TryGetComponent<HelminthController>(out var helminth))
        {
            hasTriggered = true;
            helminth.Hit();
            Destroy(gameObject);
            return;
        }
    }
}
