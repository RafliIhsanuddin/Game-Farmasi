using UnityEngine;

public class EndlessGameManager : MonoBehaviour
{
    [Header("NONAKTIFKAN di Endless")]
    [SerializeField] private GameObject adventureWaveManager;   // WaveManager lama (ADV)

    [Header("Endless System Root (aktif saat Play)")]
    [SerializeField] private GameObject endlessSystemRoot;      // parent EndlessWave, Difficulty, dsb (optional)

    private void Start()
    {
        // Matikan logic WaveManager Adventure
        if (adventureWaveManager)
            adventureWaveManager.SetActive(false);

        // Pastikan endless system aktif
        if (endlessSystemRoot)
            endlessSystemRoot.SetActive(true);
    }
}
