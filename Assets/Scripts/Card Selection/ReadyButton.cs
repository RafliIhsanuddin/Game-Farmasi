using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ReadyButton : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject selectorHUD;
    [SerializeField] private Button readyButton;

    [Header("Gameplay")]
    [SerializeField] private CardSelectionManager cardManager;
    [SerializeField] private GameObject atomSpawner;
    [SerializeField] private GameObject waveManager;
    [SerializeField] private GameObject bacteriaSpawner;

    [Header("Camera Move Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float cameraTargetX = 0f;
    [SerializeField] private float cameraMoveSpeed = 2f;

    private bool movingCamera = false;

    private void Awake()
    {
        if (readyButton != null)
            readyButton.onClick.AddListener(OnReady);

        if (atomSpawner) atomSpawner.SetActive(false);
        if (waveManager) waveManager.SetActive(false);
        if (bacteriaSpawner) bacteriaSpawner.SetActive(false);
    }

    private void Update()
    {
        if (movingCamera && cameraTransform != null)
        {
            Vector3 pos = cameraTransform.position;
            float newX = Mathf.Lerp(pos.x, cameraTargetX, Time.deltaTime * cameraMoveSpeed);
            cameraTransform.position = new Vector3(newX, pos.y, pos.z);

            if (Mathf.Abs(newX - cameraTargetX) < 0.05f)
                movingCamera = false;
        }
    }

    private void OnReady()
    {
        int selected = cardManager.GetSelectedCount();

        if (selected < 3)
        {
            SoundManager.Instance?.PlayWrong();
            return;
        }

        cardManager.ConfirmSelection();

        if (selectorHUD) selectorHUD.SetActive(false);
        if (readyButton) readyButton.interactable = false;

        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        movingCamera = true;
        while (movingCamera)
            yield return null;

        // ambil pool dari preview helper
        SpawnPreviewHelper helper = Object.FindFirstObjectByType<SpawnPreviewHelper>();
        if (helper != null)
        {
            var pool = helper.GetSelectedEnemyPool();

            var spawner = bacteriaSpawner.GetComponent<BacteriaSpawner>();
            if (spawner != null && pool.Count > 0)
            {
                spawner.SetEnemyPool(pool);
            }

            helper.ClearPreview();
        }

        if (atomSpawner) atomSpawner.SetActive(true);
        if (waveManager) waveManager.SetActive(true);
        if (bacteriaSpawner) bacteriaSpawner.SetActive(true);

        if (readyButton)
            readyButton.gameObject.SetActive(false);
    }
}
