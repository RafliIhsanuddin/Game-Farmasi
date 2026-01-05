using UnityEngine;

public class AtomSpawner : MonoBehaviour
{
    public GameObject atomPrefab;
    public WaveManager waveManager;

    [Header("Spawn Delay Settings")]
    [SerializeField] private Vector2 spawnDelayRange = new Vector2(1f, 3f);

    private bool isSpawning = true;

    private void Start()
    {
        if (waveManager != null)
            waveManager.OnLevelComplete += StopSpawning;

        SpawnAtom();
    }

    private void SpawnAtom()
    {
        if (!isSpawning) return;

        // ===============================
        // SPAWN ATOM RANDOM (MODE RANDOM)
        // ===============================
        GameObject atom = Instantiate(atomPrefab);
        Atom atomScript = atom.GetComponent<Atom>();
        if (atomScript != null)
            atomScript.useRandomSpawn = true;

        float delay = Random.Range(spawnDelayRange.x, spawnDelayRange.y);
        Invoke(nameof(SpawnAtom), delay);
    }

    private void StopSpawning()
    {
        isSpawning = false;
        CancelInvoke(nameof(SpawnAtom));
        Debug.Log("[AtomSpawner] Spawn atom dihentikan karena semua wave selesai.");
    }
}
