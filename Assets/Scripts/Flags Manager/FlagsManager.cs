using UnityEngine;
using UnityEngine.UI;

public class FlagsManager : MonoBehaviour
{
    [Header("Flag Sprites")]
    public Sprite defaultSprite;
    public Sprite expandedSprite;

    private Image flagImage;
    private bool isExpanded = false;

    void Awake()
    {
        flagImage = GetComponent<Image>();
        if (flagImage != null && defaultSprite != null)
            flagImage.sprite = defaultSprite;
    }

    public void Expand()
    {
        Debug.Log("[FlagsManager] Expand() dipanggil!");

        if (flagImage == null)
        {
            Debug.LogWarning("[FlagsManager] ❌ flagImage belum di-assign atau tidak ditemukan!");
            return;
        }
        if (expandedSprite == null)
        {
            Debug.LogWarning("[FlagsManager] ❌ expandedSprite belum diisi di Inspector!");
            return;
        }
        if (isExpanded)
        {
            Debug.LogWarning("[FlagsManager] ⚠️ Bendera sudah expanded sebelumnya, abaikan.");
            return;
        }

        // Ganti sprite
        flagImage.sprite = expandedSprite;
        flagImage.SetNativeSize();
        isExpanded = true;

        Debug.Log($"[FlagsManager] ✅ Flag berhasil diubah! Sprite baru: {flagImage.sprite.name}");
    }
}