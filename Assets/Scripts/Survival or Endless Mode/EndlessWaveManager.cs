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

    // =====================================================================
    // RUN SINGLE CYCLE
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
        ui.SetupNewCycle(w1, w2);

        spawner.SetCycle(cycleNum);

        if (difficulty.CurrentCycle == 0)
        {
            // CYCLE 1 = Single Wave 1 & Single Wave 2
            yield return WaitAllEnemiesDead();
            yield return RunSingleQueuePVZ(w1, 1);
            ui.MarkWaveComplete(0);

            yield return WaitAllEnemiesDead();

            Debug.Log($"<color=#FFA500>[DEBUG] >>> BIG WAVE WARNING</color>");
            ui.ShowBigWave(true);

            SoundManager.Instance?.PlayBigWaveWarning();

            yield return new WaitForSeconds(bigWaveDelay);
            ui.ShowBigWave(false);

            yield return RunSingleQueuePVZ(w2, 2);
            ui.MarkWaveComplete(1);

            yield return WaitAllEnemiesDead();
        }
        else
        {
            // CYCLE 2+
            yield return WaitAllEnemiesDead();
            yield return RunMulti(w1, 1);
            ui.MarkWaveComplete(0);

            Debug.Log($"<color=#FFA500>[DEBUG] >>> BIG WAVE WARNING</color>");
            ui.ShowBigWave(true);

            SoundManager.Instance?.PlayBigWaveWarning();

            yield return new WaitForSeconds(bigWaveDelay);
            ui.ShowBigWave(false);

            yield return RunGrouped(w2, 2);
            ui.MarkWaveComplete(1);

            yield return WaitAllEnemiesDead();
        }

        difficulty.AdvanceCycle();
        phase.StartCoroutine(phase.SmoothBackToSelect());
    }

    // =====================================================================
    // RUNNERS
    // =====================================================================
    private IEnumerator RunSingleQueuePVZ(int count, int waveNumber)
    {
        int cycleNum = difficulty.CurrentCycle + 1;
        Debug.Log($"<color=white>[DEBUG] >>> ENTER RunSingleQueuePVZ(cycle={cycleNum}, wave={waveNumber}, count={count})</color>");

        if (!ValidateSpawnerForWave(cycleNum, waveNumber, count))
            yield break;

        spawner.StopAllSpawning();
        spawner.SetWaveNumber(waveNumber);

        if (speedScaler != null)
            speedScaler.NotifyWaveStart(waveNumber);

        bool spawnDone = false;
        spawner.OnWaveSpawnComplete = () => { spawnDone = true; };

        spawner.StartSinglePerLineWave(count);

        // === FINAL RULE SFX ===
        if (waveNumber == 2)
            SoundManager.Instance?.PlayWaveStartMusic();

        yield return TrackKillsUntilComplete(cycleNum, waveNumber, count, () => spawnDone);
    }

    private IEnumerator RunMulti(int count, int waveNumber)
    {
        int cycleNum = difficulty.CurrentCycle + 1;
        Debug.Log($"<color=white>[DEBUG] >>> ENTER RunMulti(cycle={cycleNum}, wave={waveNumber}, count={count})</color>");

        if (!ValidateSpawnerForWave(cycleNum, waveNumber, count))
            yield break;

        spawner.StopAllSpawning();
        spawner.SetWaveNumber(waveNumber);

        if (speedScaler != null)
            speedScaler.NotifyWaveStart(waveNumber);

        bool spawnDone = false;
        spawner.OnWaveSpawnComplete = () => { spawnDone = true; };

        spawner.StartMultiPerLineWave(count);

        // === FINAL RULE SFX ===
        if (waveNumber == 2)
            SoundManager.Instance?.PlayWaveStartMusic();

        yield return TrackKillsUntilComplete(cycleNum, waveNumber, count, () => spawnDone);
    }

    private IEnumerator RunGrouped(int count, int waveNumber)
    {
        int cycleNum = difficulty.CurrentCycle + 1;
        Debug.Log($"<color=white>[DEBUG] >>> ENTER RunGrouped(cycle={cycleNum}, wave={waveNumber}, count={count})</color>");

        if (!ValidateSpawnerForWave(cycleNum, waveNumber, count))
            yield break;

        spawner.StopAllSpawning();
        spawner.SetWaveNumber(waveNumber);

        if (speedScaler != null)
            speedScaler.NotifyWaveStart(waveNumber);

        bool spawnDone = false;
        spawner.OnWaveSpawnComplete = () => { spawnDone = true; };

        spawner.StartGroupedPerLineWave(count);

        // === FINAL RULE SFX ===
        if (waveNumber == 2)
            SoundManager.Instance?.PlayWaveStartMusic();

        yield return TrackKillsUntilComplete(cycleNum, waveNumber, count, () => spawnDone);
    }

    // =====================================================================
    // VALIDATION
    // =====================================================================
    private bool ValidateSpawnerForWave(int cycleNum, int waveNumber, int count)
    {
        if (spawner == null) return false;
        if (count <= 0) return false;
        if (spawner.bacteriaPrefabs == null || spawner.bacteriaPrefabs.Count == 0) return false;
        if (spawner.spawnPoints == null || spawner.spawnPoints.Count == 0) return false;
        return true;
    }

    // =====================================================================
    // TRACK KILLS
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
                    killed += deltaKills;
                    if (ui != null)
                        ui.AddKills(deltaKills);

                    if (killed >= count)
                        yield break;
                }
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    // =====================================================================
    // WAIT CLEANUP
    // =====================================================================
    private IEnumerator WaitAllEnemiesDead()
    {
        if (spawner == null)
            yield break;

        while (!IsGameOver && spawner.CountAllAlive() > 0)
            yield return new WaitForSeconds(checkInterval);
    }

    // =====================================================================
    // GAME OVER
    // =====================================================================
    public void TriggerGameOver()
    {
        if (IsGameOver)
            return;

        IsGameOver = true;

        Debug.Log("<color=red>[EndlessWave] GAME OVER TRIGGERED</color>");

        if (spawner != null)
            spawner.StopAllSpawning();

        if (speedScaler != null)
            speedScaler.ResetSpeed();

        StopAllCoroutines();

        if (ui != null)
        {
            GameData.Data.UpdateEndlessRun(ui.CurrentCycle, ui.TotalFlags);
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
