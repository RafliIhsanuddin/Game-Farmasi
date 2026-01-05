using UnityEngine;
using System.Collections.Generic;
using System.Collections;



public class BacteriaSpawner : MonoBehaviour
{
    // ===============================
    // SPAWNER DATA
    // ===============================
    [Header("Spawner Data")]
    public List<GameObject> bacteriaPrefabs = new();
    public List<SpawnPointSlot> spawnPoints = new();

    // ===============================
    // SINGLE PER LINE SETTINGS
    // ===============================
    [Header("Single Per Line")]
    [SerializeField] private float singleSpawnDelay = 1.5f;

    // ===============================
    // MULTI PER LINE SETTINGS
    // ===============================
    [Header("Multi Per Line")]
    [SerializeField] private float multiSpawnDelay = 1.0f;

    // ===============================
    // GROUPED PER LINE SETTINGS
    // ===============================
    [Header("Grouped Per Line")]
    [SerializeField] private int minPerLine = 5;
    [SerializeField] private int maxPerLine = 12;
    [SerializeField] private float minDelayPerLine = 0.5f;
    [SerializeField] private float maxDelayPerLine = 1.5f;
    [SerializeField] private float delayBetweenGroups = 1f;

    // ===============================
    // INTERNAL STATE
    // ===============================
    private bool isSpawning = false;
    private bool cancelRequested = false;

    // SinglePerLine / Grouped butuh lock tracking
    private readonly Dictionary<SpawnPointSlot, GameObject> slotToEnemy = new();

    public System.Action OnWaveSpawnComplete;

    // ===============================
    // PUBLIC API (DIPANGGIL WAVE MANAGER)
    // ===============================
    public void StartSinglePerLineWave(int amount)
    {
        if (isSpawning) return;
        cancelRequested = false;

        // pastikan status occupied sinkron saat mulai wave
        SyncOccupiedStateFromScene();

        StartCoroutine(SpawnWaveCoroutine(amount, singleSpawnDelay, singlePerLine: true));
    }

    public void StartMultiPerLineWave(int amount)
    {
        if (isSpawning) return;
        cancelRequested = false;

        // Multi tidak butuh occupied, tapi aman sync biar konsisten
        SyncOccupiedStateFromScene();

        StartCoroutine(SpawnWaveCoroutine(amount, multiSpawnDelay, singlePerLine: false));
    }

    public void StartGroupedPerLineWave()
    {
        if (isSpawning) return;
        cancelRequested = false;

        SyncOccupiedStateFromScene();

        StartCoroutine(SpawnGroupedPerLineCoroutine());
    }

