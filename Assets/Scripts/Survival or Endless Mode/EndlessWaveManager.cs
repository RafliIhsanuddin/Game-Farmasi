using UnityEngine;
using System.Collections;

public class EndlessWaveManager : MonoBehaviour
{
    [SerializeField] private BacteriaSpawner spawner;
    [SerializeField] private EndlessDifficultyScaler difficulty;
    [SerializeField] private EndlessUIController ui;
    [SerializeField] private EndlessPhaseController phase;

    [SerializeField] private float bigWaveDelay = 3f;
    [SerializeField] private float checkInterval = 0.25f;

    public bool IsGameOver { get; private set; } = false;

    public IEnumerator RunSingleCycle()
    {
        int w1 = difficulty.GetWave1Count();
        int w2 = difficulty.GetWave2Count();

        ui.SetupNewCycle(w1, w2);

        if (difficulty.CurrentCycle == 0)
        {
            yield return RunSingle(w1);
            ui.MarkWaveComplete(0);

            ui.ShowBigWave(true);
            yield return new WaitForSeconds(bigWaveDelay);
            ui.ShowBigWave(false);

            yield return RunSingle(w2);
            ui.MarkWaveComplete(1);
        }
        else
        {
            yield return RunMulti(w1);
            ui.MarkWaveComplete(0);

            ui.ShowBigWave(true);
            yield return new WaitForSeconds(bigWaveDelay);
            ui.ShowBigWave(false);

            yield return RunGrouped(w2);
            ui.MarkWaveComplete(1);
        }

        difficulty.AdvanceCycle();
        phase.StartCoroutine(phase.SmoothBackToSelect());
    }

    private IEnumerator RunSingle(int count)
    {
        spawner.StopAllSpawning();
        spawner.StartSinglePerLineWave(count);
        yield return WaitAllEnemiesDead();
    }

    private IEnumerator RunMulti(int count)
    {
        spawner.StopAllSpawning();
        spawner.StartMultiPerLineWave(count);
        yield return WaitAllEnemiesDead();
    }

    private IEnumerator RunGrouped(int count)
    {
        spawner.StopAllSpawning();
        spawner.StartGroupedPerLineWave(count);
        yield return WaitAllEnemiesDead();
    }

    private IEnumerator WaitAllEnemiesDead()
    {
        int lastAlive = 0;
        bool seenEnemy = false;

        while (!IsGameOver)
        {
            int alive = spawner.CountAllAlive();

            if (alive > 0) seenEnemy = true;
            if (seenEnemy && alive == 0) break;

            if (alive < lastAlive)
            {
                int delta = lastAlive - alive;
                ui.AddKills(delta);
            }

            lastAlive = alive;
            yield return new WaitForSeconds(checkInterval);
        }
    }
}
