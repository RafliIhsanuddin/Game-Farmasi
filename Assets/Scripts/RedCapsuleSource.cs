using UnityEngine;
using UnityEngine.EventSystems;

public class RedCapsuleSource : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    [SerializeField] private GameObject obatPrefab;
    private GameObject currentObat;

    private void Start()
    {
        canvas = GameObject.Find("CanvasLv3").GetComponent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        currentObat = Instantiate(obatPrefab, transform.parent);
        currentObat.transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentObat == null) return;

        RectTransform rect = currentObat.GetComponent<RectTransform>();
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        currentObat = null;
    }
}