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
    // GROUPED PER LINE SETTINGS (FIXED AMOUNT)
    // ===============================
    [Header("Grouped Per Line (Fixed Amount)")]
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

    private readonly Dictionary<SpawnPointSlot, GameObject> slotToEnemy = new();
    public System.Action OnWaveSpawnComplete;

    public bool IsSpawning => isSpawning;

    // ===============================
    // PUBLIC API (POOL INJECTOR)
    // ===============================
    public void SetEnemyPool(List<GameObject> newPool)
    {
        // make sure copy only, avoid ref
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
        StartCoroutine(SpawnGroupedPerLineFixedAmountCoroutine(amount));
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
                        Debug.LogWarning("[Spawner] SinglePerLine stuck? Resync...");
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
    // GROUPED (FIXED AMOUNT)
    // ===============================
    private IEnumerator SpawnGroupedPerLineFixedAmountCoroutine(int amount)
    {
        isSpawning = true;

        if (spawnPoints.Count == 0)
        {
            isSpawning = false;
            OnWaveSpawnComplete?.Invoke();
            yield break;
        }

        int spawned = 0;
        int lineIndex = 0;

        while (!cancelRequested && spawned < amount)
        {
            SpawnPointSlot line = spawnPoints[lineIndex % spawnPoints.Count];
            lineIndex++;

            if (line == null)
            {
                yield return null;
                continue;
            }

            yield return WaitUntilLineClearSafe(line);

            int groupCount = Random.Range(minPerLine, maxPerLine + 1);
            float delay = Random.Range(minDelayPerLine, maxDelayPerLine);

            groupCount = Mathf.Min(groupCount, amount - spawned);

            for (int i = 0; i < groupCount && !cancelRequested; i++)
            {
                CleanupDeadOccupancies();
                ForceClearInvalidOccupiedSlots();

                if (line.occupied)
                    yield return WaitUntilLineClearSafe(line);

                SpawnEnemyAndLock(line);
                spawned++;
                yield return new WaitForSeconds(delay);
            }

            if (!cancelRequested)
                yield return new WaitForSeconds(delayBetweenGroups);
        }

        isSpawning = false;
        OnWaveSpawnComplete?.Invoke();
    }

    private IEnumerator WaitUntilLineClearSafe(SpawnPointSlot line)
    {
        float stuckTimer = 0f;
        const float RESYNC_AFTER = 2.0f;

        while (!cancelRequested)
        {
            CleanupDeadOccupancies();
            ForceClearInvalidOccupiedSlots();

            if (line == null) yield break;
            if (!line.occupied) yield break;

            stuckTimer += 0.2f;
            if (stuckTimer >= RESYNC_AFTER)
            {
                Debug.LogWarning("[Spawner] Line stuck - resync.");
                SyncOccupiedStateFromScene();
                stuckTimer = 0f;
            }

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