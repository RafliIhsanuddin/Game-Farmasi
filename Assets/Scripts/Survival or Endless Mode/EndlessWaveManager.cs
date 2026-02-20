using UnityEngine;
using System.Collections;

public class EndlessWaveManager : MonoBehaviour
{
    public static EndlessWaveManager Instance;

    private void Awake()
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

        ui.SetupNewCycle(w1, w2, w3);

        spawner.SetCycle(cycleNum);

        yield return WaitAllEnemiesDead();

        // =====================================================
        // WAVE 1 START (NO FLAG, NO MUSIC)
        // =====================================================

        yield return RunWave(w1, 1, spawner.StartSinglePerLineWave);

        yield return WaitAllEnemiesDead();

        // =====================================================
        // WAVE 2 WARNING
        // =====================================================

        ui.ShowBigWave(true);

        SoundManager.Instance?.PlayBigWaveWarning();

        yield return new WaitForSeconds(bigWaveDelay);

        ui.ShowBigWave(false);

        // =====================================================
        // WAVE 2 START
        // FLAG CHANGE + WAVE START MUSIC SAME FRAME
        // =====================================================

        StartWaveFlagAndMusic(0);

        yield return RunWave(w2, 2, spawner.StartGroupedPerLineWave);

        yield return WaitAllEnemiesDead();

        // =====================================================
        // WAVE 3 WARNING
        // =====================================================

        ui.ShowBigWave(true);

        SoundManager.Instance?.PlayBigWaveWarning();

        yield return new WaitForSeconds(bigWaveDelay);

        ui.ShowBigWave(false);

        // =====================================================
        // WAVE 3 START
        // FLAG CHANGE + WAVE START MUSIC SAME FRAME
        // =====================================================

        StartWaveFlagAndMusic(1);

        yield return RunWave(w3, 3, spawner.StartGroupedPerLineWave);

        yield return WaitAllEnemiesDead();

        difficulty.AdvanceCycle();

        phase.StartCoroutine(phase.SmoothBackToSelect());
    }

    // =====================================================
    // FLAG CHANGE + MUSIC START (SAME FRAME)
    // =====================================================

    private void StartWaveFlagAndMusic(int flagIndex)
    {
        // CHANGE FLAG FIRST
        ui.MarkWaveComplete(flagIndex);

        // PLAY WAVE START MUSIC (SoundManager function name correct)
        SoundManager.Instance?.PlayWaveStartMusic();
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

        spawner.OnWaveSpawnComplete = () => spawnDone = true;

        startFunc(count);

        lastAlive = spawner.CountAllAlive();

        while (!IsGameOver)
        {
            int alive = spawner.CountAllAlive();

            int killedNow = lastAlive - alive;

            if (killedNow > 0)
            {
                totalKills += killedNow;

                ui.AddKills(killedNow);
            }

            lastAlive = alive;

            if (spawnDone && alive == 0)
                yield break;

            yield return new WaitForSeconds(checkInterval);
        }
    }

    // =====================================================
    // WAIT ALL DEAD
    // =====================================================

    private IEnumerator WaitAllEnemiesDead()
    {
        while (!IsGameOver && spawner.CountAllAlive() > 0)
            yield return new WaitForSeconds(checkInterval);
    }

    // =====================================================
    // GAME OVER
    // =====================================================

    public void TriggerGameOver()
    {
        if (IsGameOver) return;

        IsGameOver = true;

        spawner.StopAllSpawning();

        speedScaler?.ResetSpeed();

        StopAllCoroutines();

        ui.ResetUIForGameOver();
    }

    // =====================================================
    // RESET
    // =====================================================

    public void ResetEndlessState()
    {
        IsGameOver = false;

        difficulty.ResetDifficulty();
    }
}