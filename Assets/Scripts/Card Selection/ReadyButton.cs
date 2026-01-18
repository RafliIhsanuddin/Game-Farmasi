using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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
    [SerializeField] private SpawnPreviewHelper preview;
    [SerializeField] private BacteriaSpawner spawner;

    [Header("Cinematic Settings")]
    [SerializeField] private float postCycleDelay = 3f;

    private bool selectionLocked = false;

    private void Awake()
    {
        if (readyButton == null)
            readyButton = GetComponent<Button>();

        readyButton.onClick.AddListener(() => StartCoroutine(OnReadyPressed()));
    }

    private IEnumerator OnReadyPressed()
    {
        // SELECTION CHECK
        if (!selectionLocked)
        {
            int selected = cardManager.GetSelectedCount();

            if (selected < minRequiredSelection)
            {
                SoundManager.Instance?.PlayWrong();
                yield break;
            }

            cardManager.ConfirmSelection();
            selectionLocked = true;
        }

        selectorHUD?.SetActive(false);

        // --------- PREVIEW → SPAWNER ----------
        if (preview != null)
        {
            List<GameObject> pool = preview.GetSelectedEnemyPool();
            if (pool != null && pool.Count > 0 && spawner != null)
            {
                spawner.SetEnemyPool(pool);
            }
        }

        // -------- ENTER PLAY PHASE --------
        yield return phaseController.StartPlayPhase();

        if (ui != null)
            ui.OnPlayPhaseStart();   // "Cycle X"

        // -------- RUN CYCLE (2 waves) --------
        yield return endlessWaveManager.RunSingleCycle();

        // -------- PVZ DELAY STYLE --------
        if (postCycleDelay > 0)
            yield return new WaitForSeconds(postCycleDelay);

        // -------- BACK TO SELECT --------
        yield return phaseController.SmoothBackToSelect();

        selectorHUD?.SetActive(true);

        // PREVIEW REFRESH (for next cycle)
        if (preview != null)
            preview.GeneratePreviewWithMinDifference();

        if (ui != null)
            ui.EnterSelectPhase();   // "Click Ready to enter Cycle X"
    }

    public void SetReadyEnabled(bool state)
    {
        readyButton.interactable = state;
    }
}
