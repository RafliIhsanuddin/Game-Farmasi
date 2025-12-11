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
        if (!hasTriggered && other.gameObject.layer == 8)
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

    public void RetryLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        Time.timeScale = 1f;
        WaveManager.isGameOver = false;
        Debug.Log("[Lose] Reloading scene: " + currentSceneName);
        SceneManager.LoadScene(currentSceneName);
    }
    
}
