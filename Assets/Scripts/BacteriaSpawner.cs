using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BacteriaSpawner : MonoBehaviour
{
    
    [Header("Spawner Settings")]
    public List<GameObject> bacteriaPrefabs = new List<GameObject>();
    public List<SpawnPointSlot> spawnPoints = new List<SpawnPointSlot>();

    [Header("Spawn Timing")]
    [SerializeField] private float spawnDelaySinglePerLine = 1.5f;
    [SerializeField] private float spawnDelayMultiPerLine = 1.0f;
    private float spawnDelay;

    [Header("Spawn Mode")]
    [SerializeField] private SpawnMode spawnMode = SpawnMode.SinglePerLine;

    [Header("UI Progress (optional)")]
    public UnityEngine.UI.Slider progressBar;

    private int bacteriaToSpawn;
    private int bacteriaSpawned;
    private bool isSpawning = false;

    public System.Action OnWaveSpawnComplete;

    public enum SpawnMode
    {
        SinglePerLine,
        MultiPerLine
    }

    

    public void StartWave(int amount)
    {
        if (isSpawning) return;

        bacteriaToSpawn = amount;
        bacteriaSpawned = 0;

        spawnDelay = (spawnMode == SpawnMode.SinglePerLine) ? spawnDelaySinglePerLine : spawnDelayMultiPerLine;

        if (progressBar != null)
            progressBar.maxValue = bacteriaToSpawn;

        StartCoroutine(SpawnWaveCoroutine());
    }

    private IEnumerator SpawnWaveCoroutine()
    {
        isSpawning = true;

        while (bacteriaSpawned < bacteriaToSpawn)
        {
            bool success = SpawnBacteriaAccordingToMode();

            if (success)
                bacteriaSpawned++;

            yield return new WaitForSeconds(spawnDelay);
        }

        isSpawning = false;
        Debug.Log("[Spawner] Wave selesai di-spawn semua bakteri!");
        OnWaveSpawnComplete?.Invoke();
    }

    private bool SpawnBacteriaAccordingToMode()
    {
        List<SpawnPointSlot> candidates = new List<SpawnPointSlot>();

        foreach (var point in spawnPoints)
        {
            if (spawnMode == SpawnMode.SinglePerLine)
            {
                if (!point.occupied)
                    candidates.Add(point);
            }
            else
            {
                candidates.Add(point);
            }
        }

        if (candidates.Count == 0)
        {
            Debug.Log("[Spawner] Semua line penuh (mode SinglePerLine).");
            return false;
        }

        SpawnPointSlot selected = candidates[Random.Range(0, candidates.Count)];
        GameObject prefab = bacteriaPrefabs[Random.Range(0, bacteriaPrefabs.Count)];
        GameObject newBacteria = Instantiate(prefab, selected.transform.position, Quaternion.identity);

        // Set spawnPoint agar nanti bisa ClearOccupied saat mati
        var green = newBacteria.GetComponent<BacteriaControllerGreen>();
        if (green != null) green.spawnPoint = selected;

        var purple = newBacteria.GetComponent<BacteriaControllerPurple>();
        if (purple != null) purple.spawnPoint = selected;

        var red = newBacteria.GetComponent<BacteriaControllerRed>();
        if (red != null) red.spawnPoint = selected;

        if (spawnMode == SpawnMode.SinglePerLine)
            selected.SetOccupied(newBacteria);

        Debug.Log($"[Spawner] Spawn {newBacteria.name} di {selected.name} | Mode: {spawnMode}");
        return true;
    }

}
