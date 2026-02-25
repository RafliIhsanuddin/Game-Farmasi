using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ActivateDeactivateGameObjects : MonoBehaviour
{
    [Header("Activate Buttons (boleh banyak)")]
    [SerializeField] private List<Button> activateButtons = new List<Button>();

    [Header("Deactivate Buttons (boleh banyak)")]
    [SerializeField] private List<Button> deactivateButtons = new List<Button>();

    [Header("Target Objects (1 list saja, boleh kosong / boleh banyak)")]
    [SerializeField] private List<GameObject> targetObjects = new List<GameObject>();

    [Header("Deactivate Blockers (Sunnah, boleh kosong / boleh banyak)")]
    [SerializeField] private List<GameObject> deactivateBlockers = new List<GameObject>();


    private void Awake()
    {
        RegisterActivateButtons();
        RegisterDeactivateButtons();
    }


    private void RegisterActivateButtons()
    {
        if (activateButtons == null || activateButtons.Count == 0)
            return;

        foreach (Button btn in activateButtons)
        {
            if (btn != null)
                btn.onClick.AddListener(ActivateObjects);
        }
    }


    private void RegisterDeactivateButtons()
    {
        if (deactivateButtons == null || deactivateButtons.Count == 0)
            return;

        foreach (Button btn in deactivateButtons)
        {
            if (btn != null)
                btn.onClick.AddListener(DeactivateObjects);
        }
    }


    private void ActivateObjects()
    {
        if (targetObjects == null || targetObjects.Count == 0)
            return;

        foreach (GameObject obj in targetObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }


    private void DeactivateObjects()
    {
        // 🔹 Sunnah: cek blocker, jika ada yang aktif maka abort deactivate
        if (IsDeactivateBlocked())
            return;

        if (targetObjects == null || targetObjects.Count == 0)
            return;

        foreach (GameObject obj in targetObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }


    // 🔹 Sunnah helper function untuk cek blocker
    private bool IsDeactivateBlocked()
    {
        if (deactivateBlockers == null || deactivateBlockers.Count == 0)
            return false;

        foreach (GameObject blocker in deactivateBlockers)
        {
            if (blocker != null && blocker.activeSelf)
                return true;
        }

        return false;
    }
}