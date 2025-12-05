using UnityEngine;

public class Nucleus : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Jika terkena bakteri apapun (Green, Red, Purple)
        if (collision.TryGetComponent<BacteriaControllerGreen>(out var green))
        {
            green.Hit();
            Destroy(gameObject);
        }
        else if (collision.TryGetComponent<BacteriaControllerRed>(out var red))
        {
            red.Hit();
            Destroy(gameObject);
        }
        else if (collision.TryGetComponent<BacteriaControllerPurple>(out var purple))
        {
            purple.Hit();
            Destroy(gameObject);
        }
    }
}
