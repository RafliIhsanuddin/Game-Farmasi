using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{

    [Header("Wave Configuration")]
    public List<int> levelGoals;          // contoh: [5,10,20] (wajib urut naik)
    public List<int> enemiesPerWave;      // contoh: [5,5,10]

    [Header("UI")]
    public Slider levelProgress;
    [Tooltip("Container untuk meletakkan ikon Flag di atas slider (RectTransform di atas slider).")]
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

    void Start()
    {
        // 1) hitung total musuh level
        totalEnemiesInLevel = 0;
        foreach (var n in enemiesPerWave) totalEnemiesInLevel += n;

        // 2) setup slider (jangan ditulis oleh skrip lain!)
        levelProgress.maxValue = totalEnemiesInLevel;
        levelProgress.value = 0;

        // 3) buat & POSISIKAN flag secara proporsional di atas slider
        SetupFlags();

        // 4) subscribe event spawner
        if (spawner != null) spawner.OnWaveSpawnComplete += OnWaveSpawned;

        // 5) mulai wave pertama
        StartWave(0);
    }

    private void SetupFlags()
    {
        goalToFlag.Clear();

        // validasi: goal terakhir sebaiknya == total musuh
        if (levelGoals.Count == 0 || levelGoals[levelGoals.Count - 1] != totalEnemiesInLevel)
            Debug.LogWarning("[WaveManager] Saran: levelGoals terakhir sebaiknya sama dengan total musuh level.");

        for (int i = 0; i < levelGoals.Count; i++)
        {
            int goal = levelGoals[i];

            GameObject flagObj = Instantiate(flagPrefab, flagContainer);
            var fm = flagObj.GetComponent<FlagsManager>();
            goalToFlag[goal] = fm;

            // --- POSISI PROPORSIONAL ---
            // t = progress relatif (0..1)
            float t = Mathf.Clamp01((float)goal / totalEnemiesInLevel);
            var rt = flagObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(t, 0.5f);
            rt.anchorMax = new Vector2(t, 0.5f);
            rt.anchoredPosition = Vector2.zero; // tepat di titik t
        }
    }

    private void StartWave(int waveIndex)
    {
        if (waveIndex >= enemiesPerWave.Count) { Debug.Log("[WaveManager] Semua wave selesai."); return; }
        int amount = enemiesPerWave[waveIndex];
        currentAliveEnemies = amount;

        Debug.Log($"[WaveManager] Start Wave {waveIndex + 1} spawn {amount}");
        spawner.StartWave(amount);
    }

    private void OnWaveSpawned()
    {
        // info saja
        Debug.Log("[WaveManager] Wave ini selesai DI-SPAWN (belum tentu mati).");
    }

    // Dipanggil bakteri saat mati
    public void RegisterKill()
    {
        totalKills++;
        currentAliveEnemies--;

        // update bar
        levelProgress.value = totalKills;

        // TRIGGER FLAG kalau fill sudah MENCAPAI / MELEWATI goal
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
            if (currentWaveIndex < enemiesPerWave.Count) StartWave(currentWaveIndex);
            else Debug.Log("[WaveManager] Level Selesai.");
        }
    }
    
}