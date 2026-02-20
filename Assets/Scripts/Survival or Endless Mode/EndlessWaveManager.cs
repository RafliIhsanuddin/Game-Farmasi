using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EndlessWaveManager : MonoBehaviour
{
    public static EndlessWaveManager Instance;

    void Awake()
    {
        Instance = this;
    }

    [Header("Core References")]
    [SerializeField] private BacteriaSpawnerEndless spawner;
    [SerializeField] private EndlessDifficultyScaler difficulty;
    [SerializeField] private EndlessUIController ui;
    [SerializeField] private EndlessPhaseController phase;
    [SerializeField] private EndlessSpeedScaler speedScaler;

    [Header("Timing")]
    [SerializeField] private float bigWaveDelay = 3f;
    [SerializeField] private float checkInterval = 0.1f;

    [Header("Progress UI")]
    [SerializeField] private Slider levelProgress;
    [SerializeField] private RectTransform flagContainer;
    [SerializeField] private GameObject flagPrefab;

    private int totalEnemiesInCycle;
    private int totalKills;
    private int totalSpawned;
    private int lastAlive;

    private List<int> flagGoals = new();
    private Dictionary<int, FlagsManager> goalToFlag = new();
    private HashSet<int> triggeredGoals = new();

    private int pendingFlagGoal = -1;

    public bool IsGameOver { get; private set; } = false;

    public IEnumerator RunSingleCycle()
    {
        totalKills = 0;
        totalSpawned = 0;
        lastAlive = 0;

        int cycleNum = difficulty.CurrentCycle + 1;
        int w1 = difficulty.GetWave1Count();
        int w2 = difficulty.GetWave2Count();

        totalEnemiesInCycle = w1 + w2;

        SetupProgressUI();
        GenerateFlagGoals(w1, w2);
        SetupFlags();

        ui.SetupNewCycle(w1, w2);

        spawner.SetCycle(cycleNum);

        yield return WaitAllEnemiesDead();

        yield return RunWave(w1, 1, spawner.StartSinglePerLineWave);

        ExpandFlagIfPending();

        ui.MarkWaveComplete(0);

        yield return WaitAllEnemiesDead();

        ui.ShowBigWave(true);
        SoundManager.Instance?.PlayBigWaveWarning();

        yield return new WaitForSeconds(bigWaveDelay);

        ui.ShowBigWave(false);

        yield return RunWave(w2, 2,
            difficulty.CurrentCycle == 0
                ? spawner.StartSinglePerLineWave
                : spawner.StartGroupedPerLineWave);

        ExpandFlagIfPending();

        ui.MarkWaveComplete(1);

        yield return WaitAllEnemiesDead();

        difficulty.AdvanceCycle();

        phase.StartCoroutine(phase.SmoothBackToSelect());
    }

    private void SetupProgressUI()
    {
        if (levelProgress == null) return;

        levelProgress.minValue = 0;
        levelProgress.maxValue = totalEnemiesInCycle;
        levelProgress.value = 0;
        levelProgress.direction = Slider.Direction.RightToLeft;
    }

    private void GenerateFlagGoals(int wave1, int wave2)
    {
        flagGoals.Clear();
        flagGoals.Add(wave1);
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

            rt.localScale = new Vector3(0.4f, 0.4f, 1f);
            rt.sizeDelta = new Vector2(30f, 60f);

            rt.anchorMin = new Vector2(1f, 0.5f);
            rt.anchorMax = new Vector2(1f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);

            float normalized = (float)goal / totalEnemiesInCycle;

            float posX = -width * normalized;

            rt.anchoredPosition = new Vector2(posX, 0f);
        }
    }

    private IEnumerator RunWave(int count, int waveNumber, System.Action<int> startFunc)
    {
        spawner.StopAllSpawning();

        spawner.SetWaveNumber(waveNumber);

        speedScaler?.NotifyWaveStart(waveNumber);

        bool spawnDone = false;

        spawner.OnWaveSpawnComplete = () => spawnDone = true;

        startFunc(count);

        totalSpawned += count;

        lastAlive = spawner.CountAllAlive();

        while (!IsGameOver)
        {
            int alive = spawner.CountAllAlive();

            int killedNow = lastAlive - alive;

            if (killedNow > 0)
            {
                totalKills += killedNow;

                levelProgress.value = totalKills;

                CheckFlagProgress();
            }

            lastAlive = alive;

            if (spawnDone && alive == 0)
                yield break;

            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void CheckFlagProgress()
    {
        foreach (int goal in flagGoals)
        {
            if (totalKills >= goal && !triggeredGoals.Contains(goal))
            {
                triggeredGoals.Add(goal);

                pendingFlagGoal = goal;

                break;
            }
        }
    }

    private void ExpandFlagIfPending()
    {
        if (pendingFlagGoal != -1)
        {
            if (goalToFlag.TryGetValue(pendingFlagGoal, out FlagsManager fm))
            {
                fm.Expand();
            }

            pendingFlagGoal = -1;
        }
    }

    private IEnumerator WaitAllEnemiesDead()
    {
        while (!IsGameOver && spawner.CountAllAlive() > 0)
            yield return new WaitForSeconds(checkInterval);
    }

    public void TriggerGameOver()
    {
        if (IsGameOver) return;

        IsGameOver = true;

        spawner.StopAllSpawning();

        speedScaler?.ResetSpeed();

        StopAllCoroutines();

        ui.ResetUIForGameOver();
    }

    public void ResetEndlessState()
    {
        IsGameOver = false;

        difficulty.ResetDifficulty();
    }
}