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
    [Tooltip("Skala preview untuk visualisasi, tidak mempengaruhi prefab asli")]
    [SerializeField] private float previewScale = 1.5f;

    private readonly List<GameObject> previewPool = new List<GameObject>();
    private readonly List<GameObject> previewInstances = new List<GameObject>();

    // Dipanggil oleh EndlessPhaseController / ReadyButton
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
            Debug.LogWarning("[Preview] Enemy prefab kurang untuk sampling unik");
            return;
        }

        ClearPreview();
        previewPool.Clear();

        // gunakan list clone supaya bisa remove untuk sampling unik
        List<GameObject> temp = new List<GameObject>(allEnemyPrefabs);

        for (int i = 0; i < previewCount; i++)
        {
            int idx = Random.Range(0, temp.Count);
            GameObject chosen = temp[idx];
            temp.RemoveAt(idx);

            previewPool.Add(chosen);

            // spawn visual preview
            GameObject inst = Instantiate(
                chosen,
                previewSlots[i].position,
                previewSlots[i].rotation
            );

            previewInstances.Add(inst);

            SetupPreviewInstance(inst);
        }

        Debug.Log($"[Preview] GeneratePreview() → pool size = {previewPool.Count}");
    }

    private void SetupPreviewInstance(GameObject inst)
    {
        // SCALE HANYA DI PREVIEW
        inst.transform.localScale *= previewScale;

        // keep animator (idle)
        Animator animator = inst.GetComponentInChildren<Animator>();

        // disable semua script lain
        var scripts = inst.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (var s in scripts)
        {
            if (animator != null && s == animator)
                continue; // animator tetap aktif untuk idle preview

            s.enabled = false;
        }

        // disable collider (jangan bisa collision)
        Collider2D col = inst.GetComponentInChildren<Collider2D>();
        if (col) col.enabled = false;

        // disable physics
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
