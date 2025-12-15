using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{

    [Header("Wave Configuration")]
    public List<int> enemiesPerWave;

    [Header("Spawn Mode per Wave")]
    [Tooltip("Atur mode spawn untuk setiap wave. Jika jumlahnya kurang dari jumlah wave, sisanya pakai mode terakhir.")]
    public List<BacteriaSpawner.SpawnMode> spawnModesPerWave;

    [Header("UI")]
    public Slider levelProgress;
    public RectTransform flagContainer;
    public GameObject flagPrefab;

    [Header("UI Win Panel")]
    public GameObject winUI;

    [Header("Spawner")]
    public BacteriaSpawner spawner;

    private int currentWaveIndex = 0;
    private int currentAliveEnemies = 0;
    private int totalKills = 0;
    private int totalEnemiesInLevel = 0;

    private Dictionary<int, FlagsManager> goalToFlag = new();
    private HashSet<int> triggeredGoals = new();
    private List<int> levelGoals = new(); // otomatis (kumulatif)

    public static bool isGameOver = false;
    public System.Action OnLevelComplete;

    void Start()
    {
        if (winUI != null) winUI.SetActive(false);
        isGameOver = false;

        // Hitung total & goals otomatis
        totalEnemiesInLevel = 0;
        foreach (var n in enemiesPerWave) totalEnemiesInLevel += n;
        GenerateLevelGoals();

        if (levelProgress != null)
        {
            levelProgress.maxValue = totalEnemiesInLevel;
            levelProgress.value = 0;
        }

        SetupFlags();

        if (spawner != null)
            spawner.OnWaveSpawnComplete += OnWaveSpawned;

        StartWave(0);
    }

    private void GenerateLevelGoals()
    {
        levelGoals.Clear();
        int cumulative = 0;
        foreach (var count in enemiesPerWave)
        {
            cumulative += count;
            levelGoals.Add(cumulative);
        }
        Debug.Log("[WaveManager] Level goals otomatis: " + string.Join(", ", levelGoals));
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
            // ✅ Semua wave selesai DI-SPAWN.
            // Sekarang pastikan: hentikan spawner lalu tunggu sampai SEMUA bakteri benar-benar habis.
            Debug.Log("[WaveManager] Semua wave selesai. Menghentikan spawner & menunggu semua musuh mati...");
            if (spawner != null) spawner.StopAllSpawning();
            StartCoroutine(WaitUntilAllEnemiesDeadThenWin());
            return;
        }

        int amount = enemiesPerWave[waveIndex];
        currentAliveEnemies = amount;

        // Atur mode spawn wave ini
        if (spawner != null)
        {
            if (spawnModesPerWave != null && spawnModesPerWave.Count > 0)
            {
                int modeIndex = Mathf.Min(waveIndex, spawnModesPerWave.Count - 1);
                spawner.Mode = spawnModesPerWave[modeIndex];
                Debug.Log($"[WaveManager] Wave {waveIndex + 1} menggunakan mode spawn: {spawner.Mode}");
            }
            else
            {
                Debug.LogWarning("[WaveManager] spawnModesPerWave kosong, gunakan mode default Spawner.");
            }
        }

        Debug.Log($"[WaveManager] Start Wave {waveIndex + 1}, jumlah musuh: {amount}");
        spawner.StartWave(amount);
    }

    private void OnWaveSpawned()
    {
        Debug.Log("[WaveManager] Wave ini selesai DI-SPAWN (belum tentu semua musuh mati).");
    }

    public void RegisterKill()
    {
        totalKills++;
        currentAliveEnemies--;
        if (levelProgress != null) levelProgress.value = totalKills;

        // Trigger flags
        foreach (int g in levelGoals)
        {
            if (totalKills >= g && !triggeredGoals.Contains(g))
            {
                triggeredGoals.Add(g);
                if (goalToFlag.TryGetValue(g, out var fm) && fm != null) fm.Expand();
                Debug.Log($"[WaveManager] Flag @{g} aktif (totalKills={totalKills}).");
            }
        }

        // Jika wave ini sudah tidak ada musuh (berdasarkan quota), lanjut wave berikutnya
        if (currentAliveEnemies <= 0)
        {
            currentWaveIndex++;
            StartWave(currentWaveIndex);
        }
    }

    private IEnumerator WaitUntilAllEnemiesDeadThenWin()
    {
        // Tunggu sampai tidak ada bakteri aktif di scene
        while (CountAllAliveEnemies() > 0)
            yield return new WaitForSeconds(0.5f);

        Debug.Log("[WaveManager] Semua musuh mati. Level benar-benar selesai!");
        OnLevelComplete?.Invoke();
        ShowWinUI();
    }

    private int CountAllAliveEnemies()
    {
        // Gunakan spawner jika ada; fallback ke pencarian langsung.
        if (spawner != null) return spawner.CountAllAlive();

        int total = 0;
        total += FindObjectsByType<BacteriaControllerGreen>(FindObjectsSortMode.None).Length;
        total += FindObjectsByType<BacteriaControllerPurple>(FindObjectsSortMode.None).Length;
        total += FindObjectsByType<BacteriaControllerRed>(FindObjectsSortMode.None).Length;
        return total;
    }

    private void ShowWinUI()
    {
        if (winUI != null)
        {
            winUI.SetActive(true);
            Time.timeScale = 0f;
            isGameOver = true;
            Debug.Log("[WaveManager] UI Win muncul (semua musuh sudah mati) dan game dipause.");
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        if (winUI != null) winUI.SetActive(false);
        Debug.Log("[WaveManager] Game dilanjutkan lagi.");
    }

    
}