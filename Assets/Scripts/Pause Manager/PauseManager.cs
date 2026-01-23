using UnityEngine;
using System.Collections.Generic;

public class PauseManager : MonoBehaviour
{
    [Header("UI Pause Panel")]
    [SerializeField] private GameObject pauseUI;

    [Header("Settings")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    [Header("Blockers (Optional, Sunnah)")]
    [SerializeField] private List<GameObject> blockObjects = new();

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
        if (IsBlocked()) return;

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

        if (SoundManager.Instance != null)
            SoundManager.Instance.PauseBgm();
    }

    public void ResumeGame()
    {
        if (IsBlocked()) return;

        if (pauseUI != null)
            pauseUI.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;

        if (SoundManager.Instance != null)
            SoundManager.Instance.ResumeBgm();
    }

    public void OnResumeButton()
    {
        if (IsBlocked()) return;
        ResumeGame();
    }

    public void OnQuitButton()
    {
        if (IsBlocked()) return;
        Application.Quit();
    }

    private bool IsBlocked()
    {
        if (blockObjects == null || blockObjects.Count == 0)
            return false;

        foreach (var obj in blockObjects)
        {
            if (obj != null && obj.activeSelf)
                return true;
        }

        return false;
    }
}