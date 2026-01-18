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

    private void Start()
    {
        Debug.Log("<color=cyan>[PHASE] Scene START → SELECT phase</color>");

        SnapCamera(selectCameraX);
        ApplySelectObjects();

        if (ui != null)
            ui.EnterSelectPhase(); // "Click Ready to enter Cycle 1"

        // === SELECT AWAL: READY ENABLE ===
        if (readyButtonObj != null)
        {
            readyButtonObj.SetActive(true);
            Debug.Log("<color=green>[PHASE] READY → ENABLE (SELECT AWAL)</color>");
        }

        if (readyButton != null)
            readyButton.SetReadyEnabled(true);
    }

    // Dipanggil dari ReadyButton
    public IEnumerator StartPlayPhase()
    {
        Debug.Log("<color=orange>[PHASE] StartPlayPhase called</color>");

        // === Press Ready: READY tetap ENABLE (sesuai tabel) ===
        if (readyButtonObj != null)
            Debug.Log("<color=green>[PHASE] READY tetap ENABLE (Press Ready)</color>");

        Debug.Log("<color=grey>[PHASE] Camera SELECT → PLAY moving...</color>");
        yield return MoveCameraSmooth(playCameraX);

        Debug.Log("<color=lime>[PHASE] Camera arrived at PLAY</color>");

        // === Arrive PLAY: READY DISABLE ===
        if (readyButtonObj != null)
        {
            readyButtonObj.SetActive(false);
            Debug.Log("<color=red>[PHASE] READY → DISABLE (Arrive PLAY)</color>");
        }

        ApplyPlayObjects();

        // Run wave cycle (READY tetap DISABLE)
        if (ui != null)
            ui.OnPlayPhaseStart();

        if (waveManager != null)
        {
            Debug.Log("<color=magenta>[PHASE] RunSingleCycle()</color>");
            waveManager.StartCoroutine(waveManager.RunSingleCycle());
        }
        else
        {
            Debug.LogError("<color=red>[PHASE] WaveManager is NULL!</color>");
        }
    }

    // Dipanggil dari EndlessWaveManager
    public IEnumerator SmoothBackToSelect()
    {
        Debug.Log("<color=grey>[PHASE] Camera PLAY → SELECT moving...</color>");

        // === Back to SELECT (move): READY tetap DISABLE ===
        if (readyButtonObj != null)
            Debug.Log("<color=red>[PHASE] READY tetap DISABLE (Back SELECT move)</color>");

        yield return MoveCameraSmooth(selectCameraX);

        Debug.Log("<color=lime>[PHASE] Camera arrived at SELECT</color>");

        ApplySelectObjects();

        // === Arrive SELECT: READY ENABLE ===
        if (readyButtonObj != null)
        {
            readyButtonObj.SetActive(true);
            Debug.Log("<color=green>[PHASE] READY → ENABLE (Arrive SELECT)</color>");
        }

        if (ui != null)
            ui.EnterSelectPhase();

        if (readyButton != null)
            readyButton.SetReadyEnabled(true);
    }

    private void ApplySelectObjects()
    {
        foreach (var o in selectActive)
        {
            if (!o) continue;
            o.SetActive(true);
        }
        foreach (var o in playActive)
        {
            if (!o) continue;
            o.SetActive(false);
        }
    }

    private void ApplyPlayObjects()
    {
        foreach (var o in selectActive)
        {
            if (!o) continue;
            o.SetActive(false);
        }
        foreach (var o in playActive)
        {
            if (!o) continue;
            o.SetActive(true);
        }
    }

    private void SnapCamera(float x)
    {
        if (!cameraTransform) return;

        cameraTransform.position = new Vector3(
            x,
            cameraTransform.position.y,
            cameraTransform.position.z
        );
    }

    private IEnumerator MoveCameraSmooth(float targetX)
    {
        if (!cameraTransform) yield break;

        while (true)
        {
            Vector3 p = cameraTransform.position;
            float newX = Mathf.Lerp(p.x, targetX, Time.deltaTime * cameraMoveSpeed);
            cameraTransform.position = new Vector3(newX, p.y, p.z);

            if (Mathf.Abs(newX - targetX) < 0.05f)
                break;

            yield return null;
        }

        cameraTransform.position = new Vector3(
            targetX,
            cameraTransform.position.y,
            cameraTransform.position.z
        );
    }
}
