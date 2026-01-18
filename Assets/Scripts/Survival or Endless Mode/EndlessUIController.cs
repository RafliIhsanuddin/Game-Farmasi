using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EndlessUIController : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI cycleText;
    [SerializeField] private TextMeshProUGUI totalFlagText;

    [Header("Progress UI per Cycle")]
    [SerializeField] private Slider slider;
    [SerializeField] private RectTransform flagContainer;
    [SerializeField] private GameObject flagPrefab;
    [SerializeField] private GameObject bigWaveWarning;

    private int totalEnemiesThisCycle;
    private int totalKillsThisCycle;
    private int totalFlagsAllCycles;
    private int currentCycleVisual = 0;

    private readonly List<FlagsManager> cycleFlags = new();

    public int TotalFlags => totalFlagsAllCycles;
    public int CurrentCycle => currentCycleVisual;

    private void Start()
    {
        if (bigWaveWarning) bigWaveWarning.SetActive(false);
        currentCycleVisual = 0;
        totalFlagsAllCycles = 0;
        UpdateSelectText();
        UpdateTotalFlagsText();
    }

    // ==== SELECT UI ====
    public void EnterSelectPhase()
    {
        UpdateSelectText();
    }

    private void UpdateSelectText()
    {
        if (!cycleText) return;
        int next = currentCycleVisual + 1;
        cycleText.text = $"Click Ready to enter Cycle {next}";
    }

    // ==== PLAY UI ====
    public void OnPlayPhaseStart()
    {
        currentCycleVisual++;
        UpdatePlayText();
    }

    private void UpdatePlayText()
    {
        if (!cycleText) return;
        cycleText.text = $"Cycle {currentCycleVisual}";
    }

    // ==== New Cycle Setup ====
    public void SetupNewCycle(int wave1, int wave2)
    {
        totalEnemiesThisCycle = wave1 + wave2;
        totalKillsThisCycle = 0;

        if (slider)
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
        if (!flagContainer || !flagPrefab || totalEnemiesThisCycle <= 0) return;

        foreach (Transform c in flagContainer)
            Destroy(c.gameObject);

        float[] anchors = { 0.25f, 0.75f }; // 2 wave = 2 flags

        foreach (var t in anchors)
        {
            var obj = Instantiate(flagPrefab, flagContainer);
            var rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(t, 0.5f);
            rt.anchoredPosition = Vector2.zero;

            var fm = obj.GetComponent<FlagsManager>();
            if (fm) cycleFlags.Add(fm);
        }
    }

    public void AddKills(int delta)
    {
        totalKillsThisCycle += delta;
        if (slider != null)
            slider.value = totalKillsThisCycle;
    }

    public void MarkWaveComplete(int waveIndex)
    {
        if (waveIndex < 0 || waveIndex >= cycleFlags.Count) return;

        var fm = cycleFlags[waveIndex];
        if (fm) fm.Expand();

        totalFlagsAllCycles++;
        UpdateTotalFlagsText();
    }

    private void UpdateTotalFlagsText()
    {
        if (totalFlagText)
            totalFlagText.text = $"Flags: {totalFlagsAllCycles}";
    }

    // ==== Big Wave ====
    public void ShowBigWave(bool state)
    {
        if (!bigWaveWarning) return;
        bigWaveWarning.SetActive(state);
    }

    // ==== Reset ====
    public void ResetUIForGameOver()
    {
        currentCycleVisual = 0;
        totalFlagsAllCycles = 0;
        UpdateTotalFlagsText();
        UpdateSelectText();
    }
}
