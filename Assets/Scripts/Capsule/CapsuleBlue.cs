using UnityEngine;

public class CapsuleBlue : MonoBehaviour
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
        // TARGET UTAMA: BAKTERI MERAH
        // ===============================
        if (collision.TryGetComponent<BacteriaControllerRed>(out BacteriaControllerRed red))
        {
            hasTriggered = true;
            red.Hit();          // 🔥 BAKTERI MERAH HANCUR
            CleanupCapsule();
            return;
        }

        // ===============================
        // OBJEK LAIN (TIDAK ADA EFEK)
        // ===============================
        if (collision.TryGetComponent<BacteriaControllerGreen>(out _) ||
            collision.TryGetComponent<BacteriaControllerPurple>(out _) ||
            collision.TryGetComponent<MushroomController>(out _) ||
            collision.TryGetComponent<ProtozoaController>(out _) ||
            collision.TryGetComponent<HelminthController>(out _))
        {
            hasTriggered = true;
            CleanupCapsule();   // ❌ tidak ada efek ke objek lain
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
