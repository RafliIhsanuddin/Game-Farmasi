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

    [Header("Grouped Settings")]
    [Tooltip("Jumlah bakteri per baris untuk mode MultiPerLineGrouped (min–max)")]
    [SerializeField] private int minPerLine = 5;
    [SerializeField] private int maxPerLine = 12;

    [Header("Per-Line Delay (Mode Grouped)")]
    [Tooltip("Range acak delay antar-bakteri untuk setiap baris (hanya untuk MultiPerLineGrouped)")]
    [SerializeField] private float minDelayPerLine = 0.5f;
    [SerializeField] private float maxDelayPerLine = 1.5f;

    private bool isSpawning = false;
    public System.Action OnWaveSpawnComplete;

    public enum SpawnMode
    {
        SinglePerLine,
        MultiPerLine,
        MultiPerLineGrouped // independen per baris, dengan delay acak per line
    }

    public void StartWave(int amount)
    {
        if (isSpawning) return;

        spawnDelay = (spawnMode == SpawnMode.SinglePerLine)
            ? spawnDelaySinglePerLine
            : spawnDelayMultiPerLine;

        switch (spawnMode)
        {
            case SpawnMode.SinglePerLine:
            case SpawnMode.MultiPerLine:
                StartCoroutine(SpawnWaveCoroutine(amount));
                break;
            case SpawnMode.MultiPerLineGrouped:
                StartCoroutine(SpawnGroupedPerLineCoroutine(amount));
                break;
        }
    }

    // ---------------------------
    // MODE NORMAL (single / multi)
    // ---------------------------
    private IEnumerator SpawnWaveCoroutine(int amount)
    {
        isSpawning = true;
        int spawned = 0;

        while (spawned < amount)
        {
            bool success = SpawnBacteriaAccordingToMode();

            if (success)
                spawned++;

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

        LinkSpawnPoint(selected, newBacteria);

        if (spawnMode == SpawnMode.SinglePerLine)
            selected.SetOccupied(newBacteria);

        Debug.Log($"[Spawner] Spawn {newBacteria.name} di {selected.name} | Mode: {spawnMode}");
        return true;
    }

    private void LinkSpawnPoint(SpawnPointSlot slot, GameObject obj)
    {
        var g = obj.GetComponent<BacteriaControllerGreen>();
        if (g != null) g.spawnPoint = slot;

        var p = obj.GetComponent<BacteriaControllerPurple>();
        if (p != null) p.spawnPoint = slot;

        var r = obj.GetComponent<BacteriaControllerRed>();
        if (r != null) r.spawnPoint = slot;
    }

    // ---------------------------
    // MODE BARU: MultiPerLineGrouped (independen per baris)
    // ---------------------------
    private IEnumerator SpawnGroupedPerLineCoroutine(int amount)
    {
        isSpawning = true;
        Debug.Log("[Spawner] Mode MultiPerLineGrouped (independen per baris) aktif.");

        // Jalankan loop terpisah untuk setiap baris
        foreach (var line in spawnPoints)
        {
            StartCoroutine(HandleLineGroupLoop(line));
        }

        // Tunggu hingga semua baris selesai (opsional)
        while (CountAllAlive() > 0)
            yield return null;

        isSpawning = false;
        Debug.Log("[Spawner] Semua baris selesai menjalankan grup mereka.");
        OnWaveSpawnComplete?.Invoke();
    }

    private IEnumerator HandleLineGroupLoop(SpawnPointSlot line)
    {
        int totalSpawned = 0;

        while (true)
        {
            // Pilih tipe bakteri acak untuk baris ini
            GameObject selectedPrefab = bacteriaPrefabs[Random.Range(0, bacteriaPrefabs.Count)];

            // Tentukan jumlah bakteri di baris ini
            int perLineCount = Random.Range(minPerLine, maxPerLine + 1);

            // Tentukan delay acak untuk baris ini
            float lineDelay = Random.Range(minDelayPerLine, maxDelayPerLine);

            Debug.Log($"[Spawner] {line.name}: Grup baru {perLineCount}x {selectedPrefab.name} | delay antar bakteri: {lineDelay:F2}s");

            // Spawn batch di baris ini
            for (int i = 0; i < perLineCount; i++)
            {
                GameObject newBacteria = Instantiate(selectedPrefab, line.transform.position, Quaternion.identity);
                LinkSpawnPoint(line, newBacteria);
                totalSpawned++;
                yield return new WaitForSeconds(lineDelay);
            }

            // Tunggu semua bakteri batch ini mati sebelum lanjut grup baru
            yield return StartCoroutine(WaitUntilAllOfTypeInLineDead(line, selectedPrefab.name));

            // Jeda antar grup
            yield return new WaitForSeconds(1.0f);
        }
    }

    private IEnumerator WaitUntilAllOfTypeInLineDead(SpawnPointSlot line, string prefabName)
    {
        while (CountAliveOfTypeInLine(line, prefabName) > 0)
            yield return new WaitForSeconds(0.5f);

        Debug.Log($"[Spawner] {line.name}: Semua {prefabName} di baris ini sudah mati. Grup baru siap!");
    }

    private int CountAliveOfTypeInLine(SpawnPointSlot line, string prefabName)
    {
        int count = 0;

        if (prefabName.Contains("Green"))
        {
            var arr = FindObjectsByType<BacteriaControllerGreen>(FindObjectsSortMode.None);
            foreach (var b in arr)
                if (b.spawnPoint == line) count++;
        }

        if (prefabName.Contains("Purple"))
        {
            var arr = FindObjectsByType<BacteriaControllerPurple>(FindObjectsSortMode.None);
            foreach (var b in arr)
                if (b.spawnPoint == line) count++;
        }

        if (prefabName.Contains("Red"))
        {
            var arr = FindObjectsByType<BacteriaControllerRed>(FindObjectsSortMode.None);
            foreach (var b in arr)
                if (b.spawnPoint == line) count++;
        }

        return count;
    }

    private int CountAllAlive()
    {
        int total = 0;
        total += FindObjectsByType<BacteriaControllerGreen>(FindObjectsSortMode.None).Length;
        total += FindObjectsByType<BacteriaControllerPurple>(FindObjectsSortMode.None).Length;
        total += FindObjectsByType<BacteriaControllerRed>(FindObjectsSortMode.None).Length;
        return total;
    }

}
