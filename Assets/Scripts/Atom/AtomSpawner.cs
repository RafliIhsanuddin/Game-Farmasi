using UnityEngine;

public class AtomSpawner : MonoBehaviour
{
    public GameObject atomPrefab;
    public WaveManager waveManager; // 🔹 referensi ke WaveManager di Inspector

    private bool isSpawning = true;

    void Start()
    {
        if (waveManager != null)
            waveManager.OnLevelComplete += StopSpawning; // 🔹 subscribe event

        SpawnAtom();
    }

    private void SpawnAtom()
    {
        if (!isSpawning) return; // 🔹 hentikan spawn bila wave sudah selesai

        Instantiate(atomPrefab);
        Invoke(nameof(SpawnAtom), Random.Range(1, 3));
    }

    private void StopSpawning()
    {
        isSpawning = false;
        CancelInvoke(nameof(SpawnAtom)); // 🔹 hentikan semua Invoke yang tersisa
        Debug.Log("[AtomSpawner] Spawn atom dihentikan karena semua wave selesai.");
    }
}
