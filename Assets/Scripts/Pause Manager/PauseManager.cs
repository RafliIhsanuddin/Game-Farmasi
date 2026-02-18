using UnityEngine;
using System.Collections.Generic;

public class PauseManager : MonoBehaviour
{
    [Header("Pause UI")]
    [SerializeField] private GameObject pauseUI;

    [Header("Pause Key")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    [Header("Block Objects")]
    [SerializeField] private List<GameObject> blockObjects = new();

    private bool isPaused = false;

    void Start()
    {
        if (pauseUI != null)
            pauseUI.SetActive(false);
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

        isPaused = true;

        Time.timeScale = 0f;

        if (pauseUI != null)
            pauseUI.SetActive(true);

        SoundManager.Instance?.PauseBgm();
    }

    public void ResumeGame()
    {
        if (IsBlocked()) return;

        isPaused = false;

        Time.timeScale = 1f;

        if (pauseUI != null)
            pauseUI.SetActive(false);

        SoundManager.Instance?.ResumeBgm();
    }

    public void OnResumeButton()
    {
        ResumeGame();
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    private bool IsBlocked()
    {
        foreach (var obj in blockObjects)
        {
            if (obj != null && obj.activeSelf)
                return true;
        }
        return false;
    }
}