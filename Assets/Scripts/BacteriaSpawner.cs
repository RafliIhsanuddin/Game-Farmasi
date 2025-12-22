using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BacteriaSpawner : MonoBehaviour
{
    
    // ===============================
    // SETTINGS
    // ===============================
    [Header("Spawner Settings")]
    public List<GameObject> bacteriaPrefabs = new();
    public List<SpawnPointSlot> spawnPoints = new();

    [Header("Spawn Timing")]
    [SerializeField] private float spawnDelaySinglePerLine = 1.5f;
    [SerializeField] private float spawnDelayMultiPerLine = 1.0f;
    private float spawnDelay;

    [Header("Spawn Mode")]
    [SerializeField] private SpawnMode spawnMode = SpawnMode.SinglePerLine;

    [Header("Grouped Settings")]
    [SerializeField] private int minPerLine = 5;
    [SerializeField] private int maxPerLine = 12;

    [Header("Per-Line Delay (Grouped)")]
    [SerializeField] private float minDelayPerLine = 0.5f;
    [SerializeField] private float maxDelayPerLine = 1.5f;

    // ===============================
    // INTERNAL STATE
    // ===============================
    private bool isSpawning = false;
    private bool cancelRequested = false;

    public System.Action OnWaveSpawnComplete;

    // ===============================
    // ENUM
    // ===============================
    public enum SpawnMode
    {
        SinglePerLine,
        MultiPerLine,
        MultiPerLineGrouped
    }

    public SpawnMode Mode
    {
        get => spawnMode;
        set => spawnMode = value;
    }

    // ===============================
    // START WAVE
    // ===============================
    public void StartWave(int amount)
    {
        if (isSpawning) return;

        cancelRequested = false;

        spawnDelay = (spawnMode == SpawnMode.SinglePerLine)
            ? spawnDelaySinglePerLine
            : spawnDelayMultiPerLine;

        if (spawnMode == SpawnMode.MultiPerLineGrouped)
        {
            StartCoroutine(SpawnGroupedPerLineCoroutine());
        }
        else
        {
            StartCoroutine(SpawnWaveCoroutine(amount));
        }
    }

    // ===============================
    // STOP ALL SPAWNING
    // ===============================
    public void StopAllSpawning()
    {
        cancelRequested = true;
        isSpawning = false;
        StopAllCoroutines();
        Debug.Log("[Spawner] Semua proses spawn dihentikan.");
    }

    // ===============================
    // COUNT ALL ALIVE (dipakai WaveManager)
    // ===============================
    public int CountAllAlive()
    {
        int total = 0;
        total += FindObjectsByType<BacteriaControllerGreen>(FindObjectsSortMode.None).Length;
        total += FindObjectsByType<BacteriaControllerRed>(FindObjectsSortMode.None).Length;
        total += FindObjectsByType<BacteriaControllerPurple>(FindObjectsSortMode.None).Length;
        total += FindObjectsByType<MushroomController>(FindObjectsSortMode.None).Length;
        total += FindObjectsByType<ProtozoaController>(FindObjectsSortMode.None).Length;
        total += FindObjectsByType<HelminthController>(FindObjectsSortMode.None).Length;
        return total;
    }

    // ===============================
    // NORMAL WAVE SPAWN
    // ===============================
    private IEnumerator SpawnWaveCoroutine(int amount)
    {
        isSpawning = true;
        int spawned = 0;

        while (!cancelRequested && spawned < amount)
        {
            if (SpawnOne())
                spawned++;

            yield return new WaitForSeconds(spawnDelay);
        }

        isSpawning = false;
        OnWaveSpawnComplete?.Invoke();
        Debug.Log("[Spawner] Wave selesai di-spawn.");
    }

    private bool SpawnOne()
    {
        if (cancelRequested) return false;

        List<SpawnPointSlot> candidates = new();

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
            return false;

        SpawnPointSlot selected = candidates[Random.Range(0, candidates.Count)];
        GameObject prefab = bacteriaPrefabs[Random.Range(0, bacteriaPrefabs.Count)];

        GameObject enemy = Instantiate(prefab, selected.transform.position, Quaternion.identity);
        LinkSpawnPoint(selected, enemy);

        if (spawnMode == SpawnMode.SinglePerLine)
            selected.SetOccupied(enemy);

        return true;
    }

    // ===============================
    // GROUPED PER LINE SPAWN (TERKONTROL)
    // ===============================
    private IEnumerator SpawnGroupedPerLineCoroutine()
    {
        isSpawning = true;

        foreach (var line in spawnPoints)
            StartCoroutine(HandleLineGroup(line));

        yield return null;
        isSpawning = false;
        OnWaveSpawnComplete?.Invoke();
        Debug.Log("[Spawner] Grouped wave dimulai.");
    }

    private IEnumerator HandleLineGroup(SpawnPointSlot line)
    {
        while (!cancelRequested)
        {
            GameObject prefab = bacteriaPrefabs[Random.Range(0, bacteriaPrefabs.Count)];
            int count = Random.Range(minPerLine, maxPerLine + 1);
            float delay = Random.Range(minDelayPerLine, maxDelayPerLine);

            // Spawn satu gelombang
            for (int i = 0; i < count && !cancelRequested; i++)
            {
                GameObject enemy = Instantiate(prefab, line.transform.position, Quaternion.identity);
                LinkSpawnPoint(line, enemy);
                yield return new WaitForSeconds(delay);
            }

            // 🔑 REM UTAMA (INI YANG MEMBUAT TERKONTROL)
            if (!cancelRequested)
                yield return WaitUntilAllOfTypeInLineDead(line, prefab.name);

            yield return new WaitForSeconds(1f);
        }
    }

    // ===============================
    // WAIT & COUNT PER LINE
    // ===============================
    private IEnumerator WaitUntilAllOfTypeInLineDead(SpawnPointSlot line, string prefabName)
    {
        while (!cancelRequested && CountAliveOfTypeInLine(line, prefabName) > 0)
            yield return new WaitForSeconds(0.5f);
    }

    private int CountAliveOfTypeInLine(SpawnPointSlot line, string prefabName)
    {
        int count = 0;

        if (prefabName.Contains("Green"))
        {
            foreach (var b in FindObjectsByType<BacteriaControllerGreen>(FindObjectsSortMode.None))
                if (b.spawnPoint == line) count++;
        }
        else if (prefabName.Contains("Red"))
        {
            foreach (var b in FindObjectsByType<BacteriaControllerRed>(FindObjectsSortMode.None))
                if (b.spawnPoint == line) count++;
        }
        else if (prefabName.Contains("Purple"))
        {
            foreach (var b in FindObjectsByType<BacteriaControllerPurple>(FindObjectsSortMode.None))
                if (b.spawnPoint == line) count++;
        }
        else if (prefabName.Contains("Mushroom"))
        {
            foreach (var m in FindObjectsByType<MushroomController>(FindObjectsSortMode.None))
                if (m.spawnPoint == line) count++;
        }
        else if (prefabName.Contains("Protozoa"))
        {
            foreach (var p in FindObjectsByType<ProtozoaController>(FindObjectsSortMode.None))
                if (p.spawnPoint == line) count++;
        }
        else if (prefabName.Contains("Helminth"))
        {
            foreach (var h in FindObjectsByType<HelminthController>(FindObjectsSortMode.None))
                if (h.spawnPoint == line) count++;
        }

        return count;
    }

    // ===============================
    // LINK SPAWN POINT
    // ===============================
    private void LinkSpawnPoint(SpawnPointSlot slot, GameObject obj)
    {
        if (obj.TryGetComponent(out BacteriaControllerGreen g)) g.spawnPoint = slot;
        if (obj.TryGetComponent(out BacteriaControllerRed r)) r.spawnPoint = slot;
        if (obj.TryGetComponent(out BacteriaControllerPurple p)) p.spawnPoint = slot;
        if (obj.TryGetComponent(out MushroomController m)) m.spawnPoint = slot;
        if (obj.TryGetComponent(out ProtozoaController pr)) pr.spawnPoint = slot;
        if (obj.TryGetComponent(out HelminthController h)) h.spawnPoint = slot;
    }

}