    // ===============================
    // STOP
    // ===============================
    public void StopAllSpawning()
    {
        cancelRequested = true;
        isSpawning = false;
        StopAllCoroutines();
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
    // SINGLE & MULTI CORE
    // ===============================
    private IEnumerator SpawnWaveCoroutine(int amount, float delay, bool singlePerLine)
    {
        isSpawning = true;
        int spawned = 0;

        // loop spawn sampai amount terpenuhi
        while (!cancelRequested && spawned < amount)
        {
            if (singlePerLine)
            {
                // sebelum cari line kosong, bersihkan dulu slot yang enemy-nya sudah mati
                CleanupDeadOccupancies();

                SpawnPointSlot free = GetFreeLine();
                if (free == null)
                {
                    // semua line masih terisi → tunggu sebentar
                    yield return null;
                    continue;
                }

                // spawn dan LOCK line
                SpawnEnemyAndLock(free);
                spawned++;
                yield return new WaitForSeconds(delay);
            }
            else
            {
                // MultiPerLine: bebas spawn ke random slot (boleh numpuk)
                if (spawnPoints.Count == 0)
                {
                    yield return null;
                    continue;
                }

                SpawnPointSlot randomSlot = spawnPoints[Random.Range(0, spawnPoints.Count)];
                SpawnEnemyNoLock(randomSlot);
                spawned++;
                yield return new WaitForSeconds(delay);
            }
        }

        isSpawning = false;
        OnWaveSpawnComplete?.Invoke();
    }

    // Cari line yang benar-benar kosong
    private SpawnPointSlot GetFreeLine()
    {
        foreach (var p in spawnPoints)
        {
            if (p == null) continue;
            if (!p.occupied) return p;
        }
        return null;
    }

    // ===============================
    // GROUPED PER LINE
    // ===============================
    private IEnumerator SpawnGroupedPerLineCoroutine()
    {
        isSpawning = true;

        // per line jalan coroutine sendiri
        foreach (var line in spawnPoints)
        {
            if (line == null) continue;
            StartCoroutine(HandleLineGroup(line));
        }

        yield return null;
        isSpawning = false;
        OnWaveSpawnComplete?.Invoke();
    }

    private IEnumerator HandleLineGroup(SpawnPointSlot line)
    {
        while (!cancelRequested)
        {
            // sebelum mulai group baru, pastikan line benar-benar clear
            yield return WaitUntilLineClear(line);

            int count = Random.Range(minPerLine, maxPerLine + 1);
            float delay = Random.Range(minDelayPerLine, maxDelayPerLine);

            for (int i = 0; i < count && !cancelRequested; i++)
            {
                // Grouped = kita treat seperti "line locked" juga,
                // supaya di dalam 1 line ada tracking yang rapih
                CleanupDeadOccupancies();
                SpawnEnemyAndLock(line);
                yield return new WaitForSeconds(delay);
            }

            // tunggu sampai semua musuh di line itu habis
            yield return WaitUntilLineClear(line);

            if (!cancelRequested)
                yield return new WaitForSeconds(delayBetweenGroups);
        }
    }

    private IEnumerator WaitUntilLineClear(SpawnPointSlot line)
    {
        while (!cancelRequested)
        {
            CleanupDeadOccupancies();
            if (line == null) yield break;

            // jika tidak occupied berarti line clear
            if (!line.occupied)
                yield break;

            // kalau occupied tapi musuhnya sudah mati, CleanupDeadOccupancies akan unlock
            yield return new WaitForSeconds(0.2f);
        }
    }

    // ===============================
    // SPAWN HELPERS
    // ===============================
    private void SpawnEnemyNoLock(SpawnPointSlot slot)
    {
        if (slot == null || bacteriaPrefabs.Count == 0) return;

        GameObject prefab = bacteriaPrefabs[Random.Range(0, bacteriaPrefabs.Count)];
        GameObject enemy = Instantiate(prefab, slot.transform.position, Quaternion.identity);
        LinkSpawnPoint(slot, enemy);
        // tidak lock slot
    }

    // 🔑 Ini yang bikin "1 line = 1 enemy" beneran jalan
    private void SpawnEnemyAndLock(SpawnPointSlot slot)
    {
        if (slot == null || bacteriaPrefabs.Count == 0) return;

        // kalau slot masih occupied, jangan spawn (safety)
        if (slot.occupied) return;

        GameObject prefab = bacteriaPrefabs[Random.Range(0, bacteriaPrefabs.Count)];
        GameObject enemy = Instantiate(prefab, slot.transform.position, Quaternion.identity);
        LinkSpawnPoint(slot, enemy);

        // LOCK: pakai sistem slot bawaan kamu
        slot.SetOccupied(enemy);
        slotToEnemy[slot] = enemy;
    }

    // ===============================
    // OCCUPIED MAINTENANCE (NO NEW SCRIPT)
    // ===============================
    // Bersihkan slot yang enemy-nya sudah mati (destroyed)
    private void CleanupDeadOccupancies()
    {
        // collect keys dulu biar aman ketika modify dictionary
        if (slotToEnemy.Count == 0) return;

        List<SpawnPointSlot> toClear = null;

        foreach (var kv in slotToEnemy)
        {
            SpawnPointSlot slot = kv.Key;
            GameObject enemy = kv.Value;

            // slot sudah null (scene unload) atau enemy sudah destroyed
            if (slot == null || enemy == null)
            {
                toClear ??= new List<SpawnPointSlot>();
                toClear.Add(slot);
            }
        }

        if (toClear == null) return;

        for (int i = 0; i < toClear.Count; i++)
        {
            SpawnPointSlot slot = toClear[i];
            if (slot != null)
            {
                // kalau SpawnPointSlot kamu punya fungsi ClearOccupied(), pakai itu.
                // kalau tidak ada, set manual (sesuaikan field kamu).
                // Aku asumsikan ada: occupied + currentEnemy di dalam slot.
                slot.ClearOccupied();
            }
            slotToEnemy.Remove(slot);
        }
    }

    // Sinkronkan occupied saat mulai wave (kalau ada enemy dari wave sebelumnya masih hidup)
    private void SyncOccupiedStateFromScene()
    {
        slotToEnemy.Clear();

        // reset semua slot jadi tidak occupied dulu
        foreach (var slot in spawnPoints)
        {
            if (slot == null) continue;
            slot.ClearOccupied();
        }

        // cari semua enemy hidup, lalu lock slot sesuai spawnPoint mereka
        // (ini supaya "single per line" tidak rusak kalau ada enemy sisa)
        foreach (var g in FindObjectsByType<BacteriaControllerGreen>(FindObjectsSortMode.None))
            if (g.spawnPoint != null) ForceLock(g.spawnPoint, g.gameObject);

        foreach (var r in FindObjectsByType<BacteriaControllerRed>(FindObjectsSortMode.None))
            if (r.spawnPoint != null) ForceLock(r.spawnPoint, r.gameObject);

        foreach (var p in FindObjectsByType<BacteriaControllerPurple>(FindObjectsSortMode.None))
            if (p.spawnPoint != null) ForceLock(p.spawnPoint, p.gameObject);

        foreach (var m in FindObjectsByType<MushroomController>(FindObjectsSortMode.None))
            if (m.spawnPoint != null) ForceLock(m.spawnPoint, m.gameObject);

        foreach (var pr in FindObjectsByType<ProtozoaController>(FindObjectsSortMode.None))
            if (pr.spawnPoint != null) ForceLock(pr.spawnPoint, pr.gameObject);

        foreach (var h in FindObjectsByType<HelminthController>(FindObjectsSortMode.None))
            if (h.spawnPoint != null) ForceLock(h.spawnPoint, h.gameObject);
    }

    private void ForceLock(SpawnPointSlot slot, GameObject enemy)
    {
        if (slot == null || enemy == null) return;
        if (!slot.occupied) slot.SetOccupied(enemy);
        slotToEnemy[slot] = enemy;
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