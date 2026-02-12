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

            WaveManager.isGameOver = true;

            Debug.Log("[Lose] Triggered by: " + other.gameObject.name);

            // 🔴 STOP MUSIC LANGSUNG
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.StopBgmLoop();
                Debug.Log("[Lose] BGM stopped immediately.");
            }

            if (animator != null)
            {
                animator.Play("DeathAnimation");
                Debug.Log("[Lose] DeathAnimation played");
            }

            StartCoroutine(ActivateObjectsAfterDelay());
            StartCoroutine(PauseAfterDelay());
        }
    }

    private IEnumerator ActivateObjectsAfterDelay()
    {
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
