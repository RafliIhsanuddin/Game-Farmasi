using UnityEngine;
using System;
using System.Collections.Generic;

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; } = false;

    // 🔹 Event: semua UI/logic bisa subscribe
    public static event Action<bool> OnPauseChanged;

    [Header("Pause UI")]
    [SerializeField] private GameObject pauseUI;

    [Header("Pause Key")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    [Header("Block Objects")]
    [SerializeField] private List<GameObject> blockObjects = new();

    private bool isPaused = false;

    private void Start()
    {
        if (pauseUI != null)
            pauseUI.SetActive(false);

        SetPaused(false);
    }

    private void Update()
    {
        if (IsBlocked()) return;
        if (WaveManager.isGameOver) return;

        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        if (IsBlocked()) return;
        if (WaveManager.isGameOver) return;

        SetPaused(true);

        Time.timeScale = 0f;

        if (pauseUI != null)
            pauseUI.SetActive(true);

        SoundManager.Instance?.PauseBgm();
    }

    public void ResumeGame()
    {
        // Resume sebaiknya boleh walau pauseUI lagi aktif,
        // tapi tetap hormati blockObjects kalau kamu memang mau.
        if (IsBlocked()) return;
        if (WaveManager.isGameOver) return;

        SetPaused(false);

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

    private void SetPaused(bool paused)
    {
        isPaused = paused;
        IsPaused = paused;

        // 🔹 Broadcast event
        OnPauseChanged?.Invoke(paused);
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
