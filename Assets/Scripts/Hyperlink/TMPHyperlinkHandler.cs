using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;


public class TMPHyperlinkHandler : MonoBehaviour,
    IPointerClickHandler, IPointerMoveHandler, IPointerExitHandler
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI tmpText;

    [Header("Colors")]
    [SerializeField] private Color normalColor = new Color(0.23f, 0.75f, 1f);
    [SerializeField] private Color hoverColor = Color.yellow;

    private int lastLinkIndex = -1;

    private void Awake()
    {
        if (tmpText == null)
            tmpText = GetComponent<TextMeshProUGUI>();

        tmpText.ForceMeshUpdate();
        ResetLinkColor();
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

    public void OnPointerMove(PointerEventData eventData)
    {
        tmpText.ForceMeshUpdate();

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(
            tmpText,
            eventData.position,
            eventData.pressEventCamera
        );

        if (linkIndex != lastLinkIndex)
        {
            ResetLinkColor();

            if (linkIndex != -1)
                SetLinkColor(linkIndex, hoverColor);

            lastLinkIndex = linkIndex;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetLinkColor();
        lastLinkIndex = -1;
    }

    private void ResetLinkColor()
    {
        tmpText.ForceMeshUpdate();

        for (int i = 0; i < tmpText.textInfo.linkCount; i++)
            SetLinkColor(i, normalColor);
    }

    private void SetLinkColor(int linkIndex, Color color)
    {
        TMP_LinkInfo linkInfo = tmpText.textInfo.linkInfo[linkIndex];

        for (int i = 0; i < linkInfo.linkTextLength; i++)
        {
            int charIndex = linkInfo.linkTextfirstCharacterIndex + i;
            var charInfo = tmpText.textInfo.characterInfo[charIndex];

            if (!charInfo.isVisible) continue;

            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Color32[] colors = tmpText.textInfo.meshInfo[meshIndex].colors32;

            colors[vertexIndex + 0] = color;
            colors[vertexIndex + 1] = color;
            colors[vertexIndex + 2] = color;
            colors[vertexIndex + 3] = color;
        }

        tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
}
