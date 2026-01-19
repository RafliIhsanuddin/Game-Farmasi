using UnityEngine;

public class EndlessGameManager : MonoBehaviour
{
    [Header("NONAKTIFKAN di Endless")]
    [SerializeField] private GameObject adventureWaveManager;

    [Header("Endless System Root (aktif saat Play)")]
    [SerializeField] private GameObject endlessSystemRoot;

    private void Start()
    {
        if (adventureWaveManager)
            adventureWaveManager.SetActive(false);

        if (endlessSystemRoot)
            endlessSystemRoot.SetActive(true);
    }
}
