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
    private bool cancelRequested = false;   // ✅ untuk hard-stop semua proses spawn
    private readonly List<Coroutine> runningLineCoroutines = new();

    public System.Action OnWaveSpawnComplete;

    public enum SpawnMode
    {
        SinglePerLine,
        MultiPerLine,
        MultiPerLineGrouped
    }

    // ✅ Accessor publik untuk WaveManager
    public SpawnMode Mode
    {
        get => spawnMode;
        set => spawnMode = value;
    }

    public void StartWave(int amount)
    {
        if (isSpawning) return;

        cancelRequested = false; // reset setiap mulai wave

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

    // ✅ Dipanggil saat level berakhir supaya tidak ada spawn baru
    public void StopAllSpawning()
    {
        cancelRequested = true;
        isSpawning = false;

        // Hentikan semua coroutine yang berjalan
        StopAllCoroutines();
        runningLineCoroutines.Clear();

        Debug.Log("[Spawner] StopAllSpawning() dipanggil. Semua proses spawn dihentikan.");
    }

    // Opsional: dipakai WaveManager untuk cek yang hidup
    public int CountAllAlive()
    {
        int total = 0;
        total += FindObjectsByType<BacteriaControllerGreen>(FindObjectsSortMode.None).Length;
        total += FindObjectsByType<BacteriaControllerPurple>(FindObjectsSortMode.None).Length;
        total += FindObjectsByType<BacteriaControllerRed>(FindObjectsSortMode.None).Length;
        return total;
    }

    private IEnumerator SpawnWaveCoroutine(int amount)
    {
        isSpawning = true;
        int spawned = 0;

        while (!cancelRequested && spawned < amount)
        {
            bool success = SpawnBacteriaAccordingToMode();
            if (success) spawned++;
            yield return new WaitForSeconds(spawnDelay);
        }

        isSpawning = false;
        Debug.Log("[Spawner] Wave selesai di-spawn semua bakteri!");
        OnWaveSpawnComplete?.Invoke();
    }

    private bool SpawnBacteriaAccordingToMode()
    {
        if (cancelRequested) return false;

        List<SpawnPointSlot> candidates = new List<SpawnPointSlot>();
        foreach (var point in spawnPoints)
        {
            if (spawnMode == SpawnMode.SinglePerLine)
            {
                if (!point.occupied) candidates.Add(point);
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

    private IEnumerator SpawnGroupedPerLineCoroutine(int amount)
    {
        isSpawning = true;
        Debug.Log("[Spawner] Mode MultiPerLineGrouped (independen per baris) aktif.");

        // Jalankan loop per baris yang bisa dihentikan dengan cancelRequested
        foreach (var line in spawnPoints)
        {
            var c = StartCoroutine(HandleLineGroupLoop(line));
            runningLineCoroutines.Add(c);
        }

        // Untuk konsistensi event “wave spawned complete”,
        // anggap wave selesai ketika set minimal sudah dimulai.
        yield return null;
        isSpawning = false;
        OnWaveSpawnComplete?.Invoke();
    }

    private IEnumerator HandleLineGroupLoop(SpawnPointSlot line)
    {
        while (!cancelRequested)
        {
            GameObject selectedPrefab = bacteriaPrefabs[Random.Range(0, bacteriaPrefabs.Count)];
            int perLineCount = Random.Range(minPerLine, maxPerLine + 1);
            float lineDelay = Random.Range(minDelayPerLine, maxDelayPerLine);

            Debug.Log($"[Spawner] {line.name}: Grup baru {perLineCount}x {selectedPrefab.name} | delay: {lineDelay:F2}s");

            for (int i = 0; i < perLineCount && !cancelRequested; i++)
            {
                GameObject newBacteria = Instantiate(selectedPrefab, line.transform.position, Quaternion.identity);
                LinkSpawnPoint(line, newBacteria);
                yield return new WaitForSeconds(lineDelay);
            }

            // Tunggu sampai grup jenis ini di baris tsb habis (kalau belum di-cancel)
            if (!cancelRequested)
                yield return WaitUntilAllOfTypeInLineDead(line, selectedPrefab.name);

            // jeda kecil antar grup
            if (!cancelRequested)
                yield return new WaitForSeconds(1.0f);
        }

        Debug.Log($"[Spawner] {line.name}: Loop dihentikan (cancelRequested).");
    }

    private IEnumerator WaitUntilAllOfTypeInLineDead(SpawnPointSlot line, string prefabName)
    {
        while (!cancelRequested && CountAliveOfTypeInLine(line, prefabName) > 0)
            yield return new WaitForSeconds(0.5f);

        if (!cancelRequested)
            Debug.Log($"[Spawner] {line.name}: Semua {prefabName} di baris ini sudah mati. Grup baru siap!");
    }

    private int CountAliveOfTypeInLine(SpawnPointSlot line, string prefabName)
    {
        int count = 0;

        if (prefabName.Contains("Green"))
        {
            var arr = FindObjectsByType<BacteriaControllerGreen>(FindObjectsSortMode.None);
            foreach (var b in arr) if (b.spawnPoint == line) count++;
        }
        if (prefabName.Contains("Purple"))
        {
            var arr = FindObjectsByType<BacteriaControllerPurple>(FindObjectsSortMode.None);
            foreach (var b in arr) if (b.spawnPoint == line) count++;
        }
        if (prefabName.Contains("Red"))
        {
            var arr = FindObjectsByType<BacteriaControllerRed>(FindObjectsSortMode.None);
            foreach (var b in arr) if (b.spawnPoint == line) count++;
        }

        return count;
    }

}
