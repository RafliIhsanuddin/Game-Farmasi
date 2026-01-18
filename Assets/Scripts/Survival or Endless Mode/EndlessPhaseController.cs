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

    [Header("Ready Button Ref")]
    [SerializeField] private ReadyButton readyButton;

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

        if (readyButton != null)
            readyButton.SetReadyEnabled(true);
    }

    // Dipanggil dari ReadyButton
    public IEnumerator StartPlayPhase()
    {
        Debug.Log("<color=orange>[PHASE] StartPlayPhase called</color>");

        if (readyButton != null)
            readyButton.SetReadyEnabled(false);

        Debug.Log("<color=grey>[PHASE] Moving camera SELECT → PLAY...</color>");
        yield return MoveCameraSmooth(playCameraX);
        Debug.Log("<color=lime>[PHASE] Camera arrived at PLAY</color>");

        ApplyPlayObjects();

        // Info saja, text "Cycle X" AKAN di-set oleh ui.SetupNewCycle() di dalam RunSingleCycle()
        if (ui != null)
            ui.OnPlayPhaseStart(); // tidak mengubah text, cuma log status

        if (waveManager != null)
        {
            Debug.Log("<color=magenta>[PHASE] Starting RunSingleCycle coroutine</color>");
            waveManager.StartCoroutine(waveManager.RunSingleCycle());
        }
        else
        {
            Debug.LogError("<color=red>[PHASE] WaveManager is NULL!</color>");
        }
    }

    // Dipanggil dari EndlessWaveManager setelah Wave1+Wave2 selesai
    public IEnumerator SmoothBackToSelect()
    {
        Debug.Log("<color=grey>[PHASE] Moving camera PLAY → SELECT...</color>");

        yield return MoveCameraSmooth(selectCameraX);

        Debug.Log("<color=lime>[PHASE] Camera arrived at SELECT</color>");

        ApplySelectObjects();

        if (ui != null)
            ui.EnterSelectPhase(); // "Click Ready to enter Cycle (X+1)"

        if (readyButton != null)
            readyButton.SetReadyEnabled(true);
    }

    private void ApplySelectObjects()
    {
        foreach (var o in selectActive)
        {
            if (!o) continue;
            o.SetActive(true);
            Debug.Log($"<color=cyan>[PHASE] SELECT ON → {o.name}</color>");
        }

        foreach (var o in playActive)
        {
            if (!o) continue;
            o.SetActive(false);
            Debug.Log($"<color=cyan>[PHASE] PLAY OFF → {o.name}</color>");
        }
    }

    private void ApplyPlayObjects()
    {
        foreach (var o in selectActive)
        {
            if (!o) continue;
            o.SetActive(false);
            Debug.Log($"<color=yellow>[PHASE] SELECT OFF → {o.name}</color>");
        }

        foreach (var o in playActive)
        {
            if (!o) continue;
            o.SetActive(true);
            Debug.Log($"<color=yellow>[PHASE] PLAY ON → {o.name}</color>");
        }
    }

    private void SnapCamera(float x)
    {
        if (!cameraTransform)
        {
            Debug.LogError("<color=red>[PHASE] Missing cameraTransform!</color>");
            return;
        }

        cameraTransform.position = new Vector3(
            x,
            cameraTransform.position.y,
            cameraTransform.position.z
        );

        Debug.Log($"<color=teal>[PHASE] Camera SNAP to X={x}</color>");
    }

    private IEnumerator MoveCameraSmooth(float targetX)
    {
        if (!cameraTransform)
        {
            Debug.LogError("<color=red>[PHASE] Missing cameraTransform!</color>");
            yield break;
        }

        while (true)
        {
            Vector3 p = cameraTransform.position;
            float newX = Mathf.Lerp(p.x, targetX, Time.deltaTime * cameraMoveSpeed);
            cameraTransform.position = new Vector3(newX, p.y, p.z);

            if (Mathf.Abs(newX - targetX) < 0.05f)
                break;

            yield return null;
        }

        // snap akhir
        cameraTransform.position = new Vector3(
            targetX,
            cameraTransform.position.y,
            cameraTransform.position.z
        );
    }
}
