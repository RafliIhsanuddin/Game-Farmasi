using UnityEngine;

public class CapsuleRed : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"CapsuleRed trigger dengan: {collision.name} [Layer: {LayerMask.LayerToName(collision.gameObject.layer)}]");

        if (collision.TryGetComponent<BacteriaController>(out BacteriaController bacteria))
        {
            Debug.Log($"[CapsuleRed] Mengenai bakteri: {bacteria.name} pada posisi {transform.position}");

            // Serang bakteri
            bacteria.Hit();

            // Hancurkan kapsul
            Destroy(gameObject);

            Debug.Log($"[CapsuleRed] CapsuleRed ({name}) hancur setelah mengenai bakteri {bacteria.name}");
        }
    }
    
}
