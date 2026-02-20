using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    private int totalKills;
    private int lastAlive;

    public bool IsGameOver { get; private set; }

    // =====================================================
    // MAIN CYCLE
    // =====================================================
    public IEnumerator RunSingleCycle()
    {
        totalKills = 0;
        lastAlive = 0;

        int cycleNum = difficulty.CurrentCycle + 1;

        int w1 = difficulty.GetWave1Count();
        int w2 = difficulty.GetWave2Count();
        int w3 = difficulty.GetWave3Count();

        // UI handles ALL progress logic
        ui.SetupNewCycle(w1, w2, w3);

        spawner.SetCycle(cycleNum);

        yield return WaitAllEnemiesDead();

        // ========================
        // WAVE 1
        // ========================

        yield return RunWave(
            w1,
            1,
            spawner.StartSinglePerLineWave);

        ui.MarkWaveComplete(0);

        yield return WaitAllEnemiesDead();

        // ========================
        // WAVE 2
        // ========================

        ui.ShowBigWave(true);

        SoundManager.Instance?.PlayBigWaveWarning();

        yield return new WaitForSeconds(bigWaveDelay);

        ui.ShowBigWave(false);

        yield return RunWave(
            w2,
            2,
            spawner.StartGroupedPerLineWave);

        ui.MarkWaveComplete(1);

        yield return WaitAllEnemiesDead();

        // ========================
        // WAVE 3
        // ========================

        ui.ShowBigWave(true);

        SoundManager.Instance?.PlayBigWaveWarning();

        yield return new WaitForSeconds(bigWaveDelay);

        ui.ShowBigWave(false);

        yield return RunWave(
            w3,
            3,
            spawner.StartGroupedPerLineWave);

        ui.MarkWaveComplete(2);

        yield return WaitAllEnemiesDead();

        difficulty.AdvanceCycle();

        phase.StartCoroutine(
            phase.SmoothBackToSelect());
    }

    // =====================================================
    // RUN WAVE
    // =====================================================
    private IEnumerator RunWave(
        int count,
        int waveNumber,
        System.Action<int> startFunc)
    {
        spawner.StopAllSpawning();

        spawner.SetWaveNumber(waveNumber);

        speedScaler?.NotifyWaveStart(waveNumber);

        bool spawnDone = false;

        spawner.OnWaveSpawnComplete =
            () => spawnDone = true;

        startFunc(count);

        lastAlive =
            spawner.CountAllAlive();

        while (!IsGameOver)
        {
            int alive =
                spawner.CountAllAlive();

            int killedNow =
                lastAlive - alive;

            if (killedNow > 0)
            {
                totalKills += killedNow;

                // ONLY notify UI
                ui.AddKills(killedNow);
            }

            lastAlive = alive;

            if (spawnDone && alive == 0)
                yield break;

            yield return new WaitForSeconds(checkInterval);
        }
    }

    // =====================================================
    // WAIT HELPERS
    // =====================================================
    private IEnumerator WaitAllEnemiesDead()
    {
        while (!IsGameOver &&
               spawner.CountAllAlive() > 0)
        {
            yield return new WaitForSeconds(checkInterval);
        }
    }

    // =====================================================
    // GAME OVER
    // =====================================================
    public void TriggerGameOver()
    {
        if (IsGameOver)
            return;

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