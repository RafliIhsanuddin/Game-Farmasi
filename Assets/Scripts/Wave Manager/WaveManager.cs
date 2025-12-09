using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{

    [Header("Wave Configuration")]
    public List<int> levelGoals;
    public List<int> enemiesPerWave;
    public Transform flagContainer;
    public GameObject flagPrefab;
    public Slider levelProgress;

    [Header("Spawner Reference")]
    public BacteriaSpawner spawner;

    [Header("Runtime Data")]
    private int nextWaveIndex = 0;
    private int totalToKillThisWave;
    private int currentAliveEnemies;
    private int totalKillsAllWaves;
    private Dictionary<int, FlagsManager> goalFlags = new Dictionary<int, FlagsManager>();

    void Start()
    {
        goalFlags.Clear();

        // buat flag untuk tiap goal
        for (int i = 0; i < levelGoals.Count; i++)
        {
            GameObject flagObj = Instantiate(flagPrefab, flagContainer);
            FlagsManager flag = flagObj.GetComponent<FlagsManager>();
            goalFlags.Add(levelGoals[i], flag);
        }

        // reset progress awal
        levelProgress.maxValue = levelGoals[levelGoals.Count - 1];
        levelProgress.value = 0f;
        totalKillsAllWaves = 0;

        // callback dari spawner
        if (spawner != null)
            spawner.OnWaveSpawnComplete += OnWaveSpawned;

        // mulai wave pertama
        StartWave(0);
    }

    // ❌ Hapus Update() karena tidak perlu update tiap frame
    // levelProgress sekarang diperbarui HANYA ketika musuh mati

    private void StartWave(int waveIndex)
    {
        if (spawner == null)
        {
            Debug.LogWarning("[WaveManager] Spawner belum di-assign!");
            return;
        }

        if (waveIndex >= enemiesPerWave.Count)
        {
            Debug.Log("[WaveManager] Tidak ada data enemiesPerWave untuk index ini!");
            return;
        }

        int amount = enemiesPerWave[waveIndex];
        totalToKillThisWave = amount;
        currentAliveEnemies = amount;

        Debug.Log($"[WaveManager] Memulai Wave {waveIndex + 1} dengan {amount} bakteri!");
        spawner.StartWave(amount);
    }

    private void OnWaveSpawned()
    {
        Debug.Log("[WaveManager] Semua bakteri di wave ini sudah di-spawn sepenuhnya.");
    }

    // dipanggil dari BacteriaController ketika bakteri mati
    public void RegisterKill()
    {
        totalKillsAllWaves++;
        currentAliveEnemies--;

        Debug.Log($"[WaveManager] Kill {totalKillsAllWaves}/{levelGoals[levelGoals.Count - 1]} | Alive: {currentAliveEnemies}");

        // 🔥 progress hanya naik di sini
        UpdateProgress();

        foreach (var goal in levelGoals)
        {
            if (totalKillsAllWaves == goal)
            {
                goalFlags[goal].Expand();
                Debug.Log($"[WaveManager] Flag untuk wave dengan goal {goal} aktif!");
            }
        }

        if (currentAliveEnemies <= 0)
        {
            Debug.Log($"[WaveManager] Wave {nextWaveIndex + 1} selesai!");
            NextWave();
        }
    }

    private void UpdateProgress()
    {
        if (levelProgress != null)
            levelProgress.value = totalKillsAllWaves;
    }

    private void NextWave()
    {
        nextWaveIndex++;

        if (nextWaveIndex < enemiesPerWave.Count)
        {
            StartWave(nextWaveIndex);
        }
        else
        {
            Debug.Log("[WaveManager] Semua wave selesai!");
        }
    }
    
}