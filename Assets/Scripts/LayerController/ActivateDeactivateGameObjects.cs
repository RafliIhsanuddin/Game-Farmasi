using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ActivateDeactivateGameObjects : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private Button activateButton;
    [SerializeField] private Button deactivateButton;

    [Header("Target Objects (boleh kosong)")]
    [SerializeField] private List<GameObject> targetObjects = new List<GameObject>();


    private void Awake()
    {
        if (activateButton != null)
            activateButton.onClick.AddListener(ActivateObjects);

        if (deactivateButton != null)
            deactivateButton.onClick.AddListener(DeactivateObjects);
    }


    private void ActivateObjects()
    {
        // Sunnah kosong → langsung return tanpa error / warning
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
        // Sunnah kosong → langsung return tanpa error / warning
        if (targetObjects == null || targetObjects.Count == 0)
            return;

        foreach (GameObject obj in targetObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}