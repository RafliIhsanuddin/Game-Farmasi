using UnityEngine;

public class CapsuleRed : MonoBehaviour
{
    public Tile ownerTile; // ini yang GameManager isi setelah spawn kapsul

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<BacteriaController>(out BacteriaController bacteria))
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
    }
}
