using UnityEngine;
using System.Collections.Generic;

public class ListCounterManager : MonoBehaviour
{
    [Header("Tracked Objects")]
    [SerializeField] private List<GameObject> trackedObjects = new List<GameObject>();

    [Header("Update Settings")]
    [SerializeField] private float updateInterval = 0.25f;

    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            CountAliveObjects();
        }
    }

    private void CountAliveObjects()
    {
        int aliveCount = 0;

        for (int i = trackedObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = trackedObjects[i];

            if (obj == null)
            {
                trackedObjects.RemoveAt(i);
                continue;
            }

            aliveCount++;
        }

        GameData.Data.TotalActiveObjects = aliveCount;
    }

    // === NEW: Final count for defeat ===
    public void ForceFinalCount()
    {
        CountAliveObjects();
        Debug.Log("[ListCounterManager] Final count applied on defeat. Total = " + GameData.Data.TotalActiveObjects);
    }

    public void AddObject(GameObject obj)
    {
        if (!trackedObjects.Contains(obj))
            trackedObjects.Add(obj);
    }

    public void RemoveObject(GameObject obj)
    {
        if (trackedObjects.Contains(obj))
            trackedObjects.Remove(obj);
    }
}