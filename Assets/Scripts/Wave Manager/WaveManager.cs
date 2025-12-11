using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{

    [Header("Wave Configuration")]
    public List<int> levelGoals;
    public List<int> enemiesPerWave;

    [Header("UI")]
    public Slider levelProgress;
    public RectTransform flagContainer;
    public GameObject flagPrefab;

    [Header("UI Win Panel")]
    public GameObject winUI;

    [Header("Spawner")]
    public BacteriaSpawner spawner;

    // Runtime
    private int currentWaveIndex = 0;
    private int currentAliveEnemies = 0;
    private int totalKills = 0;
    private int totalEnemiesInLevel = 0;
    private Dictionary<int, FlagsManager> goalToFlag = new Dictionary<int, FlagsManager>();
    private HashSet<int> triggeredGoals = new HashSet<int>();

    public static bool isGameOver = false; // 🔹 status global win/lose

    public System.Action OnLevelComplete;

    void Start()
    {
        if (winUI != null)
            winUI.SetActive(false);

        isGameOver = false; // reset setiap mulai level

        totalEnemiesInLevel = 0;
        foreach (var n in enemiesPerWave) totalEnemiesInLevel += n;

        levelProgress.maxValue = totalEnemiesInLevel;
        levelProgress.value = 0;

        SetupFlags();

        if (spawner != null)
            spawner.OnWaveSpawnComplete += OnWaveSpawned;

        StartWave(0);
    }

    private void SetupFlags()
    {
        goalToFlag.Clear();

        if (levelGoals.Count == 0 || levelGoals[levelGoals.Count - 1] != totalEnemiesInLevel)
            Debug.LogWarning("[WaveManager] Saran: levelGoals terakhir sebaiknya sama dengan total musuh level.");

        for (int i = 0; i < levelGoals.Count; i++)
        {
            int goal = levelGoals[i];
            GameObject flagObj = Instantiate(flagPrefab, flagContainer);
            var fm = flagObj.GetComponent<FlagsManager>();
            goalToFlag[goal] = fm;

            float t = Mathf.Clamp01((float)goal / totalEnemiesInLevel);
            var rt = flagObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(t, 0.5f);
            rt.anchorMax = new Vector2(t, 0.5f);
            rt.anchoredPosition = Vector2.zero;
        }
    }

    private void StartWave(int waveIndex)
    {
        if (waveIndex >= enemiesPerWave.Count)
        {
            Debug.Log("[WaveManager] Semua wave selesai.");
            OnLevelComplete?.Invoke();
            ShowWinUI();
            return;
        }

        int amount = enemiesPerWave[waveIndex];
        currentAliveEnemies = amount;

        Debug.Log($"[WaveManager] Start Wave {waveIndex + 1} spawn {amount}");
        spawner.StartWave(amount);
    }

    private void OnWaveSpawned()
    {
        Debug.Log("[WaveManager] Wave ini selesai DI-SPAWN (belum tentu mati).");
    }

    public void RegisterKill()
    {
        totalKills++;
        currentAliveEnemies--;
        levelProgress.value = totalKills;

        foreach (int g in levelGoals)
        {
            if (totalKills >= g && !triggeredGoals.Contains(g))
            {
                triggeredGoals.Add(g);
                if (goalToFlag.TryGetValue(g, out var fm) && fm != null) fm.Expand();
                Debug.Log($"[WaveManager] Flag @{g} aktif (totalKills={totalKills}).");
            }
        }

        if (currentAliveEnemies <= 0)
        {
            currentWaveIndex++;
            if (currentWaveIndex < enemiesPerWave.Count)
                StartWave(currentWaveIndex);
            else
            {
                Debug.Log("[WaveManager] Level Selesai.");
                OnLevelComplete?.Invoke();
                ShowWinUI();
            }
        }
    }

    private void ShowWinUI()
    {
        if (winUI != null)
        {
            winUI.SetActive(true);
            Time.timeScale = 0f;
            isGameOver = true; // 🔹 tandai game berakhir (menang)
            Debug.Log("[WaveManager] UI Win muncul dan game dipause.");
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        if (winUI != null)
            winUI.SetActive(false);
        Debug.Log("[WaveManager] Game dilanjutkan lagi.");
    }
    
}