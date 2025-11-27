using UnityEngine;
using System.Collections.Generic;

public class BacteriaSpawner : MonoBehaviour
{
    
    [Header("Spawner Settings")]
    public GameObject bacteriaPrefab;
    public List<SpawnPointSlot> spawnPoints = new List<SpawnPointSlot>();

    [Header("Spawn Timing")]
    public float firstSpawnDelay = 2f;
    public float repeatRate = 5f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnBacteria), firstSpawnDelay, repeatRate);
    }

    void SpawnBacteria()
    {
        // Filter semua spawn point yang kosong
        List<SpawnPointSlot> emptyPoints = new List<SpawnPointSlot>();
        foreach (var point in spawnPoints)
        {
            if (!point.occupied)
                emptyPoints.Add(point);
        }

        // Jika tidak ada titik kosong, hentikan spawn
        if (emptyPoints.Count == 0)
        {
            Debug.Log("[Spawner] Semua titik penuh!");
            return;
        }

        // Pilih titik kosong acak
        SpawnPointSlot selected = emptyPoints[Random.Range(0, emptyPoints.Count)];

        // Spawn bakteri di titik tersebut
        GameObject newBacteria = Instantiate(bacteriaPrefab, selected.transform.position, Quaternion.identity);
        BacteriaController controller = newBacteria.GetComponent<BacteriaController>();

        // Hubungkan hubungan dua arah
        selected.SetOccupied(newBacteria);
        controller.spawnPoint = selected;

        Debug.Log($"[Spawner] Spawn bakteri di {selected.name}");
    }
}
