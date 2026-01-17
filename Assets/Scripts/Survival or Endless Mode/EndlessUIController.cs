using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EndlessUIController : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI cycleText;
    [SerializeField] private TextMeshProUGUI totalFlagText;

    [Header("Progress UI (per Cycle)")]
    [SerializeField] private Slider slider;
    [SerializeField] private RectTransform flagContainer;
    [SerializeField] private GameObject flagPrefab;
    [SerializeField] private GameObject bigWaveWarning;

    private int totalEnemiesThisCycle;
    private int totalKillsThisCycle;
    private int totalFlagsAllCycles;
    private int currentCycleVisual = 0;   // 0 = belum pernah main, nanti naik di OnPlayPhaseStart

    private readonly List<FlagsManager> cycleFlags = new List<FlagsManager>();

    public int TotalFlags => totalFlagsAllCycles;
    public int CurrentCycle => currentCycleVisual;

    private void Start()
    {
        if (bigWaveWarning)
            bigWaveWarning.SetActive(false);

        totalFlagsAllCycles = 0;
        currentCycleVisual = 0;

        UpdateSelectText();
        UpdateTotalFlagsText();
    }

    // ============================
    // SELECT PHASE
    // ============================
    public void EnterSelectPhase()
    {
        UpdateSelectText();
    }

    private void UpdateSelectText()
    {
        if (!cycleText) return;

        int nextCycle = currentCycleVisual + 1;
        cycleText.text = $"Click Ready to enter Cycle {nextCycle}";
        Debug.Log($"[EndlessUI] TEXT → Select Phase: Click Ready to enter Cycle {nextCycle}");
    }

    // ============================
    // PLAY PHASE START
    // ============================
    public void OnPlayPhaseStart()
    {
        currentCycleVisual++;           // 0 → 1 → 2 → 3 ...
        UpdatePlayText();
    }

    private void UpdatePlayText()
    {
        if (!cycleText) return;

        cycleText.text = $"Cycle {currentCycleVisual}";
        Debug.Log($"[EndlessUI] TEXT → Play Phase: Cycle {currentCycleVisual}");
    }

    // ============================
    // SETUP CYCLE (WAVE INFO)
    // ============================
    public void SetupNewCycle(int cycleIndex, int wave1, int wave2)
    {
        totalEnemiesThisCycle = wave1 + wave2;
        totalKillsThisCycle = 0;

        if (slider != null)
        {
            slider.minValue = 0;
            slider.maxValue = totalEnemiesThisCycle;
            slider.value = 0;
        }

        RebuildFlags();
    }

    private void RebuildFlags()
    {
        cycleFlags.Clear();

        if (!flagContainer || !flagPrefab || totalEnemiesThisCycle <= 0)
            return;

        foreach (Transform c in flagContainer)
            Destroy(c.gameObject);

        // Karena konsep: 1 cycle = 2 wave → 2 bendera
        float t1 = 0.5f;  // tengah bar untuk wave 1
        float t2 = 1.0f;  // ujung bar untuk wave 2

        float[] anchors = { t1, t2 };

        for (int i = 0; i < anchors.Length; i++)
        {
            GameObject f = Instantiate(flagPrefab, flagContainer);
            RectTransform rt = f.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(anchors[i], 0.5f);
            rt.anchoredPosition = Vector2.zero;

            var fm = f.GetComponent<FlagsManager>();
            if (fm != null)
                cycleFlags.Add(fm);
        }
    }

    public void AddKills(int delta)
    {
        if (delta <= 0) return;

        totalKillsThisCycle += delta;
        if (slider != null)
            slider.value = totalKillsThisCycle;
    }

    /// <summary>
    /// waveIndex: 0 = wave1, 1 = wave2
    /// </summary>
    public void MarkWaveComplete(int waveIndex)
    {
        if (waveIndex < 0 || waveIndex >= cycleFlags.Count)
            return;

        var fm = cycleFlags[waveIndex];
        if (fm != null)
            fm.Expand();

        totalFlagsAllCycles++;
        UpdateTotalFlagsText();

        Debug.Log($"[EndlessUI] Flag wave {waveIndex + 1} complete. TotalFlags={totalFlagsAllCycles}");
    }

    private void UpdateTotalFlagsText()
    {
        if (totalFlagText != null)
            totalFlagText.text = $"Flags: {totalFlagsAllCycles}";
    }

    public void ShowBigWave(bool show)
    {
        if (bigWaveWarning)
            bigWaveWarning.SetActive(show);
    }

    public void ResetUIForGameOver()
    {
        currentCycleVisual = 0;
        totalFlagsAllCycles = 0;

        UpdateSelectText();
        UpdateTotalFlagsText();

        if (slider != null)
            slider.value = 0;

        if (flagContainer != null)
        {
            foreach (Transform c in flagContainer)
                Destroy(c.gameObject);
        }

        if (bigWaveWarning)
            bigWaveWarning.SetActive(false);

        Debug.Log("[EndlessUI] Reset UI for Game Over");
    }
}
