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

    private readonly List<int> flagGoals = new();
    private readonly List<FlagsManager> flagsOrdered = new();

    public int TotalFlags => totalFlagsAllCycles;
    public int CurrentCycle => currentCycleVisual;

    // =====================================================
    // INIT
    // =====================================================

    private void Awake()
    {
        slider.direction = Slider.Direction.RightToLeft;
    }

    private void Start()
    {
        if (bigWaveWarning)
            bigWaveWarning.SetActive(false);

        GameData.Data.CurrentFlags = 0;

        UpdateTotalFlagsText();
        EnterSelectPhase();
    }

    public void EnterSelectPhase()
    {
        if (!cycleText) return;

        int next = currentCycleVisual + 1;
        cycleText.text = $"Click Ready to enter Cycle {next}";
    }

    public void OnPlayPhaseStart()
    {
        // intentionally empty (compatibility with EndlessPhaseController)
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

        slider.minValue = 0;
        slider.maxValue = totalEnemiesThisCycle;
        slider.value = 0;

        GenerateFlagGoals(wave1, wave2);

        BuildFlags();
    }

    // =====================================================
    // FLAG GOALS (3 wave = 2 flag)
    // =====================================================

    private void GenerateFlagGoals(int wave1, int wave2)
    {
        flagGoals.Clear();

        int cumulative = wave1;
        flagGoals.Add(cumulative);

        cumulative += wave2;
        flagGoals.Add(cumulative);
    }

    // =====================================================
    // BUILD FLAGS (RIGHT → LEFT PERFECT, NO DRIFT)
    // =====================================================

    private void BuildFlags()
    {
        flagsOrdered.Clear();

        foreach (Transform child in flagContainer)
            Destroy(child.gameObject);

        Canvas.ForceUpdateCanvases();

        foreach (int goal in flagGoals)
        {
            GameObject flag =
                Instantiate(flagPrefab, flagContainer);

            RectTransform rt =
                flag.GetComponent<RectTransform>();

            rt.localScale =
                new Vector3(0.4f, 0.4f, 1f);

            rt.sizeDelta =
                new Vector2(30, 60);

            float normalized =
                (float)goal / totalEnemiesThisCycle;

            float anchor =
                1f - normalized;

            rt.anchorMin =
                new Vector2(anchor, 0.5f);

            rt.anchorMax =
                new Vector2(anchor, 0.5f);

            rt.pivot =
                new Vector2(0.5f, 0.5f);

            rt.anchoredPosition =
                Vector2.zero;

            FlagsManager fm =
                flag.GetComponent<FlagsManager>();

            flagsOrdered.Add(fm);
        }
    }

    // =====================================================
    // ACCURATE PROGRESS UPDATE
    // (dipanggil EndlessWaveManager berdasarkan alive difference)
    // =====================================================

    public void AddKills(int amount)
    {
        totalKillsThisCycle += amount;

        slider.value = totalKillsThisCycle;
    }

    // =====================================================
    // WAVE COMPLETE
    // INI YANG MENGUBAH FLAG MENJADI MERAH
    // dipanggil EndlessWaveManager setelah waveMusicStart
    // =====================================================

    public void MarkWaveComplete(int waveIndex)
    {
        if (waveIndex >= 0 && waveIndex < flagsOrdered.Count)
        {
            flagsOrdered[waveIndex].Expand();
        }

        totalFlagsAllCycles++;

        GameData.Data.CurrentFlags =
            totalFlagsAllCycles;

        UpdateTotalFlagsText();
    }

    // =====================================================
    // BIG WAVE WARNING
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
            totalFlagText.text =
                $"Flags: {totalFlagsAllCycles}";

        if (totalFlagSummaryText)
            totalFlagSummaryText.text =
                $"Total flag captured : {totalFlagsAllCycles}";
    }

    // =====================================================
    // RESET
    // =====================================================

    public void ResetUIForGameOver()
    {
        currentCycleVisual = 0;
        totalFlagsAllCycles = 0;

        GameData.Data.CurrentFlags = 0;

        slider.value = 0;

        UpdateTotalFlagsText();
        EnterSelectPhase();
    }
}