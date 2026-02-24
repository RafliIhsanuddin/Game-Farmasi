using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UILayerOrderController : MonoBehaviour
{
    [Header("Top = First Element")]
    [SerializeField] private List<GameObject> uiOrderList = new List<GameObject>();

    [Header("Auto Apply On Start")]
    [SerializeField] private bool applyOnStart = true;

    [Header("Base Sorting Order")]
    [SerializeField] private int baseSortingOrder = 100;

    private void Start()
    {
        if (applyOnStart)
        {
            ApplyLayerOrder();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            EditorApplication.delayCall += DelayedApply;
        }
    }

    private void DelayedApply()
    {
        if (this == null) return;
        ApplyLayerOrder();
    }
#endif

    [ContextMenu("Apply Layer Order")]
    public void ApplyLayerOrder()
    {
        if (uiOrderList == null)
            return;

        int currentOrder = baseSortingOrder;

        for (int i = 0; i < uiOrderList.Count; i++)
        {
            GameObject obj = uiOrderList[i];

            if (obj == null)
                continue;

            Canvas canvas = GetOrAddCanvas(obj);

            canvas.overrideSorting = true;
            canvas.sortingOrder = currentOrder;

            currentOrder--;
        }
    }

    private Canvas GetOrAddCanvas(GameObject obj)
    {
        Canvas canvas = obj.GetComponent<Canvas>();

        if (canvas == null)
        {
            canvas = obj.AddComponent<Canvas>();
        }

        if (obj.GetComponent<UnityEngine.UI.GraphicRaycaster>() == null)
        {
            obj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        return canvas;
    }
}