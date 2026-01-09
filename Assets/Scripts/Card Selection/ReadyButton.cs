using UnityEngine;
using UnityEngine.UI;

public class ReadyButton : MonoBehaviour
{
    [SerializeField] private CardSelectionManager cardManager;

    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnReady);
    }

    void OnReady()
    {
        cardManager.ConfirmSelection();
        btn.interactable = false;
    }
}
