using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lose : MonoBehaviour
{
    [Header("Animator Settings")]
    [SerializeField] private Animator animator;

    [Header("GameObjects To Activate After Delay")]
    [SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();

    [Header("Delay Settings")]
    [SerializeField] private float activationDelay = 2f;

    [Header("Enemy Layer Mask (Dropdown)")]
    [SerializeField] private LayerMask enemyLayerMask;

    // 🔹 NEW: Delay sebelum game pause
    [Header("Pause Delay After Trigger (Seconds)")]
    [SerializeField] private float pauseDelay = 7f;

    private bool hasTriggered = false;

    private void Start()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool isEnemy = (enemyLayerMask.value & (1 << other.gameObject.layer)) != 0;

        if (!hasTriggered && isEnemy)
        {
            hasTriggered = true;
            WaveManager.isGameOver = true; // 🔹 tandai game over (kalah)
            Debug.Log("[Lose] Triggered by: " + other.gameObject.name);

            if (animator != null)
            {
                animator.Play("DeathAnimation");
                Debug.Log("[Lose] DeathAnimation played");
            }

            StartCoroutine(ActivateObjectsAfterDelay());
            StartCoroutine(PauseAfterDelay()); // 🔹 NEW
        }
    }

    private IEnumerator ActivateObjectsAfterDelay()
    {
        Debug.Log("[Lose] Waiting for " + activationDelay + " seconds before activating objects...");
        yield return new WaitForSeconds(activationDelay);

        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                Debug.Log("[Lose] Activated object: " + obj.name);
            }
        }
    }

    // 🔹 NEW: Pause setelah delay
    private IEnumerator PauseAfterDelay()
    {
        Debug.Log("[Lose] Waiting " + pauseDelay + " seconds before PAUSE...");
        yield return new WaitForSeconds(pauseDelay);

        // === SUNNAH FINAL COUNT ===
        var counter = Object.FindFirstObjectByType<ListCounterManager>();
        if (counter != null)
        {
            counter.ForceFinalCount();
            Debug.Log("[Lose] Final ListCounterManager count applied before pause.");
        }
        else
        {
            Debug.Log("[Lose] ListCounterManager not found in scene (sunnah only). Skipped final count.");
        }

        // Freeze
        Time.timeScale = 0f;
        Debug.Log("[Lose] GAME PAUSED (Time.timeScale = 0)");
    }

    public void RetryLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        Time.timeScale = 1f;
        WaveManager.isGameOver = false;
        Debug.Log("[Lose] Reloading scene: " + currentSceneName);
        SceneManager.LoadScene(currentSceneName);
    }
}
