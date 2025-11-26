using UnityEngine;

public class Bacteria : MonoBehaviour
{
    public void TakeDamage()
    {
        Debug.Log("Bakteri kena obat!");
        Destroy(gameObject); // atau animasi
    }
}
