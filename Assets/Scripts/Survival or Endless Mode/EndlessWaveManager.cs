using UnityEngine;
using System.Collections;

public class EndlessWaveManager : MonoBehaviour
{
    [Header("Core References")]
    [SerializeField] private BacteriaSpawnerEndless spawner;
    [SerializeField] private EndlessDifficultyScaler difficulty;
    [SerializeField] private EndlessUIController ui;
    [SerializeField] private EndlessPhaseController phase;
    [SerializeField] private EndlessSpeedScaler speedScaler;

    [Header("Timing")]
    [SerializeField] private float bigWaveDelay = 3f;
    [SerializeField] private float checkInterval = 0.25f;

    public bool IsGameOver { get; private set; } = false;

    public IEnumerator RunSingleCycle()
    {
        int cycleNum = difficulty.CurrentCycle + 1;
        int w1 = difficulty.GetWave1Count();
        int w2 = difficulty.GetWave2Count();

        ui.SetupNewCycle(w1, w2);
        spawner.SetCycle(cycleNum);

        yield return WaitAllEnemiesDead();
        yield return RunWave(w1, 1, spawner.StartSinglePerLineWave);
        ui.MarkWaveComplete(0);

        yield return WaitAllEnemiesDead();

        ui.ShowBigWave(true);
        SoundManager.Instance?.PlayBigWaveWarning();
        yield return new WaitForSeconds(bigWaveDelay);
        ui.ShowBigWave(false);

        yield return RunWave(w2, 2,
            difficulty.CurrentCycle == 0
                ? spawner.StartSinglePerLineWave
                : spawner.StartGroupedPerLineWave
        );

        ui.MarkWaveComplete(1);
        yield return WaitAllEnemiesDead();

        difficulty.AdvanceCycle();
        phase.StartCoroutine(phase.SmoothBackToSelect());
    }

    private IEnumerator RunWave(int count, int waveNumber, System.Action<int> startFunc)
    {
        spawner.StopAllSpawning();
        spawner.SetWaveNumber(waveNumber);

        speedScaler?.NotifyWaveStart(waveNumber);

        bool spawnDone = false;
        spawner.OnWaveSpawnComplete = () => spawnDone = true;

        startFunc(count);

        if (waveNumber == 2)
            SoundManager.Instance?.PlayWaveStartMusic();

        int killed = 0;

        while (!IsGameOver)
        {
            if (spawnDone)
            {
                int alive = spawner.CountAllAlive();
                int theoreticalKills = Mathf.Clamp(count - alive, 0, count);
                int delta = theoreticalKills - killed;

                if (delta > 0)
                {
                    killed += delta;
                    ui.AddKills(delta);
                }

                if (killed >= count)
                    yield break;
            }

            yield return new WaitForSeconds(checkInterval);
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
