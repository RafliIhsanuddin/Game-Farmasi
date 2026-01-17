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

    [Header("Ready Button Ref")]
    [SerializeField] private ReadyButton readyButton;

    [Header("Objects Active On Select")]
    [SerializeField] private List<GameObject> selectActive = new List<GameObject>();

    [Header("Objects Active On Play")]
    [SerializeField] private List<GameObject> playActive = new List<GameObject>();

    private bool movingCamera = false;

    private void Start()
    {
        Debug.Log("<color=cyan>[EndlessPhase] SELECT Phase Start</color>");

        SnapCamera(selectCameraX);
        ApplySelectPhaseObjects();

        // SELECT awal -> READY ENABLE
        if (readyButton != null)
            readyButton.SetReadyEnabled(true);

        if (ui != null)
            ui.EnterSelectPhase();
    }

    // =========================
    // PLAY PHASE
    // =========================
    public IEnumerator StartPlayPhase()
    {
        Debug.Log("<color=yellow>[EndlessPhase] PLAY Phase Request</color>");

        // APPLY object PLAY dulu (jadi bisa dicek apa yang harus hilang)
        ApplyPlayPhaseObjects();

        // RULE KAMU:
        // Press Ready -> READY tetap ENABLE saat camera MOVE
        // Jadi di sini TIDAK disable.
        Debug.Log("<color=orange>[EndlessPhase] Camera moving SELECT → PLAY (Ready stays ENABLE)</color>");

        yield return MoveCameraSmooth(playCameraX);

        // Setelah sampai PLAY & DIAM -> DISABLE
        if (readyButton != null)
            readyButton.SetReadyEnabled(false);

        Debug.Log("<color=red>[EndlessPhase] Arrived PLAY & STOP → READY DISABLED</color>");
    }

    // =========================
    // BACK TO SELECT
    // =========================
    public IEnumerator SmoothBackToSelect()
    {
        Debug.Log("<color=orange>[EndlessPhase] Camera moving PLAY → SELECT</color>");

        // Saat balik ke select, READY harus DISABLE selama moving & sampai select baru enable
        if (readyButton != null)
            readyButton.SetReadyEnabled(false);

        ApplySelectPhaseObjects();

        yield return MoveCameraSmooth(selectCameraX);

        // Setelah sampai SELECT & DIAM -> ENABLE
        if (readyButton != null)
            readyButton.SetReadyEnabled(true);

        Debug.Log("<color=lime>[EndlessPhase] Arrived SELECT & STOP → READY ENABLED</color>");
    }

    // =========================
    // CAMERA
    // =========================
    private void SnapCamera(float x)
    {
        if (!cameraTransform)
        {
            Debug.LogError("<color=red>[Camera] Missing cameraTransform!</color>");
            return;
        }

        cameraTransform.position = new Vector3(
            x,
            cameraTransform.position.y,
            cameraTransform.position.z
        );

        Debug.Log($"<color=teal>[Camera] Snap To X={x}</color>");
    }

    private IEnumerator MoveCameraSmooth(float targetX)
    {
        if (!cameraTransform)
        {
            Debug.LogError("<color=red>[Camera] Missing cameraTransform!</color>");
            yield break;
        }

        movingCamera = true;
        Debug.Log($"<color=yellow>[Camera] Move START → Target={targetX}</color>");

        while (movingCamera)
        {
            Vector3 p = cameraTransform.position;
            float newX = Mathf.Lerp(p.x, targetX, Time.deltaTime * cameraMoveSpeed);
            cameraTransform.position = new Vector3(newX, p.y, p.z);

            Debug.Log($"<color=grey>[Camera] posX={newX:F3} → target={targetX}</color>");

            if (Mathf.Abs(newX - targetX) < 0.05f)
            {
                movingCamera = false;
                Debug.Log("<color=green>[Camera] MOVE FINISHED</color>");
            }

            yield return null;
        }
    }

    // =========================
    // OBJECT GROUPS + DEBUG
    // =========================
    private void ApplySelectPhaseObjects()
    {
        Debug.Log("<color=cyan>[Phase] APPLY SELECT OBJECTS</color>");

        foreach (var o in selectActive)
        {
            if (o == null) { Debug.LogWarning("[SelectActive] NULL"); continue; }
            o.SetActive(true);
            Debug.Log($"[SelectActive] ON  → {o.name} (activeSelf={o.activeSelf})");
        }

        foreach (var o in playActive)
        {
            if (o == null) { Debug.LogWarning("[PlayActive] NULL"); continue; }
            o.SetActive(false);
            Debug.Log($"[PlayActive] OFF → {o.name} (activeSelf={o.activeSelf})");
        }
    }

    private void ApplyPlayPhaseObjects()
    {
        Debug.Log("<color=cyan>[Phase] APPLY PLAY OBJECTS</color>");

        foreach (var o in selectActive)
        {
            if (o == null) { Debug.LogWarning("[SelectActive] NULL"); continue; }
            o.SetActive(false);
            Debug.Log($"[SelectActive] OFF → {o.name} (activeSelf={o.activeSelf})");
        }

        foreach (var o in playActive)
        {
            if (o == null) { Debug.LogWarning("[PlayActive] NULL"); continue; }
            o.SetActive(true);
            Debug.Log($"[PlayActive] ON  → {o.name} (activeSelf={o.activeSelf})");
        }
    }
}
