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
        if (bigWaveWarning)
            bigWaveWarning.SetActive(false);

        currentCycleVisual = 0;
        totalFlagsAllCycles = 0;

        UpdateTotalFlagsText();
        UpdateSelectText(); // awal → "Click Ready to enter Cycle 1"
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
        string msg = $"Click Ready to enter Cycle {next}";

        Debug.Log($"<color=cyan>[UI] SELECT → text = \"{msg}\" (currentCycleVisual={currentCycleVisual})</color>");

        cycleText.text = msg;
    }

    // ==== PLAY UI ====
    // Hanya untuk log status, TIDAK mengubah text (text diubah di SetupNewCycle)
    public void OnPlayPhaseStart()
    {
        Debug.Log($"<color=grey>[UI] Enter PLAY phase (currentCycleVisual={currentCycleVisual})</color>");
    }

    // ==== New Cycle Setup (dipanggil dari EndlessWaveManager.RunSingleCycle) ====
    public void SetupNewCycle(int wave1, int wave2)
    {
        currentCycleVisual++;

        // >>> DI SINI text berubah jadi "Cycle 1", "Cycle 2", dst <<<
        string msg = $"Cycle {currentCycleVisual}";
        if (cycleText != null)
        {
            cycleText.text = msg;
            Debug.Log($"<color=yellow>[UI] PLAY → text = \"{msg}\" (SetupNewCycle)</color>");
        }

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
