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

    public bool IsGameOver { get; private set; } = false;

    /// <summary>
    /// Jalankan 1 cycle: Wave1 (MultiPerLine) + Wave2 (GroupedPerLine)
    /// </summary>
    public IEnumerator RunSingleCycle()
    {
        if (IsGameOver)
            yield break;

        if (difficulty == null || ui == null || spawner == null)
        {
            Debug.LogError("[EndlessWave] Missing references!");
            yield break;
        }

        int wave1 = difficulty.GetWave1Count();
        int wave2 = difficulty.GetWave2Count();

        int cycleIndex = ui.CurrentCycle;  // UI sudah dinaikkan di ReadyButton
        ui.SetupNewCycle(cycleIndex, wave1, wave2);

        Debug.Log($"<color=yellow>[EndlessWave] RUN CYCLE {cycleIndex} | W1={wave1}, W2={wave2}</color>");

        // ========== WAVE 1 ==========
        Debug.Log($"<color=cyan>[EndlessWave] Wave1 Start</color>");
        yield return RunMulti(wave1);
        if (IsGameOver) yield break;

        ui.MarkWaveComplete(0);
        Debug.Log($"<color=green>[EndlessWave] Wave1 DONE</color>");

        // ========== BIG WAVE WARNING ==========
        ui.ShowBigWave(true);
        Debug.Log("<color=orange>[EndlessWave] BIG WAVE WARNING</color>");

        yield return new WaitForSeconds(bigWaveDelay);

        ui.ShowBigWave(false);

        // ========== WAVE 2 ==========
        Debug.Log($"<color=cyan>[EndlessWave] Wave2 Start</color>");
        yield return RunGrouped(wave2);
        if (IsGameOver) yield break;

        ui.MarkWaveComplete(1);
        Debug.Log($"<color=green>[EndlessWave] Wave2 DONE → Cycle FINISHED</color>");

        // Setelah 2 wave → difficulty naik
        difficulty.AdvanceCycle();
    }

    private IEnumerator RunMulti(int count)
    {
        spawner.StopAllSpawning();
        spawner.StartMultiPerLineWave(count);
        Debug.Log($"[EndlessWave] Spawn W1 count={count}");
        yield return WaitAllEnemiesDead();
    }

    private IEnumerator RunGrouped(int count)
    {
        spawner.StopAllSpawning();
        spawner.StartGroupedPerLineWave(count);
        Debug.Log($"[EndlessWave] Spawn W2 count={count}");
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

    // ===============================
    // GAME OVER SYSTEM ENDLESS
    // ===============================
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
            Debug.Log($"<color=magenta>[EndlessWave] Final Run → Cycle={ui.CurrentCycle}, Flags={ui.TotalFlags}</color>");

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
