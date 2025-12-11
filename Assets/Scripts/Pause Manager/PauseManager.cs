using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("UI Pause Panel")]
    [Tooltip("Drag panel pause (Canvas child) ke sini")]
    [SerializeField] private GameObject pauseUI;

    [Header("Settings")]
    [Tooltip("Tombol untuk pause/resume (default: Escape)")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    private bool isPaused = false;

    void Start()
    {
        if (pauseUI != null)
            pauseUI.SetActive(false);
        else
            Debug.LogWarning("[PauseManager] Pause UI belum diassign di Inspector!");
    }

    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pauseUI != null)
            pauseUI.SetActive(true);

        Time.timeScale = 0f; // Hentikan waktu di game
        isPaused = true;
        Debug.Log("[PauseManager] Game Paused");
    }

    public void ResumeGame()
    {
        if (pauseUI != null)
            pauseUI.SetActive(false);

        Time.timeScale = 1f; // Lanjutkan waktu
        isPaused = false;
        Debug.Log("[PauseManager] Game Resumed");
    }

    // 🔹 Tambahan jika kamu ingin tombol Resume di UI
    public void OnResumeButton()
    {
        ResumeGame();
    }

    // 🔹 Tambahan jika ingin tombol Quit di UI pause
    public void OnQuitButton()
    {
        Debug.Log("[PauseManager] Quit game...");
        Application.Quit();
    }
}
