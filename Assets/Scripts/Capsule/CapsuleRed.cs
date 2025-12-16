using UnityEngine;

public class CapsuleRed : MonoBehaviour
{
    public Tile ownerTile; // diisi GameManager saat spawn capsule

    private bool hasTriggered = false;   // 🔒 pengaman utama
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
        // TARGET UTAMA: BAKTERI HIJAU
        // ===============================
        if (collision.TryGetComponent<BacteriaControllerGreen>(out BacteriaControllerGreen green))
        {
            hasTriggered = true;
            green.Hit();        // 🔥 kena efek
            CleanupCapsule();
            return;
        }

        // ===============================
        // BAKTERI LAIN (MERAH / UNGU)
        // ===============================
        if (collision.TryGetComponent<BacteriaControllerRed>(out _) ||
            collision.TryGetComponent<BacteriaControllerPurple>(out _))
        {
            hasTriggered = true;
            CleanupCapsule();   // ❌ tidak kena efek
            return;
        }

        // ===============================
        // JAMUR (TIDAK BEREFEK)
        // ===============================
        if (collision.TryGetComponent<MushroomController>(out _))
        {
            hasTriggered = true;
            CleanupCapsule();   // ❌ jamur tidak kena Hit()
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
