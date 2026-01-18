using UnityEngine;
using System.Collections.Generic;


public class SpawnPreviewHelper : MonoBehaviour
{
    [Header("Preview Slots (4 Posisi)")]
    [SerializeField] private Transform[] previewSlots;

    [Header("Enemy Prefabs (semua musuh lengkap)")]
    [SerializeField] private List<GameObject> allEnemyPrefabs = new List<GameObject>();

    [Header("Jumlah Random Preview")]
    [SerializeField] private int previewCount = 4;

    [Header("Preview Visual Settings")]
    [SerializeField] private float previewScale = 1.5f;

    private readonly List<GameObject> previewPool = new List<GameObject>();
    private readonly List<GameObject> previewInstances = new List<GameObject>();

    // Memory cycle sebelumnya (A: cycle1 bebas, cycle2+ enforce)
    private List<GameObject> lastPreviewPool = null;

    public List<GameObject> GetSelectedEnemyPool()
    {
        return previewPool;
    }

    private void Start()
    {
        GeneratePreviewWithMinDifference();
    }

    public void GeneratePreviewWithMinDifference()
    {
        int attempts = 0;
        const int MAX_ATTEMPTS = 20;

        bool firstCycle = (lastPreviewPool == null);

        do
        {
            attempts++;
            GeneratePreviewCore(); // generate + visual

            if (firstCycle) break; // Cycle 1 bebas sesuai aturan A

            int diff = CountDifference(previewPool, lastPreviewPool);

            if (diff >= 2)
            {
                Debug.Log($"[Preview] OK (diff={diff}) after {attempts} attempts");
                break;
            }

            // gagal → clear visual → ulang
            Debug.Log($"[Preview] REJECT (diff={diff}) retry...");
            ClearPreview();

        } while (attempts < MAX_ATTEMPTS);

        lastPreviewPool = new List<GameObject>(previewPool);
        Debug.Log($"[Preview] FINAL POOL: {string.Join(", ", previewPool)}");
    }

    private int CountDifference(List<GameObject> a, List<GameObject> b)
    {
        int diff = 0;
        foreach (var x in a)
            if (!b.Contains(x)) diff++;
        return diff;
    }

    private void GeneratePreviewCore()
    {
        if (previewSlots == null || previewSlots.Length == 0) return;
        if (allEnemyPrefabs.Count < previewCount) return;

        ClearPreview();
        previewPool.Clear();

        List<GameObject> temp = new List<GameObject>(allEnemyPrefabs);

        for (int i = 0; i < previewCount; i++)
        {
            int idx = Random.Range(0, temp.Count);
            GameObject chosen = temp[idx];
            temp.RemoveAt(idx);

            previewPool.Add(chosen);

            var inst = Instantiate(chosen, previewSlots[i].position, previewSlots[i].rotation);
            previewInstances.Add(inst);
            SetupPreviewInstance(inst);
        }
    }

    private void SetupPreviewInstance(GameObject inst)
    {
        inst.transform.localScale *= previewScale;

        Animator animator = inst.GetComponentInChildren<Animator>();

        var scripts = inst.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (var s in scripts)
        {
            if (animator != null && s == animator) continue;
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
            if (inst != null) Destroy(inst);

        previewInstances.Clear();
    }
}
