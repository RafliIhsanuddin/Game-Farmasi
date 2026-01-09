using UnityEngine;
using UnityEngine.UI;

public class ReadyButton : MonoBehaviour
{
    [SerializeField] private CardSelectionManager cardManager;

    [SerializeField] private GameObject selectorHUD;
    [SerializeField] private GameObject readyButton;

    [SerializeField] private GameObject atomSpawner;
    [SerializeField] private GameObject waveManager;
    [SerializeField] private GameObject bacteriaSpawner;

    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnReady);

        if (atomSpawner) atomSpawner.SetActive(false);
        if (waveManager) waveManager.SetActive(false);
        if (bacteriaSpawner) bacteriaSpawner.SetActive(false);
    }

    void OnReady()
    {
        cardManager.ConfirmSelection();

        if (selectorHUD) selectorHUD.SetActive(false);
        if (readyButton) readyButton.SetActive(false);

        if (atomSpawner) atomSpawner.SetActive(true);
        if (waveManager) waveManager.SetActive(true);
        if (bacteriaSpawner) bacteriaSpawner.SetActive(true);
    }
}
