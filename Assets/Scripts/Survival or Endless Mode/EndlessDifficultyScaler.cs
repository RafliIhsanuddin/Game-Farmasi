using UnityEngine;

public class EndlessDifficultyScaler : MonoBehaviour
{
    [Header("Base Enemy Count")]
    [SerializeField] private int baseWave1Count = 10;
    [SerializeField] private int baseWave2Count = 18;

    [Header("Per Cycle Increment")]
    [SerializeField] private int wave1Increment = 3;
    [SerializeField] private int wave2Increment = 5;

    public int CurrentCycle { get; private set; } = 0;

    public int GetWave1Count()
    {
        int value = baseWave1Count + wave1Increment * CurrentCycle;
        return Mathf.Max(1, value);
    }

    public int GetWave2Count()
    {
        int value = baseWave2Count + wave2Increment * CurrentCycle;
        return Mathf.Max(1, value);
    }

    public void AdvanceCycle()
    {
        CurrentCycle++;
        Debug.Log($"[EndlessDifficulty] Advance → Cycle = {CurrentCycle + 1}");
    }

    public void ResetDifficulty()
    {
        CurrentCycle = 0;
        Debug.Log("[EndlessDifficulty] Reset → Cycle = 1");
    }
}