using UnityEngine;

public class Tile : MonoBehaviour
{

    [Header("Capsule Status")]
    public bool hasCapsule = false; // apakah tile terisi kapsul?
    public GameObject currentCapsule; // kapsul yang ada di tile ini

    // Opsional: buat mudah set data
    public void SetCapsule(GameObject capsule)
    {
        hasCapsule = true;
        currentCapsule = capsule;
    }

    // Opsional: reset saat kapsul hilang
    public void ClearCapsule()
    {
        hasCapsule = false;
        currentCapsule = null;
    }
    
    
}
