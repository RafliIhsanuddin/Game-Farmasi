using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class FlagGridSynchronizer : MonoBehaviour
{
    [Header("Reference Fill Area (Slider Fill Area)")]
    public RectTransform fillArea;

    private RectTransform flagGrid;

    private void Awake()
    {
        flagGrid = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (fillArea == null || flagGrid == null)
            return;

        SyncRectTransform();
    }

    private void SyncRectTransform()
    {
        // Samakan anchor
        flagGrid.anchorMin = fillArea.anchorMin;
        flagGrid.anchorMax = fillArea.anchorMax;

        // Samakan pivot
        flagGrid.pivot = fillArea.pivot;

        // Samakan posisi
        flagGrid.anchoredPosition = fillArea.anchoredPosition;

        // Samakan ukuran
        flagGrid.sizeDelta = fillArea.sizeDelta;

        // Samakan offset
        flagGrid.offsetMin = fillArea.offsetMin;
        flagGrid.offsetMax = fillArea.offsetMax;

        // Samakan scale
        flagGrid.localScale = fillArea.localScale;
    }
}