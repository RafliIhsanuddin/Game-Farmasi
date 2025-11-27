using UnityEngine;

public class BacteriaController : MonoBehaviour
{
    public float speed = 1.5f;
    public float health = 3f;

    [HideInInspector] public SpawnPointSlot spawnPoint;

    private bool isDead = false;

    private void FixedUpdate()
    {
        if (isDead) return;
        transform.position -= new Vector3(speed, 0, 0);
    }

    public void TakeDamage(float damage = 1f)
    {
        if (isDead) return;

        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Kosongkan slot spawn point asalnya
        if (spawnPoint != null)
        {
            spawnPoint.ClearOccupied();
            Debug.Log($"[Bacteria] Slot {spawnPoint.name} dikosongkan kembali.");
        }

        Destroy(gameObject);
    }
}
