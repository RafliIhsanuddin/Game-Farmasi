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

    private int currentWaveIndex = 0;
    private int currentAliveEnemies = 0;
    private int totalKills = 0;
    private int totalEnemiesInLevel = 0;

    private Dictionary<int, FlagsManager> goalToFlag = new();
    private HashSet<int> triggeredGoals = new();
    private List<int> levelGoals = new();

    public static bool isGameOver = false;

    // ⬇️ JANGAN DIHILANGKAN
    public System.Action OnLevelComplete;

    private void Start()
    {
        if (winUI != null) winUI.SetActive(false);
        isGameOver = false;

        totalEnemiesInLevel = 0;
        foreach (var n in enemiesPerWave)
            totalEnemiesInLevel += n;

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
            if (spawner != null)
                spawner.StopAllSpawning();

            StartCoroutine(WaitUntilAllEnemiesDeadThenWin());
            return;
        }

        int amount = enemiesPerWave[waveIndex];
        currentAliveEnemies = amount;

        if (spawner != null && spawnModesPerWave.Count > 0)
        {
            int modeIndex = Mathf.Min(waveIndex, spawnModesPerWave.Count - 1);
            spawner.Mode = spawnModesPerWave[modeIndex];
        }

        spawner.StartWave(amount);
    }

    private void OnWaveSpawned()
    {
        // wave selesai di-spawn
    }

    public void RegisterKill()
    {
        if (isGameOver) return;

        totalKills++;
        currentAliveEnemies--;

        if (levelProgress != null)
            levelProgress.value = totalKills;

        foreach (int g in levelGoals)
        {
            if (totalKills >= g && !triggeredGoals.Contains(g))
            {
                triggeredGoals.Add(g);
                if (goalToFlag.TryGetValue(g, out var fm) && fm != null)
                    fm.Expand();
            }
        }

        if (currentAliveEnemies <= 0)
        {
            currentWaveIndex++;
            StartWave(currentWaveIndex);
        }
    }

    private IEnumerator WaitUntilAllEnemiesDeadThenWin()
    {
        while (CountAllAliveEnemies() > 0)
            yield return new WaitForSeconds(0.5f);

        // ============================
        // 🔓 UNLOCK LEVEL BERIKUTNYA
        // ============================
        GameData.Data.UnlockedLevel++;

        // ⬇️ EVENT TETAP DIPANGGIL
        OnLevelComplete?.Invoke();

        ShowWinUI();
    }

    private int CountAllAliveEnemies()
    {
        if (spawner != null)
            return spawner.CountAllAlive();

        return 0;
    }

    private void ShowWinUI()
    {
        if (winUI != null)
        {
            winUI.SetActive(true);
            Time.timeScale = 0f;
            isGameOver = true;
        }
    }

    
}