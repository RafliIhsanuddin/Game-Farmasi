using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    public enum SpawnMode
    {
        SinglePerLine,
        MultiPerLine,
        MultiPerLineGrouped
    }

    [Header("Wave Configuration (Flags AUTO = waveCount - 1)")]
    public List<int> enemiesPerWave = new();

    [Header("Spawn Mode per Wave (AUTO sync with enemiesPerWave)")]
    public List<SpawnMode> spawnModesPerWave = new();

    [Header("UI")]
    public Slider levelProgress;
    public RectTransform flagContainer;
    public GameObject flagPrefab;

    [Header("Wave Transition UI")]
    [SerializeField] private float waveDelaySeconds = 5f;
    [SerializeField] private GameObject waveIncomingUI;

    [Header("Win Panel")]
    public List<GameObject> winUIs = new();

    [Header("Spawner")]
    public BacteriaSpawner spawner;

    [Header("Win Actions")]
    public List<GameObject> enableOnWin = new();
    public List<GameObject> disableOnWin = new();

    private int currentWaveIndex;
    private int remainingEnemiesInWave;
    private int totalKills;
    private int totalEnemiesInLevel;

    private List<int> flagGoals = new();
    private Dictionary<int, FlagsManager> goalToFlag = new();
    private HashSet<int> triggeredGoals = new();

    private int pendingFlagGoal = -1;

    private bool isCheckingFailSafe;
    public static bool isGameOver;

    public System.Action OnLevelComplete;

    // 🔴 NEW: track last sizes to detect which list changed
    private int lastEnemiesCount = -1;
    private int lastSpawnModesCount = -1;

    private void OnValidate()
    {
        SyncWaveListsBidirectional();
    }

    private void Start()
    {
        Time.timeScale = 1f;

        if (SoundManager.Instance != null)
            SoundManager.Instance.ResetForNewGame();

        foreach (var ui in winUIs)
            if (ui != null) ui.SetActive(false);

        if (waveIncomingUI != null)
            waveIncomingUI.SetActive(false);

        isGameOver = false;

        SyncWaveListsBidirectional();

        CalculateTotalEnemies();
        GenerateFlagGoals();
        SetupProgressUI();
        SetupFlags();

        StartWave(0);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayBgmLoop();

        if (!isCheckingFailSafe)
            StartCoroutine(FailSafeChecker());
    }

    // 🔴 FIXED: fully bidirectional sync with add AND remove support
    private void SyncWaveListsBidirectional()
    {
        if (enemiesPerWave == null)
            enemiesPerWave = new List<int>();

        if (spawnModesPerWave == null)
            spawnModesPerWave = new List<SpawnMode>();

        int enemiesCount = enemiesPerWave.Count;
        int spawnCount = spawnModesPerWave.Count;

        // First time initialization
        if (lastEnemiesCount == -1 && lastSpawnModesCount == -1)
        {
            int target = Mathf.Max(enemiesCount, spawnCount);

            if (target == 0)
                target = 1;

            ResizeEnemies(target);
            ResizeSpawnModes(target);
        }
        else
        {
            // Detect which list changed

            if (enemiesCount != lastEnemiesCount)
            {
                ResizeSpawnModes(enemiesCount);
            }
            else if (spawnCount != lastSpawnModesCount)
            {
                ResizeEnemies(spawnCount);
            }
        }

        lastEnemiesCount = enemiesPerWave.Count;
        lastSpawnModesCount = spawnModesPerWave.Count;
    }

    private void ResizeEnemies(int target)
    {
        if (target < 0) target = 0;

        while (enemiesPerWave.Count < target)
        {
            int last = enemiesPerWave.Count > 0 ? enemiesPerWave[enemiesPerWave.Count - 1] : 0;
            enemiesPerWave.Add(last);
        }

        while (enemiesPerWave.Count > target)
        {
            enemiesPerWave.RemoveAt(enemiesPerWave.Count - 1);
        }
    }

    private void ResizeSpawnModes(int target)
    {
        if (target < 0) target = 0;

        while (spawnModesPerWave.Count < target)
        {
            SpawnMode last = spawnModesPerWave.Count > 0
                ? spawnModesPerWave[spawnModesPerWave.Count - 1]
                : SpawnMode.SinglePerLine;

            spawnModesPerWave.Add(last);
        }

        while (spawnModesPerWave.Count > target)
        {
            spawnModesPerWave.RemoveAt(spawnModesPerWave.Count - 1);
        }
    }

    private void CalculateTotalEnemies()
    {
        totalEnemiesInLevel = 0;
        foreach (var n in enemiesPerWave)
            totalEnemiesInLevel += n;
    }

    private void GenerateFlagGoals()
    {
        int cumulative = 0;
        flagGoals.Clear();

        int waveCount = enemiesPerWave.Count;

        for (int i = 0; i < waveCount - 1; i++)
        {
            cumulative += enemiesPerWave[i];
            flagGoals.Add(cumulative);
        }
    }

    private void SetupProgressUI()
    {
        if (levelProgress == null) return;

        levelProgress.maxValue = totalEnemiesInLevel;
        levelProgress.value = 0;
    }

    private void SetupFlags()
    {
        goalToFlag.Clear();

        if (flagContainer == null || flagPrefab == null)
            return;

        Canvas.ForceUpdateCanvases();

        float width = flagContainer.rect.width;

        foreach (Transform child in flagContainer)
            Destroy(child.gameObject);

        foreach (int goal in flagGoals)
        {
            GameObject flagObj = Instantiate(flagPrefab, flagContainer);

            FlagsManager fm = flagObj.GetComponent<FlagsManager>();

            if (fm != null)
                goalToFlag.Add(goal, fm);

            RectTransform rt = flagObj.GetComponent<RectTransform>();

            // WAJIB: set ukuran dan scale sesuai permintaan
            rt.localScale = new Vector3(0.4f, 0.4f, 1f);
            rt.sizeDelta = new Vector2(30f, 60f);

            // pivot di kanan supaya positioning dari kanan
            rt.pivot = new Vector2(0.5f, 0.5f);

            // anchor di kanan
            rt.anchorMin = new Vector2(1f, 0.5f);
            rt.anchorMax = new Vector2(1f, 0.5f);

            // hitung normalized progress
            float normalized = (float)goal / Mathf.Max(1, totalEnemiesInLevel);

            // karena kanan → kiri, posisi negatif dari kanan
            float posX = -width * normalized;

            rt.anchoredPosition = new Vector2(posX, 0f);
        }
    }

    private void StartWave(int waveIndex)
    {
        currentWaveIndex = waveIndex;

        remainingEnemiesInWave = enemiesPerWave[waveIndex];

        if (spawner != null)
            spawner.StopAllSpawning();

        if (SoundManager.Instance != null && waveIndex > 0)
            SoundManager.Instance.PlayWaveStartMusic();

        switch (spawnModesPerWave[waveIndex])
        {
            case SpawnMode.SinglePerLine:
                spawner.StartSinglePerLineWave(remainingEnemiesInWave);
                break;

            case SpawnMode.MultiPerLine:
                spawner.StartMultiPerLineWave(remainingEnemiesInWave);
                break;

            case SpawnMode.MultiPerLineGrouped:
                spawner.StartGroupedPerLineWave(remainingEnemiesInWave);
                break;
        }
    }

    public void RegisterKill()
    {
        if (isGameOver)
            return;

        totalKills++;

        remainingEnemiesInWave--;

        if (levelProgress != null)
            levelProgress.value = totalKills;

        foreach (int goal in flagGoals)
        {
            if (totalKills >= goal && !triggeredGoals.Contains(goal))
            {
                triggeredGoals.Add(goal);
                pendingFlagGoal = goal;
                break;
            }
        }

        if (remainingEnemiesInWave <= 0)
            OnWaveCompleted();
    }

    private void OnWaveCompleted()
    {
        if (currentWaveIndex >= enemiesPerWave.Count - 1)
        {
            spawner.StopAllSpawning();
            TriggerWin();
            return;
        }

        StartCoroutine(WaveTransitionDelay(() =>
        {
            StartWave(currentWaveIndex + 1);
        }));
    }

    private IEnumerator WaveTransitionDelay(System.Action onComplete)
    {
        if (waveIncomingUI != null)
            waveIncomingUI.SetActive(true);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayBigWaveWarning();

        yield return new WaitForSeconds(waveDelaySeconds);

        if (waveIncomingUI != null)
            waveIncomingUI.SetActive(false);

        if (pendingFlagGoal != -1 && goalToFlag.TryGetValue(pendingFlagGoal, out var fm))
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayWaveStartMusic();

            fm.Expand();

            pendingFlagGoal = -1;
        }

        onComplete?.Invoke();
    }

    private IEnumerator FailSafeChecker()
    {
        isCheckingFailSafe = true;

        while (!isGameOver)
        {
            yield return new WaitForSeconds(0.5f);

            if (remainingEnemiesInWave > 0 && spawner.CountAllAlive() == 0)
            {
                remainingEnemiesInWave = 0;
                OnWaveCompleted();
            }
        }

        isCheckingFailSafe = false;
    }

    private void TriggerWin()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopBgmLoop();
            SoundManager.Instance.PlayWin();
        }

        foreach (var ui in winUIs)
            if (ui != null) ui.SetActive(true);

        foreach (var go in enableOnWin)
            if (go != null) go.SetActive(true);

        foreach (var go in disableOnWin)
            if (go != null) go.SetActive(false);

        Time.timeScale = 0f;
    }
}
