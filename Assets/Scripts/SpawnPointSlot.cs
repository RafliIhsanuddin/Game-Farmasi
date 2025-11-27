using UnityEngine;

public class SpawnPointSlot : MonoBehaviour
{
    [SerializeField] public bool occupied = false;
    [SerializeField] public GameObject currentBacteria = null;

    // Tandai bahwa titik ini sedang terisi bakteri
    public void SetOccupied(GameObject bacteria)
    {
        occupied = true;
        currentBacteria = bacteria;
    }

    // Tandai titik ini kosong kembali
    public void ClearOccupied()
    {
        occupied = false;
        currentBacteria = null;
    }

    // Debug visual di Scene
    private void OnDrawGizmos()
    {
        Gizmos.color = occupied ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.25f);
    }
}
