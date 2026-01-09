using UnityEngine;
using UnityEngine.UI;

public class CardSelectable : MonoBehaviour
{
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
        if (cap != null) cap.enabled = false;

        btn.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        manager.OnCardClicked(this);
    }

    public void MoveToSlot(Transform slot)
    {
        transform.SetParent(slot);
    }

    public void ReturnToSelector()
    {
        transform.SetParent(originalParent);
        transform.localPosition = originalPosition;
    }
}
