using UnityEngine;
using System.Collections;

public class QuestionMarkManager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject canvasDefault;       // canvas utama
    [SerializeField] private GameObject canvasQuestionMark;  // canvas bantuan / info

    [Header("Characters (Optional)")]
    [SerializeField] private GameObject whiteCharacter;
    [SerializeField] private GameObject blueCharacter;
    [SerializeField] private GameObject pinkCharacter;
    [SerializeField] private GameObject redCharacter;
    [SerializeField] private GameObject greenCharacter;
    [SerializeField] private GameObject yellowCharacter;

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

        DisableAllCharacters();

        // langsung resume tanpa delay
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

        DisableAllCharacters();

        if (SoundManager.Instance != null)
            SoundManager.Instance.PauseBgm();

        Time.timeScale = 0f;

        if (resumeRoutine != null)
        {
            StopCoroutine(resumeRoutine);
            resumeRoutine = null;
        }
    }

    // ========================
    // CHARACTER CONTROL
    // ========================
    private void DisableAllCharacters()
    {
        if (whiteCharacter != null) whiteCharacter.SetActive(false);
        if (blueCharacter != null) blueCharacter.SetActive(false);
        if (pinkCharacter != null) pinkCharacter.SetActive(false);
        if (redCharacter != null) redCharacter.SetActive(false);
        if (greenCharacter != null) greenCharacter.SetActive(false);
        if (yellowCharacter != null) yellowCharacter.SetActive(false);
    }

    // ========================
    // BLOCK LOGIC (SUNNAH)
    // ========================
    private bool IsBlocked()
    {
        if (blockCanvas == null) return false;  // sunnah, tidak ditaruh → tidak block

        return blockCanvas.activeSelf;          // kalau aktif → block behaviour
    }
}
