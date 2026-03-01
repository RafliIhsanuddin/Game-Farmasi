using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PreviewNameDisplay : MonoBehaviour
{
    [Header("Reference ke SpawnPreviewHelper")]
    [SerializeField] private SpawnPreviewHelper spawnPreviewHelper;
    
    [Header("4 TextMeshPro (Sesuai Slot)")]
    [SerializeField] private TextMeshProUGUI textSlot1;
    [SerializeField] private TextMeshProUGUI textSlot2;
    [SerializeField] private TextMeshProUGUI textSlot3;
    [SerializeField] private TextMeshProUGUI textSlot4;
    
    [Header("Mapping Prefab Name ke Display Name (7 buah)")]
    [SerializeField] private string prefabName1;
    [SerializeField] private string displayName1;
    
    [SerializeField] private string prefabName2;
    [SerializeField] private string displayName2;
    
    [SerializeField] private string prefabName3;
    [SerializeField] private string displayName3;
    
    [SerializeField] private string prefabName4;
    [SerializeField] private string displayName4;
    
    [SerializeField] private string prefabName5;
    [SerializeField] private string displayName5;
    
    [SerializeField] private string prefabName6;
    [SerializeField] private string displayName6;
    
    [SerializeField] private string prefabName7;
    [SerializeField] private string displayName7;
    
    private Dictionary<string, string> mapping = new Dictionary<string, string>();
    private string lastSlot1Text = "";
    private string lastSlot2Text = "";
    private string lastSlot3Text = "";
    private string lastSlot4Text = "";
    
    private void Start()
    {
        Debug.Log("=== PREVIEW NAME DISPLAY START ===");
        
        // Isi dictionary dengan 7 mapping
        mapping.Clear();
        
        AddMapping(prefabName1, displayName1);
        AddMapping(prefabName2, displayName2);
        AddMapping(prefabName3, displayName3);
        AddMapping(prefabName4, displayName4);
        AddMapping(prefabName5, displayName5);
        AddMapping(prefabName6, displayName6);
        AddMapping(prefabName7, displayName7);
        
        // Tampilkan semua mapping yang sudah dimasukkan
        Debug.Log($"Jumlah mapping: {mapping.Count}");
        foreach (var item in mapping)
        {
            Debug.Log($"Mapping: '{item.Key}' -> '{item.Value}'");
        }
    }
    
    private void Update()
    {
        // Update setiap frame - simple dan pasti jalan
        UpdateLabelsIfChanged();
    }
    
    private void UpdateLabelsIfChanged()
    {
        if (spawnPreviewHelper == null) return;
        
        List<GameObject> previewPool = spawnPreviewHelper.GetSelectedEnemyPool();
        
        // Cek dan update Slot 1
        if (textSlot1 != null)
        {
            string newText = GetSlotText(previewPool, 0);
            if (textSlot1.text != newText)
            {
                textSlot1.text = newText;
                Debug.Log($"Slot 1 berubah: '{lastSlot1Text}' -> '{newText}'");
                lastSlot1Text = newText;
            }
        }
        
        // Slot 2
        if (textSlot2 != null)
        {
            string newText = GetSlotText(previewPool, 1);
            if (textSlot2.text != newText)
            {
                textSlot2.text = newText;
                Debug.Log($"Slot 2 berubah: '{lastSlot2Text}' -> '{newText}'");
                lastSlot2Text = newText;
            }
        }
        
        // Slot 3
        if (textSlot3 != null)
        {
            string newText = GetSlotText(previewPool, 2);
            if (textSlot3.text != newText)
            {
                textSlot3.text = newText;
                Debug.Log($"Slot 3 berubah: '{lastSlot3Text}' -> '{newText}'");
                lastSlot3Text = newText;
            }
        }
        
        // Slot 4
        if (textSlot4 != null)
        {
            string newText = GetSlotText(previewPool, 3);
            if (textSlot4.text != newText)
            {
                textSlot4.text = newText;
                Debug.Log($"Slot 4 berubah: '{lastSlot4Text}' -> '{newText}'");
                lastSlot4Text = newText;
            }
        }
    }
    
    private string GetSlotText(List<GameObject> previewPool, int slotIndex)
    {
        if (previewPool == null || previewPool.Count <= slotIndex || previewPool[slotIndex] == null)
        {
            return "";
        }
        
        string prefabName = CleanPrefabName(previewPool[slotIndex].name);
        return GetDisplayName(prefabName);
    }
    
    private void AddMapping(string prefab, string display)
    {
        if (!string.IsNullOrEmpty(prefab) && !string.IsNullOrEmpty(display))
        {
            mapping[prefab] = display;
            Debug.Log($"Mapping ditambahkan: '{prefab}' -> '{display}'");
        }
    }
    
    private string CleanPrefabName(string name)
    {
        if (name.Contains("(Clone)"))
        {
            return name.Replace("(Clone)", "").Trim();
        }
        return name;
    }
    
    private string GetDisplayName(string prefabName)
    {
        if (mapping.ContainsKey(prefabName))
        {
            return mapping[prefabName];
        }
        
        // Kalau tidak ada di mapping, return prefabName
        Debug.LogWarning($"GetDisplayName: '{prefabName}' TIDAK ditemukan di mapping!");
        return prefabName;
    }
    
    // Untuk force update manual
    [ContextMenu("Force Update Now")]
    public void ForceUpdateNow()
    {
        Debug.Log("Force Update Now");
        lastSlot1Text = "";
        lastSlot2Text = "";
        lastSlot3Text = "";
        lastSlot4Text = "";
        UpdateLabelsIfChanged();
    }
}