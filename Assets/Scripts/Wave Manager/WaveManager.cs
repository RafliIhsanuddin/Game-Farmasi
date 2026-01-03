using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{

    [Header("Wave Configuration")]
    public List<int> enemiesPerWave;

    [Header("Spawn Mode per Wave")]
    public List<BacteriaSpawner.SpawnMode> spawnModesPerWave;

    [Header("UI")]
    public Slider levelProgress;
    public RectTransform flagContainer;
    public GameObject flagPrefab;

    [Header("UI Win Panel")]
    public GameObject winUI;

    [Header("Spawner")]
    public BacteriaSpawner spawner;

    // ===============================
    // INTERNAL STATE
    // ===============================
    private int currentWaveIndex = 0;
    private int remainingEnemiesInWave = 0;

    private int totalKills = 0;
    private int totalEnemiesInLevel = 0;

    private Dictionary<int, FlagsManager> goalToFlag = new();
    private HashSet<int> triggeredGoals = new();
    private List<int> levelGoals = new();

    public static bool isGameOver = false;

    // JANGAN DIHILANGKAN
    public System.Action OnLevelComplete;

    // ===============================
    // START
    // ===============================
    private void Start()
    {
        if (winUI != null) winUI.SetActive(false);
        isGameOver = false;

        // Hitung TOTAL MUSUH LEVEL (FIXED)
        totalEnemiesInLevel = 0;
        foreach (var count in enemiesPerWave)
            totalEnemiesInLevel += count;

        GenerateLevelGoals();

        if (levelProgress != null)
        {
            levelProgress.maxValue = totalEnemiesInLevel;
            levelProgress.value = 0;
        }

        SetupFlags();
        StartWave(0);
    }

    // ===============================
    // FLAG SETUP
    // ===============================
    private void GenerateLevelGoals()
    {
        levelGoals.Clear();
        int cumulative = 0;

        foreach (var count in enemiesPerWave)
        {
            cumulative += count;
            levelGoals.Add(cumulative);
        }
    }

    private void SetupFlags()
    {
        goalToFlag.Clear();

        for (int i = 0; i < levelGoals.Count; i++)
        {
            int goal = levelGoals[i];
            GameObject flagObj = Instantiate(flagPrefab, flagContainer);
            var fm = flagObj.GetComponent<FlagsManager>();
            goalToFlag[goal] = fm;

            float t = (float)goal / totalEnemiesInLevel;
            var rt = flagObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(t, 0.5f);
            rt.anchorMax = new Vector2(t, 0.5f);
            rt.anchoredPosition = Vector2.zero;
        }
    }

    // ===============================
    // START WAVE
    // ===============================
    private void StartWave(int waveIndex)
    {
        if (waveIndex >= enemiesPerWave.Count)
        {
            Debug.Log("✅ SEMUA WAVE SELESAI → CEK WIN");
            TryWin();
            return;
        }

        currentWaveIndex = waveIndex;
        remainingEnemiesInWave = enemiesPerWave[waveIndex];

        var mode = spawnModesPerWave.Count > 0
            ? spawnModesPerWave[Mathf.Min(waveIndex, spawnModesPerWave.Count - 1)]
            : BacteriaSpawner.SpawnMode.SinglePerLine;

        spawner.Mode = mode;
        spawner.StartWave(remainingEnemiesInWave);

        Debug.Log($"🚩 WAVE {currentWaveIndex + 1} DIMULAI | Total Musuh: {remainingEnemiesInWave}");
    }

    // ===============================
    // REGISTER KILL (SATU-SATUNYA PINTU)
    // ===============================
    public void RegisterKill()
    {
        if (isGameOver) return;

        totalKills++;
        remainingEnemiesInWave--;

        Debug.Log(
            $"💀 WAVE {currentWaveIndex + 1} | Mati: {enemiesPerWave[currentWaveIndex] - remainingEnemiesInWave} | Sisa: {remainingEnemiesInWave}"
        );

        if (levelProgress != null)
            levelProgress.value = Mathf.Min(totalKills, totalEnemiesInLevel);

        foreach (int g in levelGoals)
        {
            if (totalKills >= g && !triggeredGoals.Contains(g))
            {
                triggeredGoals.Add(g);
                if (goalToFlag.TryGetValue(g, out var fm))
                    fm?.Expand();
            }
        }

        // ===============================
        // WAVE HABIS
        // ===============================
        if (remainingEnemiesInWave <= 0)
        {
            Debug.Log($"✅ WAVE {currentWaveIndex + 1} SELESAI");

            // 🔥 INI KUNCINYA
            if (currentWaveIndex == enemiesPerWave.Count - 1)
            {
                Debug.Log("🛑 WAVE TERAKHIR HABIS → STOP SPAWNER");
                spawner.StopAllSpawning();
                TryWin();
                return;
            }

            StartWave(currentWaveIndex + 1);
        }
    }

    // ===============================
    // WIN CHECK
    // ===============================
    private void TryWin()
    {
        // Semua musuh HARUS sudah mati
        if (totalKills < totalEnemiesInLevel)
        {
            Debug.LogWarning("⚠️ WIN DICEK TAPI TOTAL KILL BELUM CUKUP");
            return;
        }

        StartCoroutine(WaitAllEnemyDeadThenWin());
    }

    private IEnumerator WaitAllEnemyDeadThenWin()
    {
        while (spawner.CountAllAlive() > 0)
            yield return new WaitForSeconds(0.2f);

        TriggerWin();
    }

    // ===============================
    // WIN
    // ===============================
    private void TriggerWin()
    {
        if (isGameOver) return;

        isGameOver = true;

        spawner.StopAllSpawning();

        if (levelProgress != null)
            levelProgress.value = totalEnemiesInLevel;

        Debug.Log("🏆 WAVE TERAKHIR SELESAI → KAMU MENANG");

        GameData.Data.UnlockedLevel++;
        OnLevelComplete?.Invoke();

        if (winUI != null)
        {
            winUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    
}