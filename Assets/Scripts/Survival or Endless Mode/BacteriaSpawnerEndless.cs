using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BacteriaSpawnerEndless : MonoBehaviour
{
    [Header("Spawner Data")]
    public List<GameObject> bacteriaPrefabs = new();
    public List<SpawnPointSlot> spawnPoints = new();

    [Header("Single Per Line")]
    [SerializeField] private float singleSpawnDelay = 1.5f;

    [Header("Multi Per Line")]
    [SerializeField] private float multiSpawnDelay = 1.0f;

    [Header("Grouped Per Line")]
    [SerializeField] private int minPerLine = 5;
    [SerializeField] private int maxPerLine = 12;
    [SerializeField] private float minDelayPerLine = 0.5f;
    [SerializeField] private float maxDelayPerLine = 1.5f;
    [SerializeField] private float delayBetweenGroups = 1f;

    [Header("Speed Scaler")]
    [SerializeField] private EndlessSpeedScaler speedScaler;

    public System.Action OnWaveSpawnComplete;

    private bool isSpawning = false;
    private bool cancelRequested = false;

    private int currentWave = 1;
    private int currentCycle = 1;

    private int groupedTargetAmount = 0;
    private int groupedSpawned = 0;

    // =====================================================
    // 🔗 PHASE → SPAWNER
    // =====================================================
    public void SetEnemyPool(List<GameObject> pool)
    {
        bacteriaPrefabs = new List<GameObject>(pool);
    }

    public void SetWaveNumber(int wave) => currentWave = wave;
    public void SetCycle(int cycle) => currentCycle = cycle;

    public void StopAllSpawning()
    {
        cancelRequested = true;
        isSpawning = false;
        StopAllCoroutines();
    }

    // =====================================================
    // 🔢 COUNT (WaveManager)
    // =====================================================
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

    // =====================================================
    // START WAVES (WaveManager)
    // =====================================================
    public void StartSinglePerLineWave(int amount)
    {
        if (isSpawning) return;
        cancelRequested = false;
        StartCoroutine(SinglePerLineRoutine(amount));
    }

    public void StartMultiPerLineWave(int amount)
    {
        if (isSpawning) return;
        cancelRequested = false;
        StartCoroutine(MultiPerLineRoutine(amount));
    }

    public void StartGroupedPerLineWave(int amount)
    {
        if (isSpawning) return;
        cancelRequested = false;

        groupedTargetAmount = amount;
        groupedSpawned = 0;

        StartCoroutine(GroupedPerLineRoutine());
    }

    // =====================================================
    // ROUTINES
    // =====================================================
    private IEnumerator SinglePerLineRoutine(int amount)
    {
        isSpawning = true;
        int spawned = 0;

        while (!cancelRequested && spawned < amount)
        {
            SpawnPointSlot free = GetFreeSlot();
            if (free == null)
            {
                yield return null;
                continue;
            }

            SpawnEnemy(free);
            spawned++;
            yield return new WaitForSeconds(singleSpawnDelay);
        }

        isSpawning = false;
        OnWaveSpawnComplete?.Invoke();
    }

    private IEnumerator MultiPerLineRoutine(int amount)
    {
        isSpawning = true;
        int spawned = 0;

        while (!cancelRequested && spawned < amount)
        {
            SpawnPointSlot slot = spawnPoints[Random.Range(0, spawnPoints.Count)];
            SpawnEnemy(slot);
            spawned++;
            yield return new WaitForSeconds(multiSpawnDelay);
        }

        isSpawning = false;
        OnWaveSpawnComplete?.Invoke();
    }

    private IEnumerator GroupedPerLineRoutine()
    {
        isSpawning = true;

        foreach (var line in spawnPoints)
        {
            if (line != null)
                StartCoroutine(GroupLine(line));
        }

        while (!cancelRequested && groupedSpawned < groupedTargetAmount)
            yield return null;

        isSpawning = false;
        OnWaveSpawnComplete?.Invoke();
    }

    private IEnumerator GroupLine(SpawnPointSlot line)
    {
        while (!cancelRequested && groupedSpawned < groupedTargetAmount)
        {
            int perLine = Random.Range(minPerLine, maxPerLine + 1);
            float delay = Random.Range(minDelayPerLine, maxDelayPerLine);

            for (int i = 0; i < perLine && groupedSpawned < groupedTargetAmount; i++)
            {
                SpawnEnemy(line);
                groupedSpawned++;
                yield return new WaitForSeconds(delay);
            }

            yield return new WaitForSeconds(delayBetweenGroups);
        }
    }

    // =====================================================
    // HELPERS
    // =====================================================
    private SpawnPointSlot GetFreeSlot()
    {
        foreach (var s in spawnPoints)
            if (s != null && !s.occupied)
                return s;
        return null;
    }

    private void SpawnEnemy(SpawnPointSlot slot)
    {
        if (slot == null || bacteriaPrefabs.Count == 0) return;

        GameObject prefab = bacteriaPrefabs[Random.Range(0, bacteriaPrefabs.Count)];
        GameObject enemy = Instantiate(prefab, slot.transform.position, Quaternion.identity);

        ApplySpeedScaling(enemy);
        LinkSpawnPoint(slot, enemy);
        slot.SetOccupied(enemy);
    }

    private void ApplySpeedScaling(GameObject enemy)
    {
        if (speedScaler == null) return;

        if (enemy.TryGetComponent(out BacteriaControllerGreen g)) g.speed = speedScaler.ApplyScale(g.speed);
        else if (enemy.TryGetComponent(out BacteriaControllerRed r)) r.speed = speedScaler.ApplyScale(r.speed);
        else if (enemy.TryGetComponent(out BacteriaControllerPurple p)) p.speed = speedScaler.ApplyScale(p.speed);
        else if (enemy.TryGetComponent(out BacteriaControllerBlue b)) b.speed = speedScaler.ApplyScale(b.speed);
        else if (enemy.TryGetComponent(out BacteriaControllerOrange o)) o.speed = speedScaler.ApplyScale(o.speed);
        else if (enemy.TryGetComponent(out BacteriaControllerPink pk)) pk.speed = speedScaler.ApplyScale(pk.speed);
        else if (enemy.TryGetComponent(out BacteriaControllerYellow y)) y.speed = speedScaler.ApplyScale(y.speed);
        else if (enemy.TryGetComponent(out VirusController v)) v.speed = speedScaler.ApplyScale(v.speed);
        else if (enemy.TryGetComponent(out MushroomController m)) m.speed = speedScaler.ApplyScale(m.speed);
        else if (enemy.TryGetComponent(out ProtozoaController pr)) pr.speed = speedScaler.ApplyScale(pr.speed);
        else if (enemy.TryGetComponent(out HelminthController h)) h.speed = speedScaler.ApplyScale(h.speed);
    }

    private void LinkSpawnPoint(SpawnPointSlot slot, GameObject obj)
    {
        if (obj.TryGetComponent(out BacteriaControllerGreen g)) g.spawnPoint = slot;
        if (obj.TryGetComponent(out BacteriaControllerRed r)) r.spawnPoint = slot;
        if (obj.TryGetComponent(out BacteriaControllerPurple p)) p.spawnPoint = slot;
        if (obj.TryGetComponent(out BacteriaControllerBlue b)) b.spawnPoint = slot;
        if (obj.TryGetComponent(out BacteriaControllerOrange o)) o.spawnPoint = slot;
        if (obj.TryGetComponent(out BacteriaControllerPink pk)) pk.spawnPoint = slot;
        if (obj.TryGetComponent(out BacteriaControllerYellow y)) y.spawnPoint = slot;
        if (obj.TryGetComponent(out VirusController v)) v.spawnPoint = slot;
        if (obj.TryGetComponent(out MushroomController m)) m.spawnPoint = slot;
        if (obj.TryGetComponent(out ProtozoaController pr)) pr.spawnPoint = slot;
        if (obj.TryGetComponent(out HelminthController h)) h.spawnPoint = slot;
    }
}
