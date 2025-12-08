using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BacteriaSpawner : MonoBehaviour
{
    
    [Header("Spawner Settings")]
    public List<GameObject> bacteriaPrefabs = new List<GameObject>();
    public List<SpawnPointSlot> spawnPoints = new List<SpawnPointSlot>();

    [Header("Spawn Timing")]
    public float spawnDelay = 1.5f;

    [Header("UI Progress")]
    public UnityEngine.UI.Slider progressBar;

    private int bacteriaToSpawn;
    private int bacteriaSpawned;
    private bool isSpawning = false;

    // 🔔 Event callback ke WaveManager
    public System.Action OnWaveSpawnComplete;

    void Update()
    {
        if (progressBar != null)
            progressBar.value = bacteriaSpawned;
    }

    public void StartWave(int amount)
    {
        if (isSpawning) return;

        bacteriaToSpawn = amount;
        bacteriaSpawned = 0;
        if (progressBar != null)
            progressBar.maxValue = bacteriaToSpawn;

        StartCoroutine(SpawnWaveCoroutine());
    }

    private IEnumerator SpawnWaveCoroutine()
    {
        isSpawning = true;

        while (bacteriaSpawned < bacteriaToSpawn)
        {
            SpawnSingleBacteria();
            bacteriaSpawned++;
            yield return new WaitForSeconds(spawnDelay);
        }

        isSpawning = false;
        Debug.Log("[Spawner] Wave selesai di-spawn semua bakteri!");
        OnWaveSpawnComplete?.Invoke(); // 🔔 Beri tahu WaveManager
    }

    private void SpawnSingleBacteria()
    {
        List<SpawnPointSlot> emptyPoints = new List<SpawnPointSlot>();
        foreach (var point in spawnPoints)
            if (!point.occupied)
                emptyPoints.Add(point);

        if (emptyPoints.Count == 0)
        {
            Debug.Log("[Spawner] Semua titik penuh!");
            return;
        }

        SpawnPointSlot selected = emptyPoints[Random.Range(0, emptyPoints.Count)];
        GameObject prefab = bacteriaPrefabs[Random.Range(0, bacteriaPrefabs.Count)];

        GameObject newBacteria = Instantiate(prefab, selected.transform.position, Quaternion.identity);
        selected.SetOccupied(newBacteria);

        Debug.Log($"[Spawner] Spawn {newBacteria.name} di {selected.name}");
    }

}
