using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lose : MonoBehaviour
{
    [Header("Animator Settings (Optional)")]
    [SerializeField] private Animator animator;

    [Header("GameObjects To Activate After Delay (Optional)")]
    [SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();

    [Header("GameObjects To Disable On Lose (Optional)")]
    [SerializeField] private List<GameObject> objectsToDisableOnLose = new List<GameObject>();

    [Header("Delay Settings")]
    [SerializeField] private float activationDelay = 2f;

    [Header("Enemy Layer Mask (Required)")]
    [SerializeField] private LayerMask enemyLayerMask;

    [Header("Pause Delay After Trigger (Seconds)")]
    [SerializeField] private float pauseDelay = 7f;

    private bool hasTriggered = false;

    private void Start()
    {
        // Aman walaupun kosong
        if (objectsToActivate != null && objectsToActivate.Count > 0)
        {
            foreach (GameObject obj in objectsToActivate)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool isEnemy = (enemyLayerMask.value & (1 << other.gameObject.layer)) != 0;

        if (!hasTriggered && isEnemy)
        {
            hasTriggered = true;

            WaveManager.isGameOver = true;

            Debug.Log("[Lose] Triggered by: " + other.gameObject.name);

            // 🔴 STOP MUSIC LANGSUNG (Optional)
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.StopBgmLoop();
                Debug.Log("[Lose] BGM stopped immediately.");
            }

            // 🔴 NONAKTIFKAN OBJECT LIST (Optional)
            DisableObjectsOnLose();

            // 🔴 Play Animation (Optional)
            if (animator != null)
            {
                animator.Play("DeathAnimation");
                Debug.Log("[Lose] DeathAnimation played");
            }

            StartCoroutine(ActivateObjectsAfterDelay());
            StartCoroutine(PauseAfterDelay());
        }
    }

    private void DisableObjectsOnLose()
    {
        if (objectsToDisableOnLose == null || objectsToDisableOnLose.Count == 0)
            return; // Sunnah, tidak wajib

        foreach (GameObject obj in objectsToDisableOnLose)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        Debug.Log("[Lose] Objects disabled.");
    }

    private IEnumerator ActivateObjectsAfterDelay()
    {
        if (objectsToActivate == null || objectsToActivate.Count == 0)
            yield break;

        yield return new WaitForSeconds(activationDelay);

        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }

    private IEnumerator PauseAfterDelay()
    {
        yield return new WaitForSeconds(pauseDelay);

        var counter = Object.FindFirstObjectByType<ListCounterManager>();
        if (counter != null)
        {
            counter.ForceFinalCount();
        }

        Time.timeScale = 0f;

        Debug.Log("[Lose] GAME PAUSED");
    }

    public void RetryLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        Time.timeScale = 1f;
        WaveManager.isGameOver = false;

        SceneManager.LoadScene(currentSceneName);
    }
}
