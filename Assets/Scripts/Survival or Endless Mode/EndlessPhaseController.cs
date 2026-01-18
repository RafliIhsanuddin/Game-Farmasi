using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndlessPhaseController : MonoBehaviour
{
    [Header("Camera (2D)")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float selectCameraX = 6.5f;
    [SerializeField] private float playCameraX = 0f;
    [SerializeField] private float cameraMoveSpeed = 3f;

    [Header("UI Ref")]
    [SerializeField] private EndlessUIController ui;

    [Header("Wave Manager")]
    [SerializeField] private EndlessWaveManager waveManager;

    [Header("Ready Button Ref (Script)")]
    [SerializeField] private ReadyButton readyButton;

    [Header("Ready Button OBJ (GameObject)")]
    [SerializeField] private GameObject readyButtonObj;

    [Header("Objects Active On Select")]
    [SerializeField] private List<GameObject> selectActive = new List<GameObject>();

    [Header("Objects Active On Play")]
    [SerializeField] private List<GameObject> playActive = new List<GameObject>();

    [Header("Selection & Preview")]
    [SerializeField] private CardSelectionManager cardManager;
    [SerializeField] private SpawnPreviewHelper previewHelper;

    private void Start()
    {
        SnapCamera(selectCameraX);
        ApplySelectObjects();

        ui?.EnterSelectPhase();

        readyButtonObj?.SetActive(true);
        readyButton?.SetReadyEnabled(true);

        previewHelper?.GeneratePreviewWithMinDifference();
    }

    public IEnumerator StartPlayPhase()
    {
        yield return MoveCameraSmooth(playCameraX);

        ApplyPlayObjects();

        readyButtonObj?.SetActive(false);
        readyButton?.SetReadyEnabled(false);

        ui?.OnPlayPhaseStart();

        if (waveManager != null)
            waveManager.StartCoroutine(waveManager.RunSingleCycle());
    }

    public IEnumerator SmoothBackToSelect()
    {
        yield return MoveCameraSmooth(selectCameraX);

        ApplySelectObjects();

        cardManager?.ReEnterSelectPhase();

        previewHelper?.ClearPreview();
        previewHelper?.GeneratePreviewWithMinDifference();

        ui?.EnterSelectPhase();

        readyButtonObj?.SetActive(true);
        readyButton?.SetReadyEnabled(true);
    }

    private void ApplySelectObjects()
    {
        foreach (var o in selectActive)
            if (o) o.SetActive(true);

        foreach (var o in playActive)
            if (o) o.SetActive(false);
    }

    private void ApplyPlayObjects()
    {
        foreach (var o in selectActive)
            if (o) o.SetActive(false);

        foreach (var o in playActive)
            if (o) o.SetActive(true);
    }
    
    private void SnapCamera(float x)
    {
        if (!cameraTransform)
        {
            Debug.LogError("[PHASE] Missing cameraTransform!");
            return;
        }

        cameraTransform.position = new Vector3(
            x,
            cameraTransform.position.y,
            cameraTransform.position.z
        );
    }

    private IEnumerator MoveCameraSmooth(float targetX)
    {
        while (true)
        {
            var p = cameraTransform.position;
            float newX = Mathf.Lerp(p.x, targetX, Time.deltaTime * cameraMoveSpeed);
            cameraTransform.position = new Vector3(newX, p.y, p.z);

            if (Mathf.Abs(newX - targetX) < 0.05f) break;
            yield return null;
        }

        var pp = cameraTransform.position;
        cameraTransform.position = new Vector3(targetX, pp.y, pp.z);
    }
}
