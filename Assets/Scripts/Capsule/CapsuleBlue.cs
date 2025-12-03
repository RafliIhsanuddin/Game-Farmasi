using UnityEngine;

public class CapsuleBlue : MonoBehaviour
{
    public Tile ownerTile; // ini yang GameManager isi setelah spawn kapsul

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<BacteriaControllerRed>(out BacteriaControllerRed bacteria))
        {
            bacteria.Hit();

            // beri tahu tile bahwa kapsul hilang
            if (ownerTile != null)
            {
                ownerTile.hasCapsule = false;
                ownerTile.currentCapsule = null;
            }

            Destroy(gameObject);
        }
        else if (collision.GetComponent<BacteriaControllerGreen>() != null ||
                 collision.GetComponent<BacteriaControllerPurple>() != null)
        {
            if (ownerTile != null)
            {
                ownerTile.hasCapsule = false;
                ownerTile.currentCapsule = null;
            }

            Destroy(gameObject);
        }
    }
}
