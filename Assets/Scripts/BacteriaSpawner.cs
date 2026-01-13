using UnityEngine;
using System.Collections.Generic;
using System.Collections;



public class BacteriaSpawner : MonoBehaviour
{
    [Header("Spawner Data")]
    public List<GameObject> bacteriaPrefabs = new();
    public List<SpawnPointSlot> spawnPoints = new();

    [Header("Single Per Line")]
    [SerializeField] private float singleSpawnDelay = 1.5f;

    [Header("Multi Per Line")]
    [SerializeField] private float multiSpawnDelay = 1.0f;

    [Header("Grouped Per Line (MultiPerLineGrouped Versi B)")]
    [SerializeField] private int minPerLine = 5;
    [SerializeField] private int maxPerLine = 12;
    [SerializeField] private float minDelayPerLine = 0.5f;
    [SerializeField] private float maxDelayPerLine = 1.5f;
    [SerializeField] private float delayBetweenGroups = 1f;

    private bool isSpawning = false;
    private bool cancelRequested = false;

    private readonly Dictionary<SpawnPointSlot, GameObject> slotToEnemy = new();
    public System.Action OnWaveSpawnComplete;

    public bool IsSpawning => isSpawning;

    // ====== STATE UNTUK MODE GROUPED (Versi B) ======
    private int groupedTargetAmount = 0;
    private int groupedSpawned = 0;

    // ===============================
    // RANDOMIZER POOL INJECTOR
    // ===============================
    public void SetEnemyPool(List<GameObject> newPool)
    {
        bacteriaPrefabs = new List<GameObject>(newPool);
    }

    // ===============================
    // START WAVES
    // ===============================
    public void StartSinglePerLineWave(int amount)
    {
        if (isSpawning) return;
        cancelRequested = false;

        SyncOccupiedStateFromScene();
        StartCoroutine(SpawnWaveCoroutine(amount, singleSpawnDelay, true));
    }

    public void StartMultiPerLineWave(int amount)
    {
        if (isSpawning) return;
        cancelRequested = false;

        SyncOccupiedStateFromScene();
        StartCoroutine(SpawnWaveCoroutine(amount, multiSpawnDelay, false));
    }

    public void StartGroupedPerLineWave(int amount)
    {
        if (isSpawning) return;
        cancelRequested = false;

        SyncOccupiedStateFromScene();
        groupedTargetAmount = amount;
        groupedSpawned = 0;

        StartCoroutine(SpawnGroupedPerLineFixedAmountCoroutine());
    }

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

        float noFreeLineTimer = 0f;
        const float NO_FREE_LINE_RESYNC_AFTER = 2.0f;

