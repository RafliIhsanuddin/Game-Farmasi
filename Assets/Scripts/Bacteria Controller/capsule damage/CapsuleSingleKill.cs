using UnityEngine;

public class CapsuleSingleKill : MonoBehaviour
{
    private bool hasKilled = false;
    private Collider2D capsuleCollider;

    private void Awake()
    {
        capsuleCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 🔒 Jika capsule sudah pernah membunuh, STOP total
        if (hasKilled) return;

        // 🔹 Cek apakah collider ini punya BacteriaControllerGreen
        if (!collision.TryGetComponent<BacteriaControllerGreen>(out BacteriaControllerGreen bacteria))
            return;

        // 🔥 Bunuh SATU bakteri saja
        hasKilled = true;
        bacteria.Hit();

        // 🔒 Matikan collider capsule agar tidak hit bakteri lain
        if (capsuleCollider != null)
            capsuleCollider.enabled = false;

        // 🔥 Capsule habis setelah membunuh
        Destroy(gameObject);
    }
}
