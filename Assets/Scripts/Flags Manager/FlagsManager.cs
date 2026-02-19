using UnityEngine;
using UnityEngine.UI;

public class FlagsManager : MonoBehaviour
{
    [Header("Flag Sprites")]
    public Sprite defaultSprite;
    public Sprite expandedSprite;

    private Image flagImage;
    private RectTransform rt;

    private bool isExpanded = false;

    void Awake()
    {
        flagImage = GetComponent<Image>();
        rt = GetComponent<RectTransform>();

        if (flagImage == null)
        {
            Debug.LogError("[FlagsManager] ❌ Image component tidak ditemukan!");
            return;
        }

        ApplyFixedSize();

        if (defaultSprite != null)
        {
            flagImage.sprite = defaultSprite;
            Debug.Log("[FlagsManager] Default sprite applied: " + defaultSprite.name);
        }
        else
        {
            Debug.LogWarning("[FlagsManager] ⚠️ defaultSprite belum diisi!");
        }
    }

    private void ApplyFixedSize()
    {
        if (rt == null)
        {
            Debug.LogError("[FlagsManager] ❌ RectTransform tidak ditemukan!");
            return;
        }

        rt.localScale = new Vector3(0.4f, 0.4f, 1f);
        rt.sizeDelta = new Vector2(30f, 60f);

        Debug.Log("[FlagsManager] Size dipaksa: scale=0.4 width=30 height=60");
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

        flagImage.sprite = expandedSprite;

        // JANGAN gunakan SetNativeSize()
        ApplyFixedSize();

        isExpanded = true;

        Debug.Log($"[FlagsManager] ✅ Flag berhasil diubah! Sprite baru: {flagImage.sprite.name}");
    }
}