        while (!cancelRequested && spawned < amount)
        {
            if (singlePerLine)
            {
                CleanupDeadOccupancies();
                ForceClearInvalidOccupiedSlots();

                SpawnPointSlot free = GetFreeLine();
                if (free == null)
                {
                    noFreeLineTimer += Time.deltaTime;

                    if (noFreeLineTimer >= NO_FREE_LINE_RESYNC_AFTER)
                    {
                        Debug.Log("[Spawner] SinglePerLine stuck? Resync...");
                        SyncOccupiedStateFromScene();
                        noFreeLineTimer = 0f;
                    }

                    yield return null;
                    continue;
                }

                noFreeLineTimer = 0f;
                SpawnEnemyAndLock(free);
                spawned++;
                yield return new WaitForSeconds(delay);
            }
            else
            {
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
    // GROUPED (VERSI B: per line, tunggu mati)
    // ===============================
    private IEnumerator SpawnGroupedPerLineFixedAmountCoroutine()
    {
        isSpawning = true;

        if (spawnPoints.Count == 0)
        {
            isSpawning = false;
            OnWaveSpawnComplete?.Invoke();
            yield break;
        }

        Debug.Log("[Spawner] Mode MultiPerLineGrouped (Versi B) aktif.");

        // Mulai loop per baris
        foreach (var line in spawnPoints)
        {
            if (line == null) continue;
            StartCoroutine(HandleLineGroupLoop(line));
        }

        // Tunggu sampai semua jumlah spawn tercapai
        while (!cancelRequested && groupedSpawned < groupedTargetAmount)
            yield return null;

        // Stop spawn baru
        cancelRequested = true;

        isSpawning = false;
        Debug.Log("[Spawner] Wave Grouped selesai di-spawn semua bakteri!");
        OnWaveSpawnComplete?.Invoke();
    }

    private IEnumerator HandleLineGroupLoop(SpawnPointSlot line)
    {
        while (!cancelRequested)
        {
            if (groupedSpawned >= groupedTargetAmount)
                yield break;

            if (bacteriaPrefabs.Count == 0)
            {
                yield return null;
                continue;
            }

            // Pilih tipe bakteri acak untuk baris ini
            GameObject selectedPrefab = bacteriaPrefabs[Random.Range(0, bacteriaPrefabs.Count)];

            // Tentukan jumlah bakteri di baris ini
            int perLineCount = Random.Range(minPerLine, maxPerLine + 1);

            // Clamp supaya tidak melebihi target global
            int remaining = groupedTargetAmount - groupedSpawned;
            if (perLineCount > remaining)
                perLineCount = remaining;

            // Kalau sudah tidak ada yang perlu di-spawn, keluar
            if (perLineCount <= 0)
                yield break;

            // Tentukan delay acak untuk baris ini
            float lineDelay = Random.Range(minDelayPerLine, maxDelayPerLine);

            Debug.Log($"[Spawner] {line.name}: Grup baru {perLineCount}x {selectedPrefab.name} | delay antar bakteri: {lineDelay:F2}s");

            // Spawn batch di baris ini
            for (int i = 0; i < perLineCount && !cancelRequested; i++)
            {
                GameObject newBacteria = Instantiate(selectedPrefab, line.transform.position, Quaternion.identity);
                LinkSpawnPoint(line, newBacteria);
                groupedSpawned++;

                if (groupedSpawned >= groupedTargetAmount)
                    break;

                yield return new WaitForSeconds(lineDelay);
            }

            // Tunggu semua bakteri tipe ini di baris ini mati sebelum lanjut grup baru
            if (!cancelRequested)
                yield return StartCoroutine(WaitUntilAllOfTypeInLineDead(line, selectedPrefab.name));

            // Jeda antar grup
            if (!cancelRequested)
                yield return new WaitForSeconds(delayBetweenGroups);
        }
    }

    private IEnumerator WaitUntilAllOfTypeInLineDead(SpawnPointSlot line, string prefabName)
    {
        while (!cancelRequested && CountAliveOfTypeInLine(line, prefabName) > 0)
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

        if (prefabName.Contains("Mushroom"))
        {
            var arr = FindObjectsByType<MushroomController>(FindObjectsSortMode.None);
            foreach (var b in arr)
                if (b.spawnPoint == line) count++;
        }

        if (prefabName.Contains("Protozoa"))
        {
            var arr = FindObjectsByType<ProtozoaController>(FindObjectsSortMode.None);
            foreach (var b in arr)
                if (b.spawnPoint == line) count++;
        }

        if (prefabName.Contains("Helminth"))
        {
            var arr = FindObjectsByType<HelminthController>(FindObjectsSortMode.None);
            foreach (var b in arr)
                if (b.spawnPoint == line) count++;
        }

        return count;
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
    }

    private void SpawnEnemyAndLock(SpawnPointSlot slot)
    {
        if (slot == null || bacteriaPrefabs.Count == 0) return;
        if (slot.occupied) return;

        GameObject prefab = bacteriaPrefabs[Random.Range(0, bacteriaPrefabs.Count)];
        GameObject enemy = Instantiate(prefab, slot.transform.position, Quaternion.identity);
        LinkSpawnPoint(slot, enemy);

        slot.SetOccupied(enemy);
        slotToEnemy[slot] = enemy;
    }

    // ===============================
    // OCCUPIED MAINTENANCE
    // ===============================
    private void CleanupDeadOccupancies()
    {
        if (slotToEnemy.Count == 0) return;

        List<SpawnPointSlot> toClear = null;

        foreach (var kv in slotToEnemy)
        {
            SpawnPointSlot slot = kv.Key;
            GameObject enemy = kv.Value;

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
                slot.ClearOccupied();

            slotToEnemy.Remove(slot);
        }
    }

    private void ForceClearInvalidOccupiedSlots()
    {
        foreach (var slot in spawnPoints)
        {
            if (slot == null) continue;
            if (!slot.occupied) continue;

            bool found = false;

            foreach (var g in FindObjectsByType<BacteriaControllerGreen>(FindObjectsSortMode.None))
                if (g.spawnPoint == slot) { found = true; break; }

            if (!found)
                foreach (var r in FindObjectsByType<BacteriaControllerRed>(FindObjectsSortMode.None))
                    if (r.spawnPoint == slot) { found = true; break; }

            if (!found)
                foreach (var p in FindObjectsByType<BacteriaControllerPurple>(FindObjectsSortMode.None))
                    if (p.spawnPoint == slot) { found = true; break; }

            if (!found)
                foreach (var m in FindObjectsByType<MushroomController>(FindObjectsSortMode.None))
                    if (m.spawnPoint == slot) { found = true; break; }

            if (!found)
                foreach (var pr in FindObjectsByType<ProtozoaController>(FindObjectsSortMode.None))
                    if (pr.spawnPoint == slot) { found = true; break; }

            if (!found)
                foreach (var h in FindObjectsByType<HelminthController>(FindObjectsSortMode.None))
                    if (h.spawnPoint == slot) { found = true; break; }

            if (!found)
            {
                Debug.LogWarning("[Spawner] Force cleared invalid slot.");
                slot.ClearOccupied();
                slotToEnemy.Remove(slot);
            }
        }
    }

    private void SyncOccupiedStateFromScene()
    {
        slotToEnemy.Clear();

        foreach (var slot in spawnPoints)
        {
            if (slot == null) continue;
            slot.ClearOccupied();
        }

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