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
    [SerializeField] private EndlessUIController ui;

    [Header("Cinematic Settings")]
    [SerializeField] private float postCycleDelay = 3f;

    private bool selectionLocked = false;
    private bool isBusy = false; // anti double-press

    private void Awake()
    {
        if (readyButton == null)
            readyButton = GetComponent<Button>();

        if (readyButton != null)
            readyButton.onClick.AddListener(() => StartCoroutine(OnReadyPressed()));
    }

    // NOTE: sesuai aturan kamu, ini cuma ngubah interactable (bukan hide/show)
    public void SetReadyEnabled(bool state)
    {
        if (readyButton != null)
        {
            readyButton.interactable = state;
            Debug.Log($"<color=orange>[ReadyButton] INTERACTABLE → {(state ? "ENABLED" : "DISABLED")}</color>");
        }
    }

    private IEnumerator OnReadyPressed()
    {
        if (isBusy)
        {
            Debug.LogWarning("[ReadyButton] Ignored click because isBusy=true");
            yield break;
        }

        isBusy = true;

        // ------ 1x selection lock check ------
        if (!selectionLocked)
        {
            int selected = cardManager != null ? cardManager.GetSelectedCount() : 0;

            if (selected < minRequiredSelection)
            {
                SoundManager.Instance?.PlayWrong();
                isBusy = false;
                yield break;
            }

            if (cardManager != null)
                cardManager.ConfirmSelection(); // mekanik lama tidak diubah

            selectionLocked = true;
            Debug.Log($"<color=cyan>[ReadyButton] Selection LOCKED → {selected} cards</color>");
        }

        Debug.Log("<color=yellow>[ReadyButton] PRESSED</color>");

        // hide selector HUD seperti biasa
        selectorHUD?.SetActive(false);

        // =========================
        // RULE KAMU:
        // Press Ready -> READY tetap ENABLE saat camera MOVE
        // Jadi di sini kita TIDAK disable tombol.
        // =========================

        // ====== ENTER PLAY (camera move) ======
        Debug.Log("<color=yellow>[ReadyButton] StartPlayPhase() (Ready stays ENABLE while moving)</color>");
        if (phaseController != null)
            yield return phaseController.StartPlayPhase(); // phase controller yang akan DISABLE setelah arrive PLAY

        // UI: Cycle naik visual saat benar-benar mulai PLAY
        if (ui != null)
            ui.OnPlayPhaseStart();

        // ====== RUN CYCLE (2 WAVE) ======
        if (endlessWaveManager != null)
            yield return endlessWaveManager.RunSingleCycle();

        // ====== CINEMATIC DELAY ======
        if (postCycleDelay > 0)
        {
            Debug.Log("<color=cyan>[ReadyButton] Cinematic Delay</color>");
            yield return new WaitForSeconds(postCycleDelay);
        }

        // ====== BACK TO SELECT ======
        Debug.Log("<color=magenta>[ReadyButton] Back To SELECT</color>");
        if (phaseController != null)
            yield return phaseController.SmoothBackToSelect(); // phase controller ENABLE setelah arrive SELECT

        selectorHUD?.SetActive(true);

        if (ui != null)
            ui.EnterSelectPhase();

        isBusy = false;
    }
}
