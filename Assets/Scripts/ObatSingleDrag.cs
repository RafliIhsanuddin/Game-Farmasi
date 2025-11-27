using UnityEngine;
using UnityEngine.EventSystems;

public class ObatSingleDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject obatPrefab;
    private Canvas canvas;
    private GameObject obatClone;
    private RectTransform cloneRect;

    private void Start()
    {
        canvas = GameObject.Find("CanvasLv3").GetComponent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Spawn obat baru
        obatClone = Instantiate(obatPrefab, transform.parent);
        obatClone.transform.SetAsLastSibling();

        cloneRect = obatClone.GetComponent<RectTransform>();

        Debug.Log("Obat clone created!");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (obatClone == null) return;

        cloneRect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (obatClone == null) return;

        // Cek bakteri via OverlapPoint
        Vector3 screenPos = eventData.position;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Vector2 point2D = new Vector2(worldPos.x, worldPos.y);

        Collider2D hit = Physics2D.OverlapPoint(point2D);

        if (hit != null)
        {
            Debug.Log("Hit: " + hit.name + " tag=" + hit.tag);

            if (hit.CompareTag("Bacteria"))
            {
                Bacteria bac = hit.GetComponent<Bacteria>();
                if (bac != null) bac.TakeDamage();

                Destroy(obatClone);
                return;
            }
        }

        // Jika tidak kena bakteri → tetap destroy obat agar tidak menumpuk
        Destroy(obatClone);
    }
}