using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ReadyButton : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject selectorHUD;
    [SerializeField] private Button readyButton;

    [Header("Selection")]
    [SerializeField] private CardSelectionManager cardManager;
    [SerializeField] private int minRequiredSelection = 3;

    [Header("Endless Hooks")]
    [SerializeField] private EndlessPhaseController phaseController;
    [SerializeField] private EndlessWaveManager endlessWaveManager;
    [SerializeField] private EndlessUIController uiController;

    [Header("Cinematic Settings")]
    [SerializeField] private float postCycleDelay = 3f;

    private bool selectionLocked = false;

    private void Awake()
    {
        if (readyButton == null)
            readyButton = GetComponent<Button>();

        if (readyButton != null)
            readyButton.onClick.AddListener(() => StartCoroutine(OnReadyPressed()));
    }

    private IEnumerator OnReadyPressed()
    {
        // batas minimal kartu (PVZ survival rules)
        if (!selectionLocked)
        {
            int selected = cardManager != null ? cardManager.GetSelectedCount() : 0;

            if (selected < minRequiredSelection)
            {
                SoundManager.Instance?.PlayWrong();
                yield break;
            }

            if (cardManager != null)
                cardManager.ConfirmSelection();

            selectionLocked = true;
        }

        Debug.Log("<color=orange>[ReadyButton] PRESSED</color>");

        // hide select UI
        if (selectorHUD)
            selectorHUD.SetActive(false);

        if (readyButton)
            readyButton.interactable = false;

        // 🚨 Cycle bertambah saat tombol READY ditekan
        if (uiController != null)
        {
            uiController.IncrementCycleVisual();
            Debug.Log($"<color=lime>[ReadyButton] Cycle Masuk → {uiController.CurrentCycle}</color>");
        }

        // pindah dunia → PLAY phase
        if (phaseController != null)
            phaseController.StartPlayPhase();

        // jalankan 1 cycle
        if (endlessWaveManager != null)
            yield return endlessWaveManager.RunSingleCycle();

        // delay cinematic PVZ
        if (postCycleDelay > 0f)
            yield return new WaitForSeconds(postCycleDelay);

        // balik ke SELECT
        if (phaseController != null)
            yield return phaseController.SmoothBackToSelect();

        // tampilkan select HUD lagi
        if (selectorHUD)
            selectorHUD.SetActive(true);

        if (readyButton)
        {
            readyButton.interactable = true;
            readyButton.gameObject.SetActive(true);
        }
    }
}
