using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Optional Settings")]
    [Tooltip("Aktifkan jika ingin log di console saat pindah scene")]
    [SerializeField] private bool showDebugLog = true;

    // 🔹 Fungsi ini bisa dipanggil dari tombol OnClick dengan parameter scene
    public void ChangeScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[UIManager] Nama scene belum diisi!");
            return;
        }

        if (showDebugLog)
            Debug.Log("[UIManager] Pindah ke scene: " + sceneName);

        SceneManager.LoadScene(sceneName);
    }

    // 🔹 Fungsi untuk tombol keluar game (opsional)
    public void QuitGame()
    {
        if (showDebugLog)
            Debug.Log("[UIManager] Keluar dari game...");
        Application.Quit();
    }
}
