using UnityEngine;
using UnityEngine.UI;

public class CardSelectable : MonoBehaviour
{
    [Header("Return To Selector Target (Static PVZ)")]
    [SerializeField] private GameObject selectorParentGO; // parent target (HUD)
    [SerializeField] private GameObject selectorPosGO;    // posisi target (HUD)

    [Header("Original (Fallback)")]
    [HideInInspector] public Transform originalParent;
    [HideInInspector] public Vector3 originalPosition;

    private CardSelectionManager manager;
    private Button btn;

    private void Awake()
    {
        manager = Object.FindFirstObjectByType<CardSelectionManager>();
        btn = GetComponent<Button>();

        originalParent = transform.parent;
        originalPosition = transform.localPosition;

        var cap = GetComponent<CapsuleSlot>();
        if (cap != null)
            cap.enabled = false;

        if (btn != null)
            btn.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (manager != null)
            manager.OnCardClicked(this);
    }

    public void MoveToSlot(Transform slot)
    {
        transform.SetParent(slot);
    }

    public void ReturnToSelector()
    {
        if (selectorParentGO != null)
        {
            transform.SetParent(selectorParentGO.transform);

            if (selectorPosGO != null)
                transform.position = selectorPosGO.transform.position;
            else
                transform.localPosition = Vector3.zero;

            return;
        }

        transform.SetParent(originalParent);
        transform.localPosition = originalPosition;
    }
}