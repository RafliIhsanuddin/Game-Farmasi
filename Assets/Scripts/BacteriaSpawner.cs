using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    // COUNT ALL ALIVE
    // ===============================
    public int CountAllAlive()
    {
        int total = 0;
        total += FindObjectsByType<BacteriaControllerGreen>(FindObjectsSortMode.None).Length;
        total += FindObjectsByType<BacteriaControllerRed>(FindObjectsSortMode.None).Length;
        total += FindObjectsByType<BacteriaControllerPurple>(FindObjectsSortMode.None).Length;
        total += FindObjectsByType<BacteriaControllerBlue>(FindObjectsSortMode.None).Length;
        total += FindObjectsByType<BacteriaControllerOrange>(FindObjectsSortMode.None).Length;
        total += FindObjectsByType<BacteriaControllerPink>(FindObjectsSortMode.None).Length;
        total += FindObjectsByType<BacteriaControllerYellow>(FindObjectsSortMode.None).Length;
        total += FindObjectsByType<VirusController>(FindObjectsSortMode.None).Length;
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
        const float NO_FREE_LINE_RESYNC_AFTER = 2f;

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
                        SyncOccupiedStateFromScene();
                        noFreeLineTimer = 0f;
                    }
                    yield return null;
                    continue;
                }

                SpawnEnemyAndLock(free);
                spawned++;
                yield return new WaitForSeconds(delay);
            }
            else
            {
                SpawnEnemyNoLock(spawnPoints[Random.Range(0, spawnPoints.Count)]);
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
            if (p != null && !p.occupied)
                return p;
        return null;
    }

    // ===============================
    // GROUPED MODE
    // ===============================
    private IEnumerator SpawnGroupedPerLineFixedAmountCoroutine()
    {
        isSpawning = true;

        foreach (var line in spawnPoints)
            if (line != null)
                StartCoroutine(HandleLineGroupLoop(line));

        while (!cancelRequested && groupedSpawned < groupedTargetAmount)
            yield return null;

        cancelRequested = true;
        isSpawning = false;
        OnWaveSpawnComplete?.Invoke();
    }

    private IEnumerator HandleLineGroupLoop(SpawnPointSlot line)
    {
        while (!cancelRequested && groupedSpawned < groupedTargetAmount)
        {
            GameObject prefab = bacteriaPrefabs[Random.Range(0, bacteriaPrefabs.Count)];
            int count = Mathf.Min(Random.Range(minPerLine, maxPerLine + 1), groupedTargetAmount - groupedSpawned);
            float delay = Random.Range(minDelayPerLine, maxDelayPerLine);

            for (int i = 0; i < count; i++)
            {
                GameObject enemy = Instantiate(prefab, line.transform.position, Quaternion.identity);
                LinkSpawnPoint(line, enemy);
                groupedSpawned++;
                yield return new WaitForSeconds(delay);
            }

            yield return StartCoroutine(WaitUntilAllOfTypeInLineDead(line, prefab));
            yield return new WaitForSeconds(delayBetweenGroups);
        }
    }

    private IEnumerator WaitUntilAllOfTypeInLineDead(SpawnPointSlot line, GameObject prefab)
    {
        while (!cancelRequested && CountAliveOfTypeInLine(line, prefab) > 0)
            yield return new WaitForSeconds(0.5f);
    }

    // ===============================
    // CORE SCRIPT-BASED
    // ===============================
    private int CountAliveOfTypeInLine(SpawnPointSlot line, GameObject prefab)
    {
        int count = 0;

        void Count<T>() where T : MonoBehaviour
        {
            foreach (var e in FindObjectsByType<T>(FindObjectsSortMode.None))
            {
                var f = typeof(T).GetField("spawnPoint");
                if (f != null && (object)f.GetValue(e) == (object)line)
                    count++;
            }
        }

        if (prefab.GetComponent<BacteriaControllerGreen>()) Count<BacteriaControllerGreen>();
        if (prefab.GetComponent<BacteriaControllerRed>()) Count<BacteriaControllerRed>();
        if (prefab.GetComponent<BacteriaControllerPurple>()) Count<BacteriaControllerPurple>();
        if (prefab.GetComponent<BacteriaControllerBlue>()) Count<BacteriaControllerBlue>();
        if (prefab.GetComponent<BacteriaControllerOrange>()) Count<BacteriaControllerOrange>();
        if (prefab.GetComponent<BacteriaControllerPink>()) Count<BacteriaControllerPink>();
        if (prefab.GetComponent<BacteriaControllerYellow>()) Count<BacteriaControllerYellow>();
        if (prefab.GetComponent<VirusController>()) Count<VirusController>();
        if (prefab.GetComponent<MushroomController>()) Count<MushroomController>();
        if (prefab.GetComponent<ProtozoaController>()) Count<ProtozoaController>();
        if (prefab.GetComponent<HelminthController>()) Count<HelminthController>();

        return count;
    }

    private void CleanupDeadOccupancies()
    {
        if (slotToEnemy.Count == 0) return;

        List<SpawnPointSlot> toClear = null;

        foreach (var kv in slotToEnemy)
            if (kv.Key == null || kv.Value == null)
                (toClear ??= new List<SpawnPointSlot>()).Add(kv.Key);

        if (toClear == null) return;

        foreach (var slot in toClear)
        {
            slot?.ClearOccupied();
            slotToEnemy.Remove(slot);
        }
    }

    private void SpawnEnemyNoLock(SpawnPointSlot slot)
    {
        if (slot == null || bacteriaPrefabs.Count == 0) return;
        GameObject enemy = Instantiate(bacteriaPrefabs[Random.Range(0, bacteriaPrefabs.Count)], slot.transform.position, Quaternion.identity);
        LinkSpawnPoint(slot, enemy);
    }

    private void SpawnEnemyAndLock(SpawnPointSlot slot)
    {
        if (slot == null || slot.occupied || bacteriaPrefabs.Count == 0) return;

        GameObject enemy = Instantiate(bacteriaPrefabs[Random.Range(0, bacteriaPrefabs.Count)], slot.transform.position, Quaternion.identity);
        LinkSpawnPoint(slot, enemy);
        slot.SetOccupied(enemy);
        slotToEnemy[slot] = enemy;
    }

    private void ForceClearInvalidOccupiedSlots()
    {
        foreach (var slot in spawnPoints)
        {
            if (slot == null || !slot.occupied) continue;

            bool found = false;

            void Check<T>() where T : MonoBehaviour
            {
                foreach (var e in FindObjectsByType<T>(FindObjectsSortMode.None))
                {
                    var f = typeof(T).GetField("spawnPoint");
                    if (f != null && (object)f.GetValue(e) == (object)slot)
                    {
                        found = true;
                        return;
                    }
                }
            }

            Check<BacteriaControllerGreen>();
            Check<BacteriaControllerRed>();
            Check<BacteriaControllerPurple>();
            Check<BacteriaControllerBlue>();
            Check<BacteriaControllerOrange>();
            Check<BacteriaControllerPink>();
            Check<BacteriaControllerYellow>();
            Check<VirusController>();
            Check<MushroomController>();
            Check<ProtozoaController>();
            Check<HelminthController>();

            if (!found)
            {
                slot.ClearOccupied();
                slotToEnemy.Remove(slot);
            }
        }
    }

    private void SyncOccupiedStateFromScene()
    {
        slotToEnemy.Clear();

        foreach (var slot in spawnPoints)
            slot?.ClearOccupied();

        void Sync<T>() where T : MonoBehaviour
        {
            foreach (var e in FindObjectsByType<T>(FindObjectsSortMode.None))
            {
                var f = typeof(T).GetField("spawnPoint");
                if (f == null) continue;

                SpawnPointSlot slot = f.GetValue(e) as SpawnPointSlot;
                if (slot == null) continue;

                if (!slot.occupied)
                    slot.SetOccupied(e.gameObject);

                slotToEnemy[slot] = e.gameObject;
            }
        }

        Sync<BacteriaControllerGreen>();
        Sync<BacteriaControllerRed>();
        Sync<BacteriaControllerPurple>();
        Sync<BacteriaControllerBlue>();
        Sync<BacteriaControllerOrange>();
        Sync<BacteriaControllerPink>();
        Sync<BacteriaControllerYellow>();
        Sync<VirusController>();
        Sync<MushroomController>();
        Sync<ProtozoaController>();
        Sync<HelminthController>();
    }

    private void LinkSpawnPoint(SpawnPointSlot slot, GameObject obj)
    {
        if (obj.TryGetComponent(out BacteriaControllerGreen g)) g.spawnPoint = slot;
        if (obj.TryGetComponent(out BacteriaControllerRed r)) r.spawnPoint = slot;
        if (obj.TryGetComponent(out BacteriaControllerPurple p)) p.spawnPoint = slot;
        if (obj.TryGetComponent(out BacteriaControllerBlue b)) b.spawnPoint = slot;
        if (obj.TryGetComponent(out BacteriaControllerOrange o)) o.spawnPoint = slot;
        if (obj.TryGetComponent(out BacteriaControllerPink pi)) pi.spawnPoint = slot;
        if (obj.TryGetComponent(out BacteriaControllerYellow y)) y.spawnPoint = slot;
        if (obj.TryGetComponent(out VirusController v)) v.spawnPoint = slot;
        if (obj.TryGetComponent(out MushroomController m)) m.spawnPoint = slot;
        if (obj.TryGetComponent(out ProtozoaController pr)) pr.spawnPoint = slot;
        if (obj.TryGetComponent(out HelminthController h)) h.spawnPoint = slot;
    }
}
