using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndlessPhaseController : MonoBehaviour
{
    [Header("Group — Active Saat SELECT Phase")]
    [SerializeField] private List<GameObject> activeOnSelect = new();

    [Header("Group — Non-Active Saat SELECT Phase")]
    [SerializeField] private List<GameObject> inactiveOnSelect = new();

    [Header("Group — Active Saat PLAY Phase")]
    [SerializeField] private List<GameObject> activeOnPlay = new();

    [Header("Group — Non-Active Saat PLAY Phase")]
    [SerializeField] private List<GameObject> inactiveOnPlay = new();

    [Header("Camera Settings (2D)")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float selectCameraX = 6.5f;  // kanan (seed select)
    [SerializeField] private float playCameraX = 0f;      // kiri (board play)
    [SerializeField] private float cameraMoveSpeed = 3f;

    [Header("Gameplay Systems")]
    [SerializeField] private GameObject waveManager;
    [SerializeField] private GameObject bacteriaSpawner;
    [SerializeField] private GameObject atomSpawner;

    private bool movingCamera = false;

    private void Start()
    {
        EnterSelectPhase(); // default survival behavior
    }

    // ===========================
    // SELECT PHASE (SNAP)
    // ===========================
    public void EnterSelectPhase()
    {
        SetList(activeOnSelect, true);
        SetList(inactiveOnSelect, false);

        if (waveManager) waveManager.SetActive(false);
        if (bacteriaSpawner) bacteriaSpawner.SetActive(false);
        if (atomSpawner) atomSpawner.SetActive(false);

        SnapCamera(selectCameraX);

        Debug.Log("[EndlessPhase] SELECT PHASE");
    }

    // ===========================
    // PLAY PHASE (dipanggil Ready)
    // ===========================
    public void StartPlayPhase()
    {
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        SetList(activeOnPlay, true);
        SetList(inactiveOnPlay, false);

        // smooth ke kiri (board)
        yield return MoveCameraSmooth(playCameraX);

        if (atomSpawner) atomSpawner.SetActive(true);
        if (bacteriaSpawner) bacteriaSpawner.SetActive(true);
        if (waveManager) waveManager.SetActive(true);

        Debug.Log("[EndlessPhase] PLAY PHASE");
    }

    // ===========================
    // BALIK KE SELECT (SMOOTH)
    // ===========================
    public IEnumerator SmoothBackToSelect()
    {
        // matikan gameplay systems dulu
        if (waveManager) waveManager.SetActive(false);
        if (bacteriaSpawner) bacteriaSpawner.SetActive(false);
        if (atomSpawner) atomSpawner.SetActive(false);

        // UI balik ke SELECT
        SetList(activeOnSelect, true);
        SetList(inactiveOnSelect, false);

        // camera smooth ke kanan
        yield return MoveCameraSmooth(selectCameraX);

        Debug.Log("[EndlessPhase] BACK TO SELECT");
    }

    // ===========================
    // CAMERA LOGIC
    // ===========================
    private void SnapCamera(float x)
    {
        if (!cameraTransform) return;

        Vector3 p = cameraTransform.position;
        cameraTransform.position = new Vector3(x, p.y, p.z);
    }

    private IEnumerator MoveCameraSmooth(float targetX)
    {
        if (!cameraTransform)
        {
            Debug.LogError("[EndlessPhase] cameraTransform NULL!");
            yield break;
        }

        movingCamera = true;

        while (movingCamera)
        {
            Vector3 p = cameraTransform.position;
            float newX = Mathf.Lerp(p.x, targetX, Time.deltaTime * cameraMoveSpeed);
            cameraTransform.position = new Vector3(newX, p.y, p.z);

            if (Mathf.Abs(newX - targetX) < 0.05f)
                movingCamera = false;

            yield return null;
        }
    }

    // ===========================
    // HELPERS
    // ===========================
    private void SetList(List<GameObject> list, bool state)
    {
        foreach (var go in list)
            if (go) go.SetActive(state);
    }
}
