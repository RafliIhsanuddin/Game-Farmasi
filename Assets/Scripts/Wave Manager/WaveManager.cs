using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{

    [Header("Wave Configuration")]
    public List<int> levelGoals; // contoh: [10, 25, 40]
    public Transform flagContainer;
    public GameObject flagPrefab;
    public Slider levelProgress;

    [Header("Runtime Data")]
    public static int currentBacteriaKilled = 0;

    private int nextGoalIndex = 0;
    private Dictionary<int, FlagsManager> goalFlags = new Dictionary<int, FlagsManager>();

    void Awake()
    {
        goalFlags = new Dictionary<int, FlagsManager>();
    }

    void Start()
    {
        // Buat flag untuk setiap goal
        foreach (int goal in levelGoals)
        {
            GameObject flagObj = Instantiate(flagPrefab, flagContainer);
            FlagsManager flag = flagObj.GetComponent<FlagsManager>();
            goalFlags.Add(goal, flag);
        }

        if (levelProgress != null)
            levelProgress.maxValue = levelGoals[levelGoals.Count - 1];

        nextGoalIndex = 0;
        Debug.Log("[WaveManager] Flags initialized for each wave goal.");
    }

    void Update()
    {
        if (levelProgress != null)
            levelProgress.value = currentBacteriaKilled;

        if (nextGoalIndex < levelGoals.Count &&
            currentBacteriaKilled >= levelGoals[nextGoalIndex])
        {
            int reachedGoal = levelGoals[nextGoalIndex];
            goalFlags[reachedGoal].Expand();

            Debug.Log($"[WaveManager] Wave goal reached: {reachedGoal} kills");

            nextGoalIndex++;
            if (nextGoalIndex >= levelGoals.Count)
            {
                Debug.Log("[WaveManager] All waves completed!");
            }
        }
    }

    // Fungsi ini dipanggil dari GameManager setiap kali bakteri mati
    public void RegisterKill()
    {
        currentBacteriaKilled++;
        Debug.Log($"[WaveManager] Current kills: {currentBacteriaKilled}");
    }
    
}