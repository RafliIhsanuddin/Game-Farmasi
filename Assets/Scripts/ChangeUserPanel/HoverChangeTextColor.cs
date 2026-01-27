using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class HoverChangeTextColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Target Text")]
        [SerializeField] private TextMeshProUGUI targetText;
    
        [Header("Colors")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hoverColor = Color.yellow;
    
        private void Reset()
        {
            // otomatis cari TMP child
            if (targetText == null)
                targetText = GetComponentInChildren<TextMeshProUGUI>();
        }
    
        private void Start()
        {
            if (targetText != null)
                targetText.color = normalColor;
        }
    
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (targetText != null)
                targetText.color = hoverColor;
        }
    
        public void OnPointerExit(PointerEventData eventData)
        {
            if (targetText != null)
                targetText.color = normalColor;
        }
}
