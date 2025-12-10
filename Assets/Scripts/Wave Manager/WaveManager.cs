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

    [Header("Spawner")]
    public BacteriaSpawner spawner;

    // runtime
    private int currentWaveIndex = 0;
    private int currentAliveEnemies = 0;
    private int totalKills = 0;
    private int totalEnemiesInLevel = 0;

    private Dictionary<int, FlagsManager> goalToFlag = new Dictionary<int, FlagsManager>();
    private HashSet<int> triggeredGoals = new HashSet<int>();

    // 🔹 Tambahan event publik untuk notifikasi ke spawner lain
    public System.Action OnLevelComplete;

    void Start()
    {
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
            OnLevelComplete?.Invoke(); // 🔹 Panggil event ketika level selesai
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

        for (int i = 0; i < levelGoals.Count; i++)
        {
            int g = levelGoals[i];
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
                OnLevelComplete?.Invoke(); // 🔹 Pastikan event juga dipanggil di sini
            }
        }
    }
    
}