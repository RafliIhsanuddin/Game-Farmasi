using UnityEngine;

public class EndlessSpeedScaler : MonoBehaviour
{
    public enum SpeedGrowthMode
    {
        Increment,
        Multiplier
    }

    [Header("Mode Pertumbuhan Kecepatan")]
    [SerializeField] private SpeedGrowthMode mode = SpeedGrowthMode.Increment;

    [Header("Increment Per Wave (Mode = Increment)")]
    [SerializeField] private float wave1Increment = 1f;
    [SerializeField] private float wave2Increment = 2f;

    [Header("Multiplier Per Wave (Mode = Multiplier)")]
    [SerializeField] private float wave1Multiplier = 2f;
    [SerializeField] private float wave2Multiplier = 2f;

    // State kumulatif
    private float currentAdd = 0f;   // dipakai kalau mode Increment
    private float currentMul = 1f;   // dipakai kalau mode Multiplier

    // ========= API UMUM =========
    public SpeedGrowthMode Mode => mode;

    public string ModeName => (mode == SpeedGrowthMode.Increment) ? "increment" : "multiplier";

    public void NotifyWaveStart(int waveNumber)
    {
        if (mode == SpeedGrowthMode.Increment)
        {
            float inc = GetIncrementForWave(waveNumber);
            currentAdd += inc;
            Debug.Log($"[SpeedScaler] Wave {waveNumber} start (Increment) → +{inc}, totalAdd={currentAdd}");
        }
        else
        {
            float mul = GetMultiplierForWave(waveNumber);
            currentMul *= mul;
            Debug.Log($"[SpeedScaler] Wave {waveNumber} start (Multiplier) → x{mul}, totalMul={currentMul}");
        }
    }

    /// <summary>
    /// Menerapkan scaling ke baseSpeed (baseSpeed diambil dari prefab enemy).
    /// </summary>
    public float ApplyScale(float baseSpeed)
    {
        if (mode == SpeedGrowthMode.Increment)
        {
            return baseSpeed + currentAdd;
        }
        else
        {
            return baseSpeed * currentMul;
        }
    }

    /// <summary>
    /// Reset kecepatan kumulatif (dipanggil saat GAME OVER).
    /// </summary>
    public void ResetSpeed()
    {
        currentAdd = 0f;
        currentMul = 1f;
        Debug.Log("[SpeedScaler] ResetSpeed() → currentAdd=0, currentMul=1");
    }

    // ========= Helper untuk debug =========
    public float GetIncrementForWave(int waveNumber)
    {
        return (waveNumber == 1) ? wave1Increment : wave2Increment;
    }

    public float GetMultiplierForWave(int waveNumber)
    {
        return (waveNumber == 1) ? wave1Multiplier : wave2Multiplier;
    }
}