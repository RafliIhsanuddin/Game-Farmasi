using UnityEngine;

public class EndlessDifficultyScaler : MonoBehaviour
{
    [Header("Base Counts per Wave (Cycle 1)")]
    [SerializeField] private int baseWave1Count = 10;
    [SerializeField] private int baseWave2Count = 12;

    [Header("Increment Per Cycle")]
    [SerializeField] private int incrementPerCycle = 4;

    private int currentCycle = 1;

    public int CurrentCycle => currentCycle;

    public int GetWave1Count()
    {
        return baseWave1Count + (currentCycle - 1) * incrementPerCycle;
    }

    public int GetWave2Count()
    {
        return baseWave2Count + (currentCycle - 1) * incrementPerCycle;
    }

    public void AdvanceCycle()
    {
        currentCycle++;
        Debug.Log($"[EndlessDifficulty] AdvanceCycle → {currentCycle}");
    }
}