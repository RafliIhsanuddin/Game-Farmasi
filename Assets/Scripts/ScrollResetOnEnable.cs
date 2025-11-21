using UnityEngine;
using UnityEngine.UI;

public class ScrollResetOnEnable : MonoBehaviour
{
    [Header("Canvas atau GameObject yang Diaktif/Nonaktifkan")]
    [SerializeField] private GameObject targetCanvas;

    [Header("ScrollRect yang Akan Direset")]
    [SerializeField] private ScrollRect scrollRect;

    private void OnEnable()
    {
        if (scrollRect != null)
        {
            // Reset scrollbar ke atas (1 = atas, 0 = bawah)
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }

    private void OnDisable()
    {
        // (Opsional) bisa kosongan, biar clean
    }
}
