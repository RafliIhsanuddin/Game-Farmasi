using UnityEngine;

public class SpawnPointSlot : MonoBehaviour
{
    public bool occupied = false;
    private GameObject currentEnemy;

    public void SetOccupied(GameObject enemy)
    {
        occupied = true;
        currentEnemy = enemy;
    }

    public void ClearOccupied()
    {
        occupied = false;
        currentEnemy = null;
    }
}
