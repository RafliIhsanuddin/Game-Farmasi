using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TMPLinkClickOnly : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI tmpText;

    private void Awake()
    {
        if (tmpText == null)
            tmpText = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(
            tmpText,
            eventData.position,
            eventData.pressEventCamera
        );

        if (linkIndex != -1)
        {
            string url = tmpText.textInfo.linkInfo[linkIndex].GetLinkID();
            Application.OpenURL(url);
        }
    }
}