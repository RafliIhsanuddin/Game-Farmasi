using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("UI Pause Panel")]
    [SerializeField] private GameObject pauseUI;

    [Header("Settings")]
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
        // 🔹 Cegah pause jika game sudah Win atau Lose
        if (WaveManager.isGameOver) return;

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

        Time.timeScale = 0f;
        isPaused = true;
        Debug.Log("[PauseManager] Game Paused");
    }

    public void ResumeGame()
    {
        if (pauseUI != null)
            pauseUI.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("[PauseManager] Game Resumed");
    }

    public void OnResumeButton()
    {
        ResumeGame();
    }

    public void OnQuitButton()
    {
        Debug.Log("[PauseManager] Quit game...");
        Application.Quit();
    }
}
