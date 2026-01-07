using UnityEngine;

public class Tile : MonoBehaviour
{

    [Header("Capsule Status")]
    public bool hasCapsule;
    public GameObject currentCapsule;

    public void SetCapsule(GameObject capsule)
    {
        hasCapsule = true;
        currentCapsule = capsule;

        // 🔴 INI KUNCI UTAMA
        capsule.transform.SetParent(transform, false);

        Debug.Log(
            $"[Tile] Capsule '{capsule.name}' attached to Tile: {name}"
        );

        // Trigger sorting refresh
        GetComponent<TileSpriteOrderController>()
            ?.ApplySortingOrder();
    }

    public void ClearCapsule()
    {
        hasCapsule = false;
        currentCapsule = null;
    }
    
    
}
