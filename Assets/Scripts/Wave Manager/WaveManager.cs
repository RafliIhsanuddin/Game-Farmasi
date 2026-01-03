using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{

    // ===============================
    // INSPECTOR CONFIG
    // ===============================

    [Header("Flags")]
    [Tooltip("Jumlah bendera (checkpoint)")]
    public int flagCount = 1;

    [Header("Wave Configuration (AUTO = flag + 1)")]
    public List<int> enemiesPerWave = new();

    [Header("Spawn Mode per Wave")]
    public List<BacteriaSpawner.SpawnMode> spawnModesPerWave = new();

    [Header("UI")]
    public Slider levelProgress;
    public RectTransform flagContainer;
    public GameObject flagPrefab;

    [Header("UI Win Panel")]
    public GameObject winUI;

    [Header("Spawner")]
    public BacteriaSpawner spawner;

    // ===============================
    // INTERNAL STATE
    // ===============================

    private int currentWaveIndex = 0;
    private int remainingEnemiesInWave = 0;
    private int totalKills = 0;
    private int totalEnemiesInLevel = 0;

    private List<int> flagGoals = new();
    private Dictionary<int, FlagsManager> goalToFlag = new();
    private HashSet<int> triggeredGoals = new();

    private bool isCheckingFailSafe = false;
    public static bool isGameOver = false;

    public System.Action OnLevelComplete;

    // ===============================
    // UNITY EVENTS
    // ===============================

    private void OnValidate()
    {
        int requiredWaveCount = flagCount + 1;

        // Auto resize enemiesPerWave
        while (enemiesPerWave.Count < requiredWaveCount)
            enemiesPerWave.Add(5);

        while (enemiesPerWave.Count > requiredWaveCount)
            enemiesPerWave.RemoveAt(enemiesPerWave.Count - 1);

        // Auto resize spawnModes
        while (spawnModesPerWave.Count < requiredWaveCount)
            spawnModesPerWave.Add(BacteriaSpawner.SpawnMode.SinglePerLine);

        while (spawnModesPerWave.Count > requiredWaveCount)
            spawnModesPerWave.RemoveAt(spawnModesPerWave.Count - 1);
    }

    // ===============================
    // START
    // ===============================

    private void Start()
    {
        if (winUI != null) winUI.SetActive(false);
        isGameOver = false;

        CalculateTotalEnemies();
        GenerateFlagGoals();
        SetupProgressUI();
        SetupFlags();

        StartWave(0);
    }

    // ===============================
    // SETUP
    // ===============================

    private void CalculateTotalEnemies()
    {
        totalEnemiesInLevel = 0;
        foreach (var n in enemiesPerWave)
            totalEnemiesInLevel += n;
    }

    private void GenerateFlagGoals()
    {
        flagGoals.Clear();

        int cumulative = 0;
        for (int i = 0; i < flagCount; i++)
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

        foreach (Transform child in flagContainer)
            Destroy(child.gameObject);

        foreach (int goal in flagGoals)
        {
            GameObject flagObj = Instantiate(flagPrefab, flagContainer);
            var fm = flagObj.GetComponent<FlagsManager>();
            goalToFlag[goal] = fm;

            float t = (float)goal / totalEnemiesInLevel;
            var rt = flagObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(t, 0.5f);
            rt.anchorMax = new Vector2(t, 0.5f);
            rt.anchoredPosition = Vector2.zero;
        }
    }

    // ===============================
    // WAVE LOGIC
    // ===============================

    private void StartWave(int waveIndex)
    {
        currentWaveIndex = waveIndex;
        remainingEnemiesInWave = enemiesPerWave[waveIndex];

        var mode = spawnModesPerWave[waveIndex];
        spawner.Mode = mode;
        spawner.StartWave(remainingEnemiesInWave);

        Debug.Log($"🚩 WAVE {waveIndex + 1} DIMULAI | Spawn {remainingEnemiesInWave}");

        if (!isCheckingFailSafe)
            StartCoroutine(FailSafeChecker());
    }

    public void RegisterKill()
    {
        if (isGameOver) return;

        totalKills++;
        remainingEnemiesInWave = Mathf.Max(remainingEnemiesInWave - 1, 0);

        if (levelProgress != null)
            levelProgress.value = totalKills;

        // Flag trigger
        foreach (int goal in flagGoals)
        {
            if (totalKills >= goal && !triggeredGoals.Contains(goal))
            {
                triggeredGoals.Add(goal);
                if (goalToFlag.TryGetValue(goal, out var fm))
                    fm?.Expand();
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
            TryWin();
            return;
        }

        StartWave(currentWaveIndex + 1);
    }

    // ===============================
    // FAIL SAFE
    // ===============================

    private IEnumerator FailSafeChecker()
    {
        isCheckingFailSafe = true;

        while (!isGameOver)
        {
            yield return new WaitForSeconds(0.5f);

            if (remainingEnemiesInWave > 0 && spawner.CountAllAlive() == 0)
            {
                Debug.LogWarning("⚠️ FAILSAFE TRIGGERED");
                remainingEnemiesInWave = 0;
                OnWaveCompleted();
            }
        }

        isCheckingFailSafe = false;
    }

    // ===============================
    // WIN
    // ===============================

    private void TryWin()
    {
        if (totalKills < totalEnemiesInLevel) return;
        StartCoroutine(WaitAllEnemyDeadThenWin());
    }

    private IEnumerator WaitAllEnemyDeadThenWin()
    {
        while (spawner.CountAllAlive() > 0)
            yield return new WaitForSeconds(0.2f);

        TriggerWin();
    }

    private void TriggerWin()
    {
        if (isGameOver) return;

        isGameOver = true;
        spawner.StopAllSpawning();

        if (levelProgress != null)
            levelProgress.value = totalEnemiesInLevel;

        Debug.Log("🏆 LEVEL SELESAI");

        OnLevelComplete?.Invoke();

        if (winUI != null)
        {
            winUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    
}