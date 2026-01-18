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
    [SerializeField] private EndlessWaveManager endlessWaveManager;   // tidak dipakai langsung, tapi biarkan saja
    [SerializeField] private EndlessUIController ui;
    [SerializeField] private SpawnPreviewHelper preview;
    [SerializeField] private BacteriaSpawner spawner;

    private bool selectionLocked = false;

    private void Awake()
    {
        if (readyButton == null)
            readyButton = GetComponent<Button>();

        readyButton.onClick.AddListener(() => StartCoroutine(OnReadyPressed()));
    }

    private IEnumerator OnReadyPressed()
    {
        Debug.Log("<color=orange>[READY] Player pressed Ready</color>");

        // === SELECTION CHECK ===
        if (!selectionLocked)
        {
            int selected = cardManager != null ? cardManager.GetSelectedCount() : 0;

            if (selected < minRequiredSelection)
            {
                Debug.Log($"<color=red>[READY] Not enough cards selected ({selected}/{minRequiredSelection})</color>");
                SoundManager.Instance?.PlayWrong();
                yield break;
            }

            if (cardManager != null)
                cardManager.ConfirmSelection();

            selectionLocked = true;
            Debug.Log("<color=green>[READY] Selection Locked</color>");
        }

        // === CLOSE SELECTOR HUD ===
        if (selectorHUD != null)
            selectorHUD.SetActive(false);

        // === PREVIEW → SPAWNER ASSIGN ===
        if (preview != null)
        {
            List<GameObject> pool = preview.GetSelectedEnemyPool();
            if (pool != null && pool.Count > 0 && spawner != null)
            {
                spawner.SetEnemyPool(pool);
                Debug.Log($"<color=cyan>[READY] Enemy pool assigned to spawner (count={pool.Count})</color>");
            }
            else
            {
                Debug.Log("<color=yellow>[READY] Preview pool empty or spawner missing</color>");
            }
        }

        // === ENTER PLAY PHASE ===
        // PhaseController akan:
        // - Pindahkan kamera SELECT → PLAY
        // - Switch Select/Play objects
        // - Nanti WaveManager.RunSingleCycle() dipanggil dari PhaseController
        if (phaseController != null)
        {
            yield return phaseController.StartPlayPhase();
            Debug.Log("<color=lime>[READY] Handoff to PhaseController completed</color>");
        }
        else
        {
            Debug.LogError("<color=red>[READY] PhaseController is NULL!</color>");
        }
    }

    public void SetReadyEnabled(bool state)
    {
        if (readyButton != null)
        {
            readyButton.interactable = state;
            Debug.Log($"<color=yellow>[READY] Button interactable = {state}</color>");
        }
    }
}
