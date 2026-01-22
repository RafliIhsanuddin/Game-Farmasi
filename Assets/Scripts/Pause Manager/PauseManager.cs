using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("UI Pause Panel")]
    [SerializeField] private GameObject pauseUI;

    [Header("Settings")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    [Header("Blocker (Optional)")]
    [SerializeField] private GameObject blockObject; 
    // Sunnah: kalau tidak diassign, tidak error, tidak nge-block

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
        // Kalau diblok, semua behaviour PauseManager dimatikan
        if (IsBlocked()) return;

        // Cegah pause jika game sudah Win atau Lose
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
        if (IsBlocked()) return;

        if (pauseUI != null)
            pauseUI.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;

        // PAUSE BGM
        if (SoundManager.Instance != null)
            SoundManager.Instance.PauseBgm();

        Debug.Log("[PauseManager] Game Paused");
    }

    public void ResumeGame()
    {
        if (IsBlocked()) return;

        if (pauseUI != null)
            pauseUI.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;

        // RESUME BGM
        if (SoundManager.Instance != null)
            SoundManager.Instance.ResumeBgm();

        Debug.Log("[PauseManager] Game Resumed");
    }

    public void OnResumeButton()
    {
        if (IsBlocked()) return;
        ResumeGame();
    }

    public void OnQuitButton()
    {
        if (IsBlocked()) return;

        Debug.Log("[PauseManager] Quit game...");
        Application.Quit();
    }

    // ============================
    // BLOCK LOGIC (SUNNAH)
    // ============================
    private bool IsBlocked()
    {
        // Kalau tidak diassign → tidak nge-block
        if (blockObject == null) return false;

        // Kalau diassign dan aktif → block behaviour
        return blockObject.activeSelf;
    }
}
