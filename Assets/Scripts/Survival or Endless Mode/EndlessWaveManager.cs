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

    /// <summary>
    /// Jalankan 1 cycle:
    /// - Cycle 1: Wave1 & Wave2 pakai SinglePerLine (queue by enemy)
    /// - Cycle 2+: Wave1 = MultiPerLine, Wave2 = GroupedPerLine
    /// Patokan wave selesai = jumlah kill >= count dari EndlessDifficultyScaler.
    /// </summary>
    public IEnumerator RunSingleCycle()
    {
        if (spawner == null || difficulty == null || ui == null || phase == null)
        {
            Debug.LogError("[EndlessWave] Missing references (spawner/difficulty/ui/phase)");
            yield break;
        }

        int cycleNum = difficulty.CurrentCycle + 1;

        int w1 = difficulty.GetWave1Count();
        int w2 = difficulty.GetWave2Count();

        Debug.Log($"[EndlessWave] Start Cycle {cycleNum} → Wave1={w1}, Wave2={w2}");
        ui.SetupNewCycle(w1, w2);

        // ==========================
        // CYCLE 1: SINGLE-PER-LINE
        // ==========================
        if (difficulty.CurrentCycle == 0)
        {
            Debug.Log($"[EndlessWave] CYCLE {cycleNum} → Mode: SinglePerLine for both waves");

            // Pastikan papan bersih sebelum cycle ini
            yield return WaitAllEnemiesDead();

            // ----- WAVE 1 -----
            yield return RunSingleQueuePVZ(w1, 1);

            ui.MarkWaveComplete(0);
            Debug.Log($"Cycle {cycleNum} Wave 1 — All enemies dead, moving to Wave 2...");

            // Safety sebelum BigWave
            yield return WaitAllEnemiesDead();

            // ----- BIG WAVE WARNING -----
            Debug.Log($"Cycle {cycleNum} — BIG WAVE WARNING (Wave 2 incoming)");
            ui.ShowBigWave(true);
            yield return new WaitForSeconds(bigWaveDelay);
            ui.ShowBigWave(false);

            // ----- WAVE 2 -----
            yield return RunSingleQueuePVZ(w2, 2);
            ui.MarkWaveComplete(1);

            Debug.Log($"Cycle {cycleNum} — Wave 2 complete, cycle finished.");
            yield return WaitAllEnemiesDead();
        }
        else
        {
            // ==========================
            // CYCLE 2+: Multi + Grouped
            // ==========================
            Debug.Log($"[EndlessWave] CYCLE {cycleNum} → Wave1=MultiPerLine, Wave2=GroupedPerLine");

            // Safety: bersihkan enemy sisa sebelum wave baru
            yield return WaitAllEnemiesDead();

            // Wave 1 (Multi)
            yield return RunMulti(w1, 1);
            ui.MarkWaveComplete(0);
            Debug.Log($"Cycle {cycleNum} Wave 1 — All enemies dead, moving to Wave 2 (Grouped)...");

            // Big Wave warning
            Debug.Log($"Cycle {cycleNum} — BIG WAVE WARNING (Wave 2 incoming)");
            ui.ShowBigWave(true);
            yield return new WaitForSeconds(bigWaveDelay);
            ui.ShowBigWave(false);

            // Wave 2 (Grouped)
            yield return RunGrouped(w2, 2);
            ui.MarkWaveComplete(1);

            Debug.Log($"Cycle {cycleNum} — Both waves complete, cycle finished.");
            yield return WaitAllEnemiesDead();
        }

        difficulty.AdvanceCycle();
        phase.StartCoroutine(phase.SmoothBackToSelect());
    }

    // =========================================================
    //  CYCLE 1: SinglePerLine PVZ Queue (lane-based)
    //  Classic-A: wave selesai jika kill >= count
    // =========================================================
    private IEnumerator RunSingleQueuePVZ(int count, int waveNumber)
    {
        int cycleNum = difficulty.CurrentCycle + 1;
        Debug.Log($"[EndlessWave] Prepare Cycle {cycleNum} Wave {waveNumber} — SinglePerLine, target={count}");

        if (spawner == null)
        {
            Debug.LogError($"Cycle {cycleNum} Wave {waveNumber} — spawner failed enemy not spawn (spawner NULL)");
            yield break;
        }

        if (count <= 0)
        {
            Debug.LogWarning($"Cycle {cycleNum} Wave {waveNumber} — spawner failed enemy not spawn (count <= 0)");
            yield break;
        }

        if (spawner.bacteriaPrefabs == null || spawner.bacteriaPrefabs.Count == 0)
        {
            Debug.LogWarning($"Cycle {cycleNum} Wave {waveNumber} — spawner failed enemy not spawn (empty enemy pool)");
            yield break;
        }

        if (spawner.spawnPoints == null || spawner.spawnPoints.Count == 0)
        {
            Debug.LogWarning($"Cycle {cycleNum} Wave {waveNumber} — spawner failed enemy not spawn (no spawn points)");
            yield break;
        }

        spawner.StopAllSpawning();

        bool spawnDone = false;
        spawner.OnWaveSpawnComplete = () =>
        {
            spawnDone = true;
            Debug.Log($"[EndlessWave] Cycle {cycleNum} Wave {waveNumber} — Spawner reported SPAWN COMPLETE");
        };

        Debug.Log($"Cycle {cycleNum} Wave {waveNumber} — START (SinglePerLine)");
        Debug.Log($"Cycle {cycleNum} Wave {waveNumber} — Total enemies: {count}");
        Debug.Log("spawner activate enemy spawn mode single per line");

        // Mulai wave SinglePerLine
        spawner.StartSinglePerLineWave(count);

        int killed = 0;

        // Monitor sampai jumlah kill >= count
        while (!IsGameOver)
        {
            // Bisa kamu aktifkan kalau mau lihat queue lane
            DebugQueueState();

            if (spawnDone)
            {
                int alive = spawner.CountAllAlive();

                int theoreticalKills = Mathf.Clamp(count - alive, 0, count);
                int deltaKills = theoreticalKills - killed;

                if (deltaKills > 0)
                {
                    // Print per kill (1, 2, 3, dst) + remaining
                    for (int i = 0; i < deltaKills; i++)
                    {
                        int currentKillIndex = killed + i + 1;
                        int remaining = Mathf.Max(count - currentKillIndex, 0);
                        Debug.Log($"Cycle {cycleNum} Wave {waveNumber} — Kill: {currentKillIndex} ({remaining} remaining)");
                    }

                    killed += deltaKills;
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

    /// <summary>
    /// Debug status tiap SpawnPointSlot:
    /// - OCCUPIED → wait enemy
    /// - FREE → candidate spawn
    /// Hanya untuk bantu cek queue behaviour (opsional).
    /// </summary>
    private void DebugQueueState()
    {
        if (spawner == null || spawner.spawnPoints == null)
            return;

        // Kalau mau lihat detail lane, un-comment log di bawah.
        /*
        List<int> free = new List<int>();

        for (int i = 0; i < spawner.spawnPoints.Count; i++)
        {
            var slot = spawner.spawnPoints[i];
            if (slot == null) continue;

            if (slot.occupied)
                Debug.Log($"[Queue] {slot.name} = OCCUPIED → wait enemy");
            else
            {
                Debug.Log($"[Queue] {slot.name} = FREE → candidate");
                free.Add(i);
            }
        }

        if (free.Count == 0)
            Debug.Log("[Queue] ALL lanes occupied → WAIT until some enemy dies");
        else if (free.Count == 1)
            Debug.Log($"[Queue] 1 lane free → effectively spam lane index={free[0]}");
        else
            Debug.Log($"[Queue] {free.Count} lanes free → random among free lanes");
        */
    }

    // =========================================================
    //  CYCLE 2+: Multi & Grouped (Classic-A: kill >= count)
    // =========================================================
    private IEnumerator RunMulti(int count, int waveNumber)
    {
        int cycleNum = difficulty.CurrentCycle + 1;
        Debug.Log($"[EndlessWave] Prepare Cycle {cycleNum} Wave {waveNumber} — MultiPerLine, target={count}");

        if (spawner == null)
        {
            Debug.LogError($"Cycle {cycleNum} Wave {waveNumber} — spawner failed enemy not spawn (spawner NULL)");
            yield break;
        }

        if (count <= 0)
        {
            Debug.LogWarning($"Cycle {cycleNum} Wave {waveNumber} — spawner failed enemy not spawn (count <= 0)");
            yield break;
        }

        if (spawner.bacteriaPrefabs == null || spawner.bacteriaPrefabs.Count == 0)
        {
            Debug.LogWarning($"Cycle {cycleNum} Wave {waveNumber} — spawner failed enemy not spawn (empty enemy pool)");
            yield break;
        }

        if (spawner.spawnPoints == null || spawner.spawnPoints.Count == 0)
        {
            Debug.LogWarning($"Cycle {cycleNum} Wave {waveNumber} — spawner failed enemy not spawn (no spawn points)");
            yield break;
        }

        spawner.StopAllSpawning();

        bool spawnDone = false;
        spawner.OnWaveSpawnComplete = () =>
        {
            spawnDone = true;
            Debug.Log($"[EndlessWave] Cycle {cycleNum} Wave {waveNumber} — Spawner reported SPAWN COMPLETE");
        };

        Debug.Log($"Cycle {cycleNum} Wave {waveNumber} — START (MultiPerLine)");
        Debug.Log($"Cycle {cycleNum} Wave {waveNumber} — Total enemies: {count}");
        Debug.Log("spawner activate enemy spawn mode multi per line");

        spawner.StartMultiPerLineWave(count);

        int killed = 0;

        while (!IsGameOver)
        {
            if (spawnDone)
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

    private IEnumerator RunGrouped(int count, int waveNumber)
    {
        int cycleNum = difficulty.CurrentCycle + 1;
        Debug.Log($"[EndlessWave] Prepare Cycle {cycleNum} Wave {waveNumber} — GroupedPerLine, target={count}");

        if (spawner == null)
        {
            Debug.LogError($"Cycle {cycleNum} Wave {waveNumber} — spawner failed enemy not spawn (spawner NULL)");
            yield break;
        }

        if (count <= 0)
        {
            Debug.LogWarning($"Cycle {cycleNum} Wave {waveNumber} — spawner failed enemy not spawn (count <= 0)");
            yield break;
        }

        if (spawner.bacteriaPrefabs == null || spawner.bacteriaPrefabs.Count == 0)
        {
            Debug.LogWarning($"Cycle {cycleNum} Wave {waveNumber} — spawner failed enemy not spawn (empty enemy pool)");
            yield break;
        }

        if (spawner.spawnPoints == null || spawner.spawnPoints.Count == 0)
        {
            Debug.LogWarning($"Cycle {cycleNum} Wave {waveNumber} — spawner failed enemy not spawn (no spawn points)");
            yield break;
        }

        spawner.StopAllSpawning();

        bool spawnDone = false;
        spawner.OnWaveSpawnComplete = () =>
        {
            spawnDone = true;
            Debug.Log($"[EndlessWave] Cycle {cycleNum} Wave {waveNumber} — Spawner reported SPAWN COMPLETE");
        };

        Debug.Log($"Cycle {cycleNum} Wave {waveNumber} — START (GroupedPerLine)");
        Debug.Log($"Cycle {cycleNum} Wave {waveNumber} — Total enemies: {count}");
        Debug.Log("spawner activate enemy spawn mode grouped per line");

        spawner.StartGroupedPerLineWave(count);

        int killed = 0;

        while (!IsGameOver)
        {
            if (spawnDone)
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

    // =========================================================
    //  WAIT until no enemies alive on board (cleanup only)
    // =========================================================
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

    // =========================================================
    //  GAME OVER SUPPORT
    // =========================================================
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
