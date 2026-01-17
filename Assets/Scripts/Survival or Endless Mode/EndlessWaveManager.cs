using UnityEngine;
using System.Collections;

public class EndlessWaveManager : MonoBehaviour
{
    [Header("Core References")]
    [SerializeField] private BacteriaSpawner spawner;
    [SerializeField] private EndlessDifficultyScaler difficulty;
    [SerializeField] private EndlessUIController ui;

    [Header("Timing")]
    [SerializeField] private float bigWaveDelay = 3f;
    [SerializeField] private float checkInterval = 0.25f;

    private int completedCycles = 0;
    private bool hasSavedRun = false;

    /// <summary>
    /// Dipanggil oleh ReadyButton setiap kali player menekan Ready.
    /// Menjalankan 1 cycle: Wave1(Multi) + Wave2(Grouped) lalu selesai.
    /// </summary>
    public IEnumerator RunSingleCycle()
    {
        if (spawner == null || difficulty == null || ui == null)
        {
            Debug.LogWarning("[EndlessWaveManager] Missing references.");
            yield break;
        }

        if (WaveManager.isGameOver)
        {
            SaveEndlessRunIfNeeded();
            yield break;
        }

        int cycleIndex = difficulty.CurrentCycle;
        int wave1Count = difficulty.GetWave1Count();
        int wave2Count = difficulty.GetWave2Count();

        ui.SetupNewCycle(cycleIndex, wave1Count, wave2Count);

        // WAVE 1: Multi Per Line
        yield return RunMulti(wave1Count);
        ui.MarkWaveComplete(0);

        // Big Wave Warning
        ui.ShowBigWave(true);
        yield return new WaitForSeconds(bigWaveDelay);
        ui.ShowBigWave(false);

        // WAVE 2: Grouped Per Line
        yield return RunGrouped(wave2Count);
        ui.MarkWaveComplete(1);

        // Satu cycle (2 wave) selesai
        completedCycles++;
        difficulty.AdvanceCycle();

        Debug.Log($"[EndlessWaveManager] Cycle {cycleIndex} selesai. CompletedCycles={completedCycles}");
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

        while (true)
        {
            if (WaveManager.isGameOver)
            {
                spawner.StopAllSpawning();
                SaveEndlessRunIfNeeded();
                yield break;
            }

            int alive = spawner != null ? spawner.CountAllAlive() : 0;

            if (alive > 0) seenEnemy = true;
            if (seenEnemy && alive == 0)
                break;

            if (alive < lastAlive && ui != null)
            {
                int delta = lastAlive - alive;
                ui.AddKills(delta);
            }

            lastAlive = alive;

            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void SaveEndlessRunIfNeeded()
    {
        if (hasSavedRun) return;

        int finalCycle = completedCycles;
        int finalFlags = ui != null ? ui.TotalFlags : finalCycle * 2;

        GameData.Data.UpdateEndlessRun(finalCycle, finalFlags);
        hasSavedRun = true;

        Debug.Log($"[EndlessWaveManager] SaveEndlessRun: cycle={finalCycle}, flags={finalFlags}");
    }
}
