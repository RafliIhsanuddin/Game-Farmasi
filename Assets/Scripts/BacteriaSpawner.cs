using UnityEngine;
using System.Collections.Generic;

public class BacteriaSpawner : MonoBehaviour
{
    
    [Header("Spawner Settings")]
    public List<GameObject> bacteriaPrefabs = new List<GameObject>();
    public List<SpawnPointSlot> spawnPoints = new List<SpawnPointSlot>();

    [Header("Controller Scripts")]
    [SerializeField] private List<MonoBehaviour> bacteriaControllers = new List<MonoBehaviour>();
    // Isi daftar ini di Inspector: drag skrip seperti BacteriaControllerGreen, Red, Purple, dll.

    [Header("Spawn Timing")]
    public float firstSpawnDelay = 2f;
    public float repeatRate = 5f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnBacteria), firstSpawnDelay, repeatRate);
    }

    void SpawnBacteria()
    {
        // Cari titik kosong
        List<SpawnPointSlot> emptyPoints = new List<SpawnPointSlot>();
        foreach (var point in spawnPoints)
        {
            if (!point.occupied)
                emptyPoints.Add(point);
        }

        if (emptyPoints.Count == 0)
        {
            Debug.Log("[Spawner] Semua titik penuh!");
            return;
        }

        SpawnPointSlot selected = emptyPoints[Random.Range(0, emptyPoints.Count)];

        if (bacteriaPrefabs.Count == 0)
        {
            Debug.LogWarning("[Spawner] Tidak ada prefab bakteri di daftar!");
            return;
        }

        GameObject chosenPrefab = bacteriaPrefabs[Random.Range(0, bacteriaPrefabs.Count)];
        GameObject newBacteria = Instantiate(chosenPrefab, selected.transform.position, Quaternion.identity);

        // Coba hubungkan ke semua skrip yang kamu isi di Inspector
        foreach (var script in bacteriaControllers)
        {
            if (script == null) continue;

            System.Type scriptType = script.GetType();
            var component = newBacteria.GetComponent(scriptType);
            if (component != null)
            {
                // kalau skrip punya field public/serialized "spawnPoint"
                var field = scriptType.GetField("spawnPoint");
                if (field != null)
                    field.SetValue(component, selected);

                Debug.Log($"[Spawner] Menghubungkan {scriptType.Name} ke {newBacteria.name}");
            }
        }

        selected.SetOccupied(newBacteria);
        //Debug.Log($"[Spawner] Spawn {chosenPrefab.name} di {selected.name}");
    }

}
