using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Optional Settings")]
    [Tooltip("Aktifkan jika ingin log di console saat pindah scene")]
    [SerializeField] private bool showDebugLog = true;

    // 🔹 Fungsi pindah scene (AMAN dari timeScale nyangkut)
    public void ChangeScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[UIManager] Nama scene belum diisi!");
            return;
        }

        // ✅ RESET TIMESCALE SEBELUM PINDAH SCENE
        Time.timeScale = 1f;

        if (showDebugLog)
            Debug.Log("[UIManager] TimeScale reset & pindah ke scene: " + sceneName);

        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        // ✅ JUGA RESET SAAT KELUAR GAME (BEST PRACTICE)
        Time.timeScale = 1f;

        if (showDebugLog)
            Debug.Log("[UIManager] Keluar dari game...");
        Application.Quit();
    }
}
