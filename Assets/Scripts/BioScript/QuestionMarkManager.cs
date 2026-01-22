using UnityEngine;
using System.Collections;

public class QuestionMarkManager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject canvasDefault;       // canvas utama
    [SerializeField] private GameObject canvasQuestionMark;  // canvas bantuan / info

    [Header("Blocker (Optional)")]
    [SerializeField] private GameObject blockCanvas;         // sunnah: kalau aktif → block behaviour

    private Coroutine resumeRoutine;

    private void Start()
    {
        ShowDefault();
    }

    public void ShowDefault()
    {
        if (IsBlocked()) return; // <<< BLOCKER CHECK

        if (canvasDefault != null) canvasDefault.SetActive(true);
        if (canvasQuestionMark != null) canvasQuestionMark.SetActive(false);

        if (resumeRoutine != null)
            StopCoroutine(resumeRoutine);

        // LANGSUNG RESUME TANPA DELAY
        Time.timeScale = 1f;
        resumeRoutine = null;

        if (SoundManager.Instance != null)
            SoundManager.Instance.ResumeBgm();
    }

    public void ShowQuestionMark()
    {
        if (IsBlocked()) return; // <<< BLOCKER CHECK

        if (canvasDefault != null) canvasDefault.SetActive(false);
        if (canvasQuestionMark != null) canvasQuestionMark.SetActive(true);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PauseBgm();

        Time.timeScale = 0f;

        if (resumeRoutine != null)
        {
            StopCoroutine(resumeRoutine);
            resumeRoutine = null;
        }
    }

    // ===============================================
    // BLOCK LOGIC (SUNNAH, OPSIONAL, NO ERROR)
    // ===============================================
    private bool IsBlocked()
    {
        if (blockCanvas == null) return false;  // sunnah, tidak ditaruh → tidak block

        return blockCanvas.activeSelf;          // kalau aktif → block behaviour
    }
}