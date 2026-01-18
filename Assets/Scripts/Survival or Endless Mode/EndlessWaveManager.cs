using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndlessWaveManager : MonoBehaviour
{
    [Header("Core References")]
    [SerializeField] private BacteriaSpawner spawner;
    [SerializeField] private EndlessDifficultyScaler difficulty;
    [SerializeField] private EndlessUIController ui;
    [SerializeField] private EndlessPhaseController phase;

    [Header("Timing")]
    [SerializeField] private float bigWaveDelay = 3f;
    [SerializeField] private float checkInterval = 0.25f;

    public bool IsGameOver { get; private set; } = false;

    // =====================================================================
    //  RUN SINGLE CYCLE (PATCH DEBUG FULL)
    // =====================================================================
    public IEnumerator RunSingleCycle()
    {
        if (spawner == null || difficulty == null || ui == null || phase == null)
        {
            Debug.LogError("[EndlessWave] Missing references (spawner/difficulty/ui/phase)");
            yield break;
        }

        Debug.Log("<color=#FF00FF>[DEBUG] >>> ENTER RunSingleCycle()</color>");

        int cycleNum = difficulty.CurrentCycle + 1;

        int w1 = difficulty.GetWave1Count();
        int w2 = difficulty.GetWave2Count();

        Debug.Log($"[EndlessWave] Start Cycle {cycleNum} → Wave 1 Total={w1}, Wave 2 Total={w2}");

        Debug.Log($"[EndlessWave] Spawner state BEFORE cycle {cycleNum}: prefabs={spawner.bacteriaPrefabs?.Count}, spawnPoints={spawner.spawnPoints?.Count}, isSpawning={spawner.IsSpawning}");

        ui.SetupNewCycle(w1, w2);

        if (difficulty.CurrentCycle == 0)
        {
            Debug.Log($"[EndlessWave] Cycle {cycleNum} → Mode: SinglePerLine (Wave 1 & Wave 2)");

            Debug.Log($"<color=yellow>[DEBUG] >>> BEFORE Wave1: WaitAllEnemiesDead()</color>");
            yield return WaitAllEnemiesDead();

            Debug.Log($"<color=lime>[DEBUG] >>> ENTER Wave1 Runner (Single)</color>");
            Debug.Log($"<color=cyan>[DEBUG] >>> CALL RunSingleQueuePVZ(w1,1)</color>");

            yield return RunSingleQueuePVZ(w1, 1);

            ui.MarkWaveComplete(0);

            Debug.Log($"<color=#AAAAAA>Cycle {cycleNum} Wave 1 — FINISHED</color>");

            Debug.Log($"<color=yellow>[DEBUG] >>> BEFORE Wave2: WaitAllEnemiesDead()</color>");
            yield return WaitAllEnemiesDead();

            Debug.Log($"<color=#FFA500>[DEBUG] >>> BIG WAVE WARNING</color>");

            ui.ShowBigWave(true);
            yield return new WaitForSeconds(bigWaveDelay);
            ui.ShowBigWave(false);

            Debug.Log($"<color=lime>[DEBUG] >>> ENTER Wave2 Runner (Single)</color>");
            Debug.Log($"<color=cyan>[DEBUG] >>> CALL RunSingleQueuePVZ(w2,2)</color>");

            yield return RunSingleQueuePVZ(w2, 2);

            ui.MarkWaveComplete(1);

            Debug.Log($"<color=#AAAAAA>Cycle {cycleNum} Wave 2 — FINISHED</color>");

            Debug.Log($"<color=yellow>[DEBUG] >>> AFTER Cycle Cleanup</color>");
            yield return WaitAllEnemiesDead();
        }
        else
        {
            Debug.Log($"[EndlessWave] Cycle {cycleNum} → Mode: MultiPerLine + GroupedPerLine");

            Debug.Log($"<color=yellow>[DEBUG] >>> BEFORE Wave1: WaitAllEnemiesDead()</color>");
            yield return WaitAllEnemiesDead();

            Debug.Log($"<color=lime>[DEBUG] >>> ENTER Wave1 Runner (Multi)</color>");
            Debug.Log($"<color=cyan>[DEBUG] >>> CALL RunMulti(w1,1)</color>");

            yield return RunMulti(w1, 1);
            ui.MarkWaveComplete(0);

            Debug.Log($"<color=#FFA500>[DEBUG] >>> BIG WAVE WARNING</color>");

            ui.ShowBigWave(true);
            yield return new WaitForSeconds(bigWaveDelay);
            ui.ShowBigWave(false);

            Debug.Log($"<color=lime>[DEBUG] >>> ENTER Wave2 Runner (Grouped)</color>");
            Debug.Log($"<color=cyan>[DEBUG] >>> CALL RunGrouped(w2,2)</color>");

            yield return RunGrouped(w2, 2);
            ui.MarkWaveComplete(1);

            Debug.Log($"<color=yellow>[DEBUG] >>> AFTER Cycle Cleanup</color>");
            yield return WaitAllEnemiesDead();
        }

        Debug.Log($"<color=#00FFCC>[DEBUG] >>> AdvanceCycle()</color>");
        difficulty.AdvanceCycle();

        Debug.Log($"<color=#00FFCC>[DEBUG] >>> CALL SmoothBackToSelect()</color>");
        phase.StartCoroutine(phase.SmoothBackToSelect());
    }

    // =====================================================================
    //  RUNNERS (PATCH HEADER DEBUG)
    // =====================================================================
    private IEnumerator RunSingleQueuePVZ(int count, int waveNumber)
    {
        int cycleNum = difficulty.CurrentCycle + 1;
        Debug.Log($"<color=white>[DEBUG] >>> ENTER RunSingleQueuePVZ(cycle={cycleNum}, wave={waveNumber}, count={count})</color>");

        if (!ValidateSpawnerForWave(cycleNum, waveNumber, count))
            yield break;

        Debug.Log("spawner activate enemy spawn mode single per line");

        spawner.StopAllSpawning();

        bool spawnDone = false;
        spawner.OnWaveSpawnComplete = () =>
        {
            spawnDone = true;
            Debug.Log($"[EndlessWave] Cycle {cycleNum} Wave {waveNumber} — Spawner reported SPAWN COMPLETE");
        };

        spawner.StartSinglePerLineWave(count);

        yield return TrackKillsUntilComplete(cycleNum, waveNumber, count, () => spawnDone);
    }

    private IEnumerator RunMulti(int count, int waveNumber)
    {
        int cycleNum = difficulty.CurrentCycle + 1;
        Debug.Log($"<color=white>[DEBUG] >>> ENTER RunMulti(cycle={cycleNum}, wave={waveNumber}, count={count})</color>");

        if (!ValidateSpawnerForWave(cycleNum, waveNumber, count))
            yield break;

        Debug.Log("spawner activate enemy spawn mode multi per line");

        spawner.StopAllSpawning();

        bool spawnDone = false;
        spawner.OnWaveSpawnComplete = () =>
        {
            spawnDone = true;
            Debug.Log($"[EndlessWave] Cycle {cycleNum} Wave {waveNumber} — Spawner reported SPAWN COMPLETE");
        };

        spawner.StartMultiPerLineWave(count);

        yield return TrackKillsUntilComplete(cycleNum, waveNumber, count, () => spawnDone);
    }

    private IEnumerator RunGrouped(int count, int waveNumber)
    {
        int cycleNum = difficulty.CurrentCycle + 1;
        Debug.Log($"<color=white>[DEBUG] >>> ENTER RunGrouped(cycle={cycleNum}, wave={waveNumber}, count={count})</color>");

        if (!ValidateSpawnerForWave(cycleNum, waveNumber, count))
            yield break;

        Debug.Log("spawner activate enemy spawn mode grouped per line");

        spawner.StopAllSpawning();

        bool spawnDone = false;
        spawner.OnWaveSpawnComplete = () =>
        {
            spawnDone = true;
            Debug.Log($"[EndlessWave] Cycle {cycleNum} Wave {waveNumber} — Spawner reported SPAWN COMPLETE");
        };

        spawner.StartGroupedPerLineWave(count);

        yield return TrackKillsUntilComplete(cycleNum, waveNumber, count, () => spawnDone);
    }

    // =====================================================================
    // VALIDATION (TIDAK DIUBAH)
    // =====================================================================
    private bool ValidateSpawnerForWave(int cycleNum, int waveNumber, int count)
    {
        if (spawner == null)
        {
            Debug.LogError($"Cycle {cycleNum} Wave {waveNumber} — spawner failed enemy not spawn (spawner NULL)");
            return false;
        }

        if (count <= 0)
        {
            Debug.LogWarning($"Cycle {cycleNum} Wave {waveNumber} — spawner failed enemy not spawn (amount <= 0)");
            return false;
        }

        if (spawner.bacteriaPrefabs == null || spawner.bacteriaPrefabs.Count == 0)
        {
            Debug.LogWarning($"Cycle {cycleNum} Wave {waveNumber} — spawner failed enemy not spawn (empty enemy pool)");
            return false;
        }

        if (spawner.spawnPoints == null || spawner.spawnPoints.Count == 0)
        {
            Debug.LogWarning($"Cycle {cycleNum} Wave {waveNumber} — spawner failed enemy not spawn (no spawn points)");
            return false;
        }

        return true;
    }

    // =====================================================================
    // TRACK KILLS (TIDAK DIUBAH)
    // =====================================================================
    private IEnumerator TrackKillsUntilComplete(int cycleNum, int waveNumber, int count, System.Func<bool> isSpawnDone)
    {
        int killed = 0;

        while (!IsGameOver)
        {
            if (isSpawnDone())
            {
                int alive = spawner.CountAllAlive();
                int theoreticalKills = Mathf.Clamp(count - alive, 0, count);
                int deltaKills = theoreticalKills - killed;

                if (deltaKills > 0)
                {
                    for (int i = 0; i < deltaKills; i++)
                    {
                        int currentKillIndex = killed + i + 1;
                        int remaining = Mathf.Max(count - currentKillIndex, 0);

                        Debug.Log($"Cycle {cycleNum} Wave {waveNumber} — Kill: {currentKillIndex} ({remaining} remaining)");
                    }

                    killed += deltaKills;

                    if (ui != null)
                        ui.AddKills(deltaKills);
                }

                if (killed >= count)
                {
                    Debug.Log($"Cycle {cycleNum} Wave {waveNumber} — FINISHED (All {count} dead)");
                    yield break;
                }
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    // =====================================================================
    // WAIT CLEANUP (TIDAK DIUBAH)
    // =====================================================================
    private IEnumerator WaitAllEnemiesDead()
    {
        if (spawner == null)
            yield break;

        while (!IsGameOver)
        {
            int alive = spawner.CountAllAlive();
            if (alive <= 0)
                break;

            yield return new WaitForSeconds(checkInterval);
        }
    }

    // =====================================================================
    // GAME OVER (TIDAK DIUBAH)
    // =====================================================================
    public void TriggerGameOver()
    {
        if (IsGameOver)
            return;

        IsGameOver = true;

        Debug.Log("<color=red>[EndlessWave] GAME OVER TRIGGERED</color>");

        if (spawner != null)
            spawner.StopAllSpawning();

        StopAllCoroutines();

        if (ui != null)
        {
            GameData.Data.UpdateEndlessRun(ui.CurrentCycle, ui.TotalFlags);
            Debug.Log($"[EndlessWave] Final Run → Cycle={ui.CurrentCycle}, Flags={ui.TotalFlags}");
            ui.ResetUIForGameOver();
        }
    }

    public void ResetEndlessState()
    {
        IsGameOver = false;
        if (difficulty != null)
            difficulty.ResetDifficulty();
    }
}
