using UnityEngine;
using System.Collections.Generic;


public class SpawnPreviewHelper : MonoBehaviour
{
    [Header("Preview Slots (3 Posisi)")]
    [SerializeField] private Transform[] previewSlots;

    [Header("Enemy Prefabs (semua musuh lengkap)")]
    [SerializeField] private List<GameObject> allEnemyPrefabs = new List<GameObject>();

    [Header("Jumlah Random Preview")]
    [SerializeField] private int previewCount = 3;

    private readonly List<GameObject> previewPool = new List<GameObject>();
    private readonly List<GameObject> previewInstances = new List<GameObject>();

    public List<GameObject> GetSelectedEnemyPool()
    {
        return previewPool;
    }

    private void Start()
    {
        GeneratePreview();
    }

    public void GeneratePreview()
    {
        if (previewSlots == null || previewSlots.Length == 0)
        {
            Debug.LogWarning("[Preview] Tidak ada previewSlots");
            return;
        }

        if (allEnemyPrefabs.Count < previewCount)
        {
            Debug.LogWarning("[Preview] Enemy prefab kurang untuk random unik");
            return;
        }

        previewPool.Clear();
        previewInstances.Clear();

        List<GameObject> temp = new List<GameObject>(allEnemyPrefabs);

        for (int i = 0; i < previewCount; i++)
        {
            int idx = Random.Range(0, temp.Count);
            GameObject chosen = temp[idx];
            temp.RemoveAt(idx);

            previewPool.Add(chosen);

            GameObject inst = Instantiate(chosen, previewSlots[i].position, previewSlots[i].rotation);
            previewInstances.Add(inst);

            SetupPreviewInstance(inst);
        }
    }

    private void SetupPreviewInstance(GameObject inst)
    {
        Animator animator = inst.GetComponentInChildren<Animator>();

        var scripts = inst.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (var s in scripts)
        {
            if (animator != null && s == animator)
                continue;

            s.enabled = false;
        }

        Collider2D col = inst.GetComponentInChildren<Collider2D>();
        if (col) col.enabled = false;

        Rigidbody2D rb = inst.GetComponentInChildren<Rigidbody2D>();
        if (rb) rb.simulated = false;
    }

    public void ClearPreview()
    {
        foreach (var inst in previewInstances)
        {
            if (inst != null)
                Destroy(inst);
        }

        previewInstances.Clear();
    }
}
