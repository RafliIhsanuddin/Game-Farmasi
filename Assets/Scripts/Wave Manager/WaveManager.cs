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

    private void Start()
    {
        // 🔴 INTEGRASI AUDIO & TIMESCALE
        Time.timeScale = 1f;
        if (SoundManager.Instance != null)
            SoundManager.Instance.ResetForNewGame();

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

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayBgmLoop();

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
        int cumulative = 0;
        flagGoals.Clear();

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
        if (flagContainer == null || flagPrefab == null) return;

        foreach (Transform child in flagContainer)
            Destroy(child.gameObject);

        foreach (int goal in flagGoals)
        {
            GameObject flagObj = Instantiate(flagPrefab, flagContainer);
            goalToFlag[goal] = flagObj.GetComponent<FlagsManager>();

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
        if (isGameOver) return;

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
        if (isGameOver) return;
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
