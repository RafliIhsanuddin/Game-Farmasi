using UnityEngine;
using UnityEngine.EventSystems;

public class RedCapsule : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Cari canvas bernama "CanvasLv3"
        canvas = GameObject.Find("CanvasLv3").GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("CanvasLv3 tidak ditemukan!");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // 🔥 Convert UI screen position → world space
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;

        // 🔥 Raycast untuk cari bakteri
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("Bacteria"))
        {
            hit.collider.GetComponent<Bacteria>().TakeDamage();
            Destroy(gameObject);
        }
    }
}
