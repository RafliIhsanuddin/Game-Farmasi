using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EndlessUIController : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI cycleText;
    [SerializeField] private TextMeshProUGUI totalFlagText;
    [SerializeField] private TextMeshProUGUI totalFlagSummaryText;

    [Header("Progress UI")]
    [SerializeField] private Slider slider;
    [SerializeField] private RectTransform flagContainer;
    [SerializeField] private GameObject flagPrefab;
    [SerializeField] private GameObject bigWaveWarning;

    private int totalEnemiesThisCycle;
    private int totalKillsThisCycle;
    private int totalFlagsAllCycles;
    private int currentCycleVisual = 0;

    // 3 wave => 2 flags: goal wave1, goal wave1+wave2
    private readonly List<int> flagGoals = new();

    // flag order: index 0 = boundary wave1->wave2, index 1 = boundary wave2->wave3
    private readonly List<FlagsManager> flagsOrdered = new();

    public int TotalFlags => totalFlagsAllCycles;
    public int CurrentCycle => currentCycleVisual;

    // =====================================================
    // INIT
    // =====================================================
    private void Awake()
    {
        if (slider != null)
            slider.direction = Slider.Direction.RightToLeft;
    }

    private void Start()
    {
        if (bigWaveWarning)
            bigWaveWarning.SetActive(false);

        GameData.Data.CurrentFlags = 0;

        UpdateTotalFlagsText();
        UpdateSelectText();
    }

    // =====================================================
    // REQUIRED BY EndlessPhaseController
    // =====================================================
    public void EnterSelectPhase()
    {
        UpdateSelectText();
    }

    public void OnPlayPhaseStart()
    {
        // empty for compatibility
    }

    private void UpdateSelectText()
    {
        if (!cycleText) return;

        int next = currentCycleVisual + 1;
        cycleText.text = $"Click Ready to enter Cycle {next}";
    }

    // =====================================================
    // SETUP NEW CYCLE
    // =====================================================
    public void SetupNewCycle(int wave1, int wave2, int wave3)
    {
        currentCycleVisual++;

        if (cycleText)
            cycleText.text = $"Cycle {currentCycleVisual}";

        totalEnemiesThisCycle = wave1 + wave2 + wave3;
        totalKillsThisCycle = 0;

        if (slider)
        {
            slider.minValue = 0;
            slider.maxValue = totalEnemiesThisCycle;
            slider.value = 0;
            slider.direction = Slider.Direction.RightToLeft;
        }

        GenerateFlagGoals(wave1, wave2);
        BuildFlags();
    }

    // =====================================================
    // FLAG GOALS (3 wave => 2 flag)
    // =====================================================
    private void GenerateFlagGoals(int wave1, int wave2)
    {
        flagGoals.Clear();

        int cumulative = 0;

        cumulative += wave1;
        flagGoals.Add(cumulative);

        cumulative += wave2;
        flagGoals.Add(cumulative);
    }

    // =====================================================
    // BUILD FLAGS (ANCHOR BASED, RIGHT -> LEFT)
    // =====================================================
    private void BuildFlags()
    {
        flagsOrdered.Clear();

        if (!flagContainer || !flagPrefab || totalEnemiesThisCycle <= 0)
            return;

        foreach (Transform child in flagContainer)
            Destroy(child.gameObject);

        Canvas.ForceUpdateCanvases();

        foreach (int goal in flagGoals)
        {
            GameObject flagObj = Instantiate(flagPrefab, flagContainer);

            RectTransform rt = flagObj.GetComponent<RectTransform>();
            FlagsManager fm = flagObj.GetComponent<FlagsManager>();

            rt.localScale = new Vector3(0.4f, 0.4f, 1f);
            rt.sizeDelta = new Vector2(30f, 60f);

            float normalized = (float)goal / Mathf.Max(1, totalEnemiesThisCycle);

            // RightToLeft => anchor = 1 - normalized
            float anchorX = 1f - normalized;

            rt.anchorMin = new Vector2(anchorX, 0.5f);
            rt.anchorMax = new Vector2(anchorX, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;

            flagsOrdered.Add(fm);
        }
    }

    // =====================================================
    // ADD KILLS (FROM EndlessWaveManager: killedNow)
    // =====================================================
    public void AddKills(int amount)
    {
        if (totalEnemiesThisCycle <= 0 || slider == null)
            return;

        totalKillsThisCycle += amount;

        // clamp agar tidak overshoot max cycle
        if (totalKillsThisCycle > totalEnemiesThisCycle)
            totalKillsThisCycle = totalEnemiesThisCycle;

        if (totalKillsThisCycle < 0)
            totalKillsThisCycle = 0;

        slider.value = totalKillsThisCycle;
    }

    // =====================================================
    // SNAP TO WAVE BOUNDARY (PRESISI DI TITIK BENDERA)
    // dipanggil tepat saat wave baru mulai
    // waveIndex: 0 => snap ke goal wave1
    // waveIndex: 1 => snap ke goal wave1+wave2
    // =====================================================
    public void SnapToWaveBoundary(int waveIndex)
    {
        if (slider == null) return;
        if (waveIndex < 0 || waveIndex >= flagGoals.Count) return;

        int target = flagGoals[waveIndex];

        // PRESISI: set tepat ke target, bukan >= atau <=
        totalKillsThisCycle = Mathf.Clamp(target, 0, totalEnemiesThisCycle);
        slider.value = totalKillsThisCycle;
    }

    // =====================================================
    // WAVE COMPLETE (FLAG MERAH SAAT WAVE START MUSIC)
    // waveIndex: 0 untuk flag pertama, 1 untuk flag kedua
    // =====================================================
    public void MarkWaveComplete(int waveIndex)
    {
        if (waveIndex >= 0 && waveIndex < flagsOrdered.Count)
        {
            if (flagsOrdered[waveIndex] != null)
                flagsOrdered[waveIndex].Expand();
        }

        totalFlagsAllCycles++;
        GameData.Data.CurrentFlags = totalFlagsAllCycles;

        UpdateTotalFlagsText();
    }

    // =====================================================
    // BIG WAVE
    // =====================================================
    public void ShowBigWave(bool state)
    {
        if (bigWaveWarning)
            bigWaveWarning.SetActive(state);
    }

    // =====================================================
    // UI TEXT
    // =====================================================
    private void UpdateTotalFlagsText()
    {
        if (totalFlagText)
            totalFlagText.text = $"Flags: {totalFlagsAllCycles}";

        if (totalFlagSummaryText)
            totalFlagSummaryText.text = $"Total flag captured : {totalFlagsAllCycles}";
    }

    // =====================================================
    // RESET
    // =====================================================
    public void ResetUIForGameOver()
    {
        currentCycleVisual = 0;
        totalFlagsAllCycles = 0;
        totalKillsThisCycle = 0;
        totalEnemiesThisCycle = 0;

        GameData.Data.CurrentFlags = 0;

        if (slider)
            slider.value = 0;

        UpdateTotalFlagsText();
        UpdateSelectText();
    }
}