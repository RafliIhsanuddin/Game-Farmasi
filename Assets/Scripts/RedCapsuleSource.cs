using UnityEngine;
using UnityEngine.EventSystems;

public class RedCapsuleSource : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject redCapsulePrefab; // prefab red capsule biasa

    private Canvas canvas;
    private RectTransform rectTransform;

    private GameObject spawnedCapsule;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GameObject.Find("CanvasLv3").GetComponent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Spawn prefab saat mulai drag
        if (spawnedCapsule == null)
        {
            spawnedCapsule = Instantiate(redCapsulePrefab, canvas.transform);

            // Set posisi clone sama dengan source
            RectTransform cloneRect = spawnedCapsule.GetComponent<RectTransform>();
            cloneRect.anchoredPosition = rectTransform.anchoredPosition;
            cloneRect.localScale = rectTransform.localScale;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (spawnedCapsule != null)
        {
            RectTransform cloneRect = spawnedCapsule.GetComponent<RectTransform>();
            cloneRect.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        spawnedCapsule = null; // reset agar bisa spawn lagi di drag berikutnya
    }
}
