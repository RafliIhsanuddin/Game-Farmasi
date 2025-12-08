using UnityEngine;
using UnityEngine.UI;


public class FlagsManager : MonoBehaviour
{
    [Header("Flag Sprites")]
    public Sprite defaultSprite;
    public Sprite expandedSprite;

    private Image flagImage;
    private bool isExpanded = false;

    void Awake()
    {
        flagImage = GetComponent<Image>();
        if (flagImage != null && defaultSprite != null)
            flagImage.sprite = defaultSprite;
    }

    public void Expand()
    {
        if (flagImage == null || expandedSprite == null || isExpanded)
            return;

        flagImage.sprite = expandedSprite;
        flagImage.SetNativeSize();
        isExpanded = true;
        Debug.Log("[FlagsManager] Flag expanded!");
    }
    
    
}
