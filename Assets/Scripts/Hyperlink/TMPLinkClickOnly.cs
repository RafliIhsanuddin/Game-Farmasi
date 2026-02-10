using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMPLinkClickOnly : MonoBehaviour, IPointerClickHandler
{
    [Header("Target TMP (Auto jika kosong)")]
    [SerializeField] private TextMeshProUGUI tmpText;

    private void Awake()
    {
        // Jika belum di-assign lewat Inspector
        if (tmpText == null)
        {
            tmpText = GetComponent<TextMeshProUGUI>();
        }

        // Safety check terakhir
        if (tmpText == null)
        {
            Debug.LogError(
                $"[TMPLinkClickOnly] TextMeshProUGUI tidak ditemukan di GameObject '{gameObject.name}'",
                this
            );
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Saat di Editor: otomatis isi jika kosong
        if (tmpText == null)
        {
            tmpText = GetComponent<TextMeshProUGUI>();
        }

        // Warning langsung terlihat di Inspector
        if (tmpText == null)
        {
            Debug.LogWarning(
                $"[TMPLinkClickOnly] Tidak ada TextMeshProUGUI di GameObject '{gameObject.name}'. " +
                $"Script ini butuh TMP di object yang sama.",
                this
            );
        }
    }
#endif

    public void OnPointerClick(PointerEventData eventData)
    {
        if (tmpText == null) return;

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(
            tmpText,
            eventData.position,
            eventData.pressEventCamera
        );

        if (linkIndex != -1)
        {
            string url = tmpText.textInfo.linkInfo[linkIndex].GetLinkID();
            Application.OpenURL(url);
        }
    }
}