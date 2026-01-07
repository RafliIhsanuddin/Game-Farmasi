using UnityEngine;
using UnityEngine.Rendering;

public class TileSpriteOrderController : MonoBehaviour
{
    [Header("Sorting Settings")]
    [SerializeField] private int targetOrderInLayer = 3;

    [Tooltip("Apply automatically when capsule is attached")]
    [SerializeField] private bool applyWhenChildAdded = true;

    [Tooltip("Affect SortingGroup if exists (recommended)")]
    [SerializeField] private bool affectSortingGroup = true;

    // =========================
    // PUBLIC ENTRY POINT
    // =========================
    public void ApplySortingOrder()
    {
        Debug.Log($"[TileSpriteOrder] ApplySortingOrder called on Tile: {name}");

        // -------------------------
        // 1. SORTING GROUP (PRIORITY)
        // -------------------------
        if (affectSortingGroup)
        {
            SortingGroup[] groups = GetComponentsInChildren<SortingGroup>(true);

            if (groups.Length > 0)
            {
                foreach (SortingGroup sg in groups)
                {
                    sg.sortingOrder = targetOrderInLayer;
                    Debug.Log(
                        $"[TileSpriteOrder] ✔ SortingGroup '{sg.name}' set to order {targetOrderInLayer}"
                    );
                }

                Debug.Log(
                    $"[TileSpriteOrder][SUCCESS] SortingGroup detected, SpriteRenderer order is controlled by SortingGroup"
                );
                return; // ❗ STOP HERE (SpriteRenderer WILL be overridden anyway)
            }
        }

        // -------------------------
        // 2. SPRITE RENDERER
        // -------------------------
        SpriteRenderer[] spriteRenderers =
            GetComponentsInChildren<SpriteRenderer>(true);

        if (spriteRenderers.Length == 0)
        {
            Debug.LogWarning(
                $"[TileSpriteOrder][FAILED] No SpriteRenderer found under Tile: {name}"
            );
            return;
        }

        int appliedCount = 0;

        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.sortingOrder = targetOrderInLayer;
            appliedCount++;

            Debug.Log(
                $"[TileSpriteOrder] ✔ SpriteRenderer '{sr.name}' set to order {targetOrderInLayer}"
            );
        }

        Debug.Log(
            $"[TileSpriteOrder][SUCCESS] {appliedCount} SpriteRenderer updated on Tile: {name}"
        );
    }

    // =========================
    // AUTO-DETECT CHILD ADD
    // =========================
    private void OnTransformChildrenChanged()
    {
        if (!applyWhenChildAdded) return;

        Debug.Log(
            $"[TileSpriteOrder] Child hierarchy changed on Tile: {name}"
        );

        ApplySortingOrder();
    }
    
}
