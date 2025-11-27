using UnityEngine;
using UnityEngine.EventSystems;

public class RedCapsule : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        canvas = GameObject.Find("CanvasLv3").GetComponent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        Debug.Log("OnEndDrag dijalankan");

        Vector3 screenPos = eventData.position;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Vector2 point2D = new Vector2(worldPos.x, worldPos.y);

        Collider2D hit = Physics2D.OverlapPoint(point2D);

        if (hit != null)
        {
            Debug.Log("Collider terdeteksi: " + hit.name + " | tag = " + hit.tag);

            if (hit.CompareTag("Bacteria"))
            {
                Debug.Log("Bakteri terkena!");

                Bacteria bacteria = hit.GetComponent<Bacteria>();
                if (bacteria != null)
                {
                    Debug.Log("Menjalankan TakeDamage pada bakteri");
                    bacteria.TakeDamage();
                }
                else
                {
                    Debug.LogWarning("Komponen Bacteria tidak ditemukan di object ini!");
                }

                Destroy(gameObject);
                return;
            }
            else
            {
                Debug.Log("Bukan bakteri yang terkena!");
            }
        }
        else
        {
            Debug.LogWarning("Tidak mengenai collider apa pun!");
        }
    }
}
