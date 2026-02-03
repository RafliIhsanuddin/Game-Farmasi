using UnityEngine;

public class QuestionMarkManager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject canvasDefault;
    [SerializeField] private GameObject canvasQuestionMark;

    // ========================
    // RANGER
    // ========================
    [Header("Ranger Canvas Root")]
    [SerializeField] private GameObject rangerCanvasRoot;

    [Header("Ranger Canvas")]
    [SerializeField] private GameObject rangerWhiteCanvas;
    [SerializeField] private GameObject rangerBlueCanvas;
    [SerializeField] private GameObject rangerPinkCanvas;
    [SerializeField] private GameObject rangerRedCanvas;
    [SerializeField] private GameObject rangerGreenCanvas;
    [SerializeField] private GameObject rangerYellowCanvas;

    // ========================
    // ENEMY
    // ========================
    [Header("Enemy Panel Root")]
    [SerializeField] private GameObject enemyPanelRoot;

    [Header("Enemy Panel")]
    [SerializeField] private GameObject enemyPurplePanel;
    [SerializeField] private GameObject enemyRedPanel;
    [SerializeField] private GameObject enemyVirusPanel;
    [SerializeField] private GameObject enemyFungiPanel;
    [SerializeField] private GameObject enemyProtozoaPanel;
    [SerializeField] private GameObject enemyHelminthPanel;

    [Header("Blocker (Optional)")]
    [SerializeField] private GameObject blockCanvas;

    private void Start()
    {
        ShowDefault();
    }

    // ========================
    // DEFAULT / QUESTION
    // ========================
    public void ShowDefault()
    {
        if (IsBlocked()) return;

        canvasDefault?.SetActive(true);
        canvasQuestionMark?.SetActive(false);

        DisableAllRanger();
        DisableAllEnemy();

        Time.timeScale = 1f;
        SoundManager.Instance?.ResumeBgm();
    }

    public void ShowQuestionMark()
    {
        if (IsBlocked()) return;

        canvasDefault?.SetActive(false);
        canvasQuestionMark?.SetActive(true);

        DisableAllRanger();
        DisableAllEnemy();

        Time.timeScale = 0f;
        SoundManager.Instance?.PauseBgm();
    }

    // ========================
    // RANGER (SATU AKTIF)
    // ========================
    public void ShowRangerWhite()
    {
        DisableAllRanger();
        DisableAllEnemy();
        rangerWhiteCanvas?.SetActive(true);
    }

    public void ShowRangerBlue()
    {
        DisableAllRanger();
        DisableAllEnemy();
        rangerBlueCanvas?.SetActive(true);
    }

    public void ShowRangerPink()
    {
        DisableAllRanger();
        DisableAllEnemy();
        rangerPinkCanvas?.SetActive(true);
    }

    public void ShowRangerRed()
    {
        DisableAllRanger();
        DisableAllEnemy();
        rangerRedCanvas?.SetActive(true);
    }

    public void ShowRangerGreen()
    {
        DisableAllRanger();
        DisableAllEnemy();
        rangerGreenCanvas?.SetActive(true);
    }

    public void ShowRangerYellow()
    {
        DisableAllRanger();
        DisableAllEnemy();
        rangerYellowCanvas?.SetActive(true);
    }

    public void ShowRangerCanvasRoot()
    {
        DisableAllRanger();
        DisableAllEnemy();
        rangerCanvasRoot?.SetActive(true);
    }

    // ========================
    // ENEMY (SATU AKTIF)
    // ========================
    public void ShowEnemyPurple()
    {
        DisableAllEnemy();
        DisableAllRanger();
        enemyPurplePanel?.SetActive(true);
    }

    public void ShowEnemyRed()
    {
        DisableAllEnemy();
        DisableAllRanger();
        enemyRedPanel?.SetActive(true);
    }

    public void ShowEnemyVirus()
    {
        DisableAllEnemy();
        DisableAllRanger();
        enemyVirusPanel?.SetActive(true);
    }

    public void ShowEnemyFungi()
    {
        DisableAllEnemy();
        DisableAllRanger();
        enemyFungiPanel?.SetActive(true);
    }

    public void ShowEnemyProtozoa()
    {
        DisableAllEnemy();
        DisableAllRanger();
        enemyProtozoaPanel?.SetActive(true);
    }

    public void ShowEnemyHelminth()
    {
        DisableAllEnemy();
        DisableAllRanger();
        enemyHelminthPanel?.SetActive(true);
    }

    public void ShowEnemyPanelRoot()
    {
        DisableAllEnemy();
        DisableAllRanger();
        enemyPanelRoot?.SetActive(true);
    }

    // ========================
    // DISABLE
    // ========================
    private void DisableAllRanger()
    {
        rangerCanvasRoot?.SetActive(false);
        rangerWhiteCanvas?.SetActive(false);
        rangerBlueCanvas?.SetActive(false);
        rangerPinkCanvas?.SetActive(false);
        rangerRedCanvas?.SetActive(false);
        rangerGreenCanvas?.SetActive(false);
        rangerYellowCanvas?.SetActive(false);
    }

    private void DisableAllEnemy()
    {
        enemyPanelRoot?.SetActive(false);
        enemyPurplePanel?.SetActive(false);
        enemyRedPanel?.SetActive(false);
        enemyVirusPanel?.SetActive(false);
        enemyFungiPanel?.SetActive(false);
        enemyProtozoaPanel?.SetActive(false);
        enemyHelminthPanel?.SetActive(false);
    }

    private bool IsBlocked()
    {
        return blockCanvas != null && blockCanvas.activeSelf;
    }
}
