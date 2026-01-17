using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EndlessUIController : MonoBehaviour
{
    [Header("Progress UI (per Cycle)")]
    [SerializeField] private Slider slider;
    [SerializeField] private RectTransform flagContainer;
    [SerializeField] private GameObject flagPrefab;
    [SerializeField] private GameObject bigWaveWarning;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI cycleText;
    [SerializeField] private TextMeshProUGUI totalFlagText;

    private int totalEnemiesThisCycle;
    private int totalKillsThisCycle;
    private int totalFlagsAllCycles;

    private int wave1Goal;
    private int wave2Goal;

    // Cycle visual untuk UI (tidak reset saat SELECT)
    private int currentCycleVisual = 0;

    private readonly List<FlagsManager> cycleFlags = new List<FlagsManager>();

    public int TotalFlags => totalFlagsAllCycles;
    public int CurrentCycle => currentCycleVisual;

    private void Start()
    {
        if (bigWaveWarning)
            bigWaveWarning.SetActive(false);

        totalFlagsAllCycles = 0;
        currentCycleVisual = 0;

        UpdateCycleText();
        UpdateTotalFlagsText();

        Debug.Log("<color=cyan>[EndlessUI] Start → Cycle: 0, Flags: 0</color>");
    }

    // Dipanggil EndlessWaveManager saat PLAY start
    public void SetupNewCycle(int cycleIndex, int wave1Count, int wave2Count)
    {
        totalEnemiesThisCycle = wave1Count + wave2Count;
        totalKillsThisCycle = 0;

        if (slider != null)
        {
            slider.minValue = 0;
            slider.maxValue = totalEnemiesThisCycle;
            slider.value = 0;
        }

        wave1Goal = wave1Count;
        wave2Goal = totalEnemiesThisCycle;

        currentCycleVisual = cycleIndex;
        UpdateCycleText();

        Debug.Log($"<color=orange>[EndlessUI] Setup Cycle → {currentCycleVisual}</color>");

        RebuildFlags();
    }

    private void RebuildFlags()
    {
        cycleFlags.Clear();

        if (flagContainer == null || flagPrefab == null || totalEnemiesThisCycle <= 0)
            return;

        foreach (Transform c in flagContainer)
            GameObject.Destroy(c.gameObject);

        float t1 = (float)wave1Goal / totalEnemiesThisCycle;
        float t2 = (float)wave2Goal / totalEnemiesThisCycle;

        float[] anchors = { t1, t2 };

        for (int i = 0; i < anchors.Length; i++)
        {
            GameObject f = GameObject.Instantiate(flagPrefab, flagContainer);

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

    public void MarkWaveComplete(int waveIndex)
    {
        if (waveIndex < 0 || waveIndex >= cycleFlags.Count)
            return;

        var fm = cycleFlags[waveIndex];
        if (fm != null)
            fm.Expand();

        totalFlagsAllCycles++;
        UpdateTotalFlagsText();
    }

    // === NEW === (buat ReadyButton)
    public void IncrementCycleVisual()
    {
        currentCycleVisual++;
        UpdateCycleText();

        Debug.Log($"<color=lime>[EndlessUI] Cycle Masuk → {currentCycleVisual}</color>");
    }

    private void UpdateCycleText()
    {
        if (cycleText != null)
            cycleText.text = $"Cycle: {currentCycleVisual}";
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

    // dipanggil saat endless kalah
    public void ResetUIForGameOver()
    {
        currentCycleVisual = 0;
        totalFlagsAllCycles = 0;

        UpdateCycleText();
        UpdateTotalFlagsText();

        Debug.Log("<color=red>[EndlessUI] Reset UI (GameOver)</color>");
    }
}
