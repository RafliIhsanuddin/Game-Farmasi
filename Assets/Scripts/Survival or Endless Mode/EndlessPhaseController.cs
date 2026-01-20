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

    [Header("Spawner Endless")]
    [SerializeField] private BacteriaSpawnerEndless spawner;
    
    [Header("UI Ref")]
    [SerializeField] private EndlessUIController ui;

    [Header("Wave Manager")]
    [SerializeField] private EndlessWaveManager waveManager;

    [Header("Ready Button Ref (Script)")]
    [SerializeField] private ReadyButton readyButton;

    [Header("Ready Button OBJ (GameObject)")]
    [SerializeField] private GameObject readyButtonObj;

    [Header("Selection Manager")]
    [SerializeField] private CardSelectionManager cardManager;

    [Header("Enemy Preview Helper")]
    [SerializeField] private SpawnPreviewHelper preview;

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
            ui.EnterSelectPhase();

        if (cardManager != null)
            cardManager.ReEnterSelectPhase();

        if (readyButtonObj != null)
        {
            readyButtonObj.SetActive(true);
            Debug.Log("<color=green>[PHASE] READY → ENABLE (SELECT AWAL)</color>");
        }

        if (readyButton != null)
            readyButton.SetReadyEnabled(true);
    }

    public IEnumerator StartPlayPhase()
    {
        Debug.Log("<color=orange>[PHASE] StartPlayPhase called</color>");

        Debug.Log("<color=grey>[PHASE] Camera SELECT → PLAY moving...</color>");
        yield return MoveCameraSmooth(playCameraX);

        Debug.Log("<color=lime>[PHASE] Camera arrived at PLAY</color>");

        ApplyPlayObjects();

        if (readyButtonObj != null)
        {
            readyButtonObj.SetActive(false);
            Debug.Log("<color=red>[PHASE] READY → DISABLE (Arrive PLAY)</color>");
        }

        if (ui != null)
            ui.OnPlayPhaseStart();

        if (preview != null && waveManager != null)
        {
            var pool = preview.GetSelectedEnemyPool();
            spawner.SetEnemyPool(pool);
            Debug.Log("<color=yellow>[PHASE] Enemy pool injected to WaveManager</color>");
        }

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

    public IEnumerator SmoothBackToSelect()
    {
        Debug.Log("<color=grey>[PHASE] Camera PLAY → SELECT moving...</color>");

        if (readyButtonObj != null)
            Debug.Log("<color=red>[PHASE] READY tetap DISABLE (Back SELECT move)</color>");

        yield return MoveCameraSmooth(selectCameraX);

        Debug.Log("<color=lime>[PHASE] Camera arrived at SELECT</color>");

        ApplySelectObjects();

        ClearAllTiles();   // original

        ClearAllAtoms();   // 🔴 ADD HERE

        if (cardManager != null)
            cardManager.ReEnterSelectPhase();

        if (preview != null)
            preview.GeneratePreviewWithMinDifference();

        if (readyButtonObj != null)
        {
            readyButtonObj.SetActive(true);
            Debug.Log("<color=green>[PHASE] READY → ENABLE (Arrive SELECT)</color>");
        }

        if (readyButton != null)
            readyButton.SetReadyEnabled(true);

        if (ui != null)
            ui.EnterSelectPhase();
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

        cameraTransform.position = new Vector3(
            targetX,
            cameraTransform.position.y,
            cameraTransform.position.z
        );
    }

    private void ClearAllTiles()
    {
        Tile[] tiles = Object.FindObjectsByType<Tile>(FindObjectsSortMode.None);

        foreach (var t in tiles)
        {
            if (t.currentCapsule != null)
            {
                Destroy(t.currentCapsule);
            }

            t.ClearCapsule();
        }

        Debug.Log("<color=magenta>[PHASE] Semua tile CLEARED untuk fase SELECT</color>");
    }

    // ====================================================
    // 🔴🔴🔴  TAMBAHAN SESUAI REQUEST: CLEAR ATOM SUN  🔴🔴🔴
    // ====================================================
    private void ClearAllAtoms()
    {
        Atom[] atoms = Object.FindObjectsByType<Atom>(FindObjectsSortMode.None);

        foreach (var a in atoms)
            Destroy(a.gameObject);

        Debug.Log("<color=magenta>[PHASE] Semua ATOM (komponen Atom) dihapus (SELECT)</color>");
    }
}
