using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lose : MonoBehaviour
{
    
    [Header("Animator Settings")]
    [SerializeField] private Animator animator;

    [Header("GameObjects To Activate After Delay")]
    [Tooltip("Masukkan GameObject yang ingin diaktifkan setelah delay di sini")]
    [SerializeField] private List<GameObject> objectsToActivate = new List<GameObject>();

    [Header("Delay Settings")]
    [Tooltip("Waktu delay sebelum mengaktifkan GameObject (dalam detik)")]
    [SerializeField] private float activationDelay = 2f;

    private bool hasTriggered = false;

    private void Start()
    {
        // 🔹 Pastikan semua objek dinonaktifkan di awal
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Layer 8 biasanya untuk player — bisa kamu ganti sesuai kebutuhan
        if (!hasTriggered && other.gameObject.layer == 8)
        {
            hasTriggered = true;
            Debug.Log("[Lose] Triggered by: " + other.gameObject.name);

            if (animator != null)
            {
                animator.Play("DeathAnimation");
                Debug.Log("[Lose] DeathAnimation played");
            }

            // 🔹 Jalankan coroutine untuk mengaktifkan objek dengan delay
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
    
    // 🔹 Fungsi Retry — bisa dipanggil dari tombol UI
    public void RetryLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log("[Lose] Reloading scene: " + currentSceneName);
        SceneManager.LoadScene(currentSceneName);
    }
    
}
