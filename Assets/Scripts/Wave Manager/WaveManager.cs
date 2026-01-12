using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{

    public enum SpawnMode
    {
        SinglePerLine,
        MultiPerLine,
        GroupedPerLine
    }

    [Header("Flags")]
    public int flagCount = 1;

    [Header("Wave Configuration (AUTO = flag + 1)")]
    public List<int> enemiesPerWave = new();

    [Header("Spawn Mode per Wave")]
    public List<SpawnMode> spawnModesPerWave = new();

    [Header("UI")]
    public Slider levelProgress;
    public RectTransform flagContainer;
    public GameObject flagPrefab;

    [Header("Wave Transition UI")]
    [SerializeField] private float waveDelaySeconds = 5f;
    [SerializeField] private GameObject waveIncomingUI;

    [Header("Win Panel (Multiple UI)")]
    public List<GameObject> winUIs = new();

    [Header("Spawner")]
    public BacteriaSpawner spawner;

    private int currentWaveIndex = 0;
    private int remainingEnemiesInWave = 0;
    private int totalKills = 0;
    private int totalEnemiesInLevel = 0;

    private List<int> flagGoals = new();
    private Dictionary<int, FlagsManager> goalToFlag = new();
    private HashSet<int> triggeredGoals = new();

    private int pendingFlagGoal = -1;

    private bool isCheckingFailSafe = false;
    public static bool isGameOver = false;

    public System.Action OnLevelComplete;

    private void OnValidate()
    {
        int required = flagCount + 1;
        AutoResize(enemiesPerWave, required, 5);
        AutoResize(spawnModesPerWave, required, SpawnMode.SinglePerLine);
    }

    private void AutoResize<T>(List<T> list, int size, T defaultValue)
    {
        while (list.Count < size) list.Add(defaultValue);
        while (list.Count > size) list.RemoveAt(list.Count - 1);
    }

    private void Start()
    {
        foreach (var ui in winUIs)
            if (ui != null) ui.SetActive(false);

        if (waveIncomingUI != null)
            waveIncomingUI.SetActive(false);

        isGameOver = false;

        CalculateTotalEnemies();
        GenerateFlagGoals();
        SetupProgressUI();
        SetupFlags();

        StartWave(0);

        if (!isCheckingFailSafe)
            StartCoroutine(FailSafeChecker());
    }

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

        if (flagContainer != null)
        {
            foreach (Transform child in flagContainer)
                Destroy(child.gameObject);
        }

        if (flagContainer == null || flagPrefab == null) return;

        foreach (int goal in flagGoals)
        {
            GameObject flagObj = Instantiate(flagPrefab, flagContainer);
            FlagsManager fm = flagObj.GetComponent<FlagsManager>();
            goalToFlag[goal] = fm;

            float t = (float)goal / Mathf.Max(1, totalEnemiesInLevel);
            RectTransform rt = flagObj.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(t, 0.5f);
            rt.anchoredPosition = Vector2.zero;
        }
    }

    private void StartWave(int waveIndex)
    {
        currentWaveIndex = waveIndex;
        remainingEnemiesInWave = enemiesPerWave[waveIndex];

        if (spawner != null)
            spawner.StopAllSpawning();

        Debug.Log($"🚩 WAVE {waveIndex + 1} DIMULAI | Mode: {spawnModesPerWave[waveIndex]} | Spawn: {remainingEnemiesInWave}");

        if (SoundManager.Instance != null && currentWaveIndex > 0)
        {
            SoundManager.Instance.PlayWaveStartMusic();
        }

        switch (spawnModesPerWave[waveIndex])
        {
            case SpawnMode.SinglePerLine:
                spawner.StartSinglePerLineWave(remainingEnemiesInWave);
                break;

            case SpawnMode.MultiPerLine:
                spawner.StartMultiPerLineWave(remainingEnemiesInWave);
                break;

            case SpawnMode.GroupedPerLine:
                spawner.StartGroupedPerLineWave(remainingEnemiesInWave);
                break;
        }
    }

    public void RegisterKill()
    {
        if (isGameOver) return;

        totalKills++;
        remainingEnemiesInWave = Mathf.Max(remainingEnemiesInWave - 1, 0);

        if (levelProgress != null)
            levelProgress.value = totalKills;

        foreach (int goal in flagGoals)
        {
            if (totalKills >= goal && !triggeredGoals.Contains(goal))
            {
                pendingFlagGoal = goal;
                triggeredGoals.Add(goal);
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
            TryWin();
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

        if (pendingFlagGoal != -1 &&
            goalToFlag.TryGetValue(pendingFlagGoal, out var fm))
        {
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayWaveStartMusic();

            fm?.Expand();
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

            int alive = spawner != null ? spawner.CountAllAlive() : 0;

            if (remainingEnemiesInWave > 0 && alive == 0)
            {
                Debug.LogWarning("⚠️ FAILSAFE: remainingEnemiesInWave > 0 tapi alive == 0. Paksa wave selesai.");
                remainingEnemiesInWave = 0;
                OnWaveCompleted();
            }
        }

        isCheckingFailSafe = false;
    }

    private void TryWin()
    {
        if (totalKills < totalEnemiesInLevel) return;
        StartCoroutine(WaitAllEnemyDeadThenWin());
    }

    private IEnumerator WaitAllEnemyDeadThenWin()
    {
        while (spawner != null && spawner.CountAllAlive() > 0)
            yield return new WaitForSeconds(0.2f);

        TriggerWin();
    }

    private void TriggerWin()
    {
        if (isGameOver) return;

        isGameOver = true;

        if (spawner != null)
            spawner.StopAllSpawning();

        if (levelProgress != null)
            levelProgress.value = totalEnemiesInLevel;

        Debug.Log("🏆 LEVEL SELESAI");

        OnLevelComplete?.Invoke();

        foreach (var ui in winUIs)
            if (ui != null) ui.SetActive(true);

        Time.timeScale = 0f;
    }
    
}