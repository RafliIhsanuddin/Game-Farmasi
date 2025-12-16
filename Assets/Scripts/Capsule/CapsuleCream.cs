using UnityEngine;

public class CapsuleCream : MonoBehaviour
{
    public Tile ownerTile; // diisi GameManager saat spawn capsule

    private bool hasTriggered = false;   // 🔒 pengaman satu kali trigger
    private Collider2D capsuleCollider;

    private void Awake()
    {
        capsuleCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 🔒 Capsule hanya boleh bereaksi SATU KALI
        if (hasTriggered) return;

        // ===============================
        // TARGET UTAMA: JAMUR
        // ===============================
        if (collision.TryGetComponent<MushroomController>(out MushroomController mushroom))
        {
            hasTriggered = true;
            mushroom.Hit();     // 🔥 JAMUR HANCUR
            CleanupCapsule();
            return;
        }

        // ===============================
        // OBJEK LAIN (BAKTERI / DLL)
        // ===============================
        if (collision.TryGetComponent<BacteriaControllerGreen>(out _) ||
            collision.TryGetComponent<BacteriaControllerRed>(out _) ||
            collision.TryGetComponent<BacteriaControllerPurple>(out _))
        {
            hasTriggered = true;
            CleanupCapsule();   // ❌ tidak ada efek ke bakteri
            return;
        }
    }

    private void CleanupCapsule()
    {
        // 🔒 Matikan collider agar tidak trigger ulang
        if (capsuleCollider != null)
            capsuleCollider.enabled = false;

        // 🔹 Beri tahu tile bahwa capsule hilang
        if (ownerTile != null)
        {
            ownerTile.hasCapsule = false;
            ownerTile.currentCapsule = null;
        }

        // 🔥 Hancurkan capsule
        Destroy(gameObject);
    }
}
