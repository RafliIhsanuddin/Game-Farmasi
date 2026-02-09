using UnityEngine;

public class BioManager : MonoBehaviour
{
    [Header("Semua Panel / GameObject (Optional)")]
    [SerializeField] private GameObject stageSelectionGO;
    [SerializeField] private GameObject bioRangerGO;
    [SerializeField] private GameObject bioEnemyGO;
    [SerializeField] private GameObject howToPlayGO;

    // =====================
    // ENEMY BIO SUB PANELS (NEW)
    // =====================
    [Header("Enemy Bio Panels")]
    [SerializeField] private GameObject ChooseEnemyBioPanel;
    [SerializeField] private GameObject EnemyOtherBioPanel;
    [SerializeField] private GameObject EnemyBacteriaBioPanel;

    [Header("Ranger Characters (Optional)")]
    [SerializeField] private GameObject whiteCharacter;
    [SerializeField] private GameObject blueCharacter;
    [SerializeField] private GameObject pinkCharacter;
    [SerializeField] private GameObject redCharacter;
    [SerializeField] private GameObject greenCharacter;
    [SerializeField] private GameObject yellowCharacter;

    [Header("Enemy Characters (Optional)")]
    [SerializeField] private GameObject purpleEnemy;
    [SerializeField] private GameObject redEnemy;
    [SerializeField] private GameObject fungiEnemy;
    [SerializeField] private GameObject virusEnemy;
    [SerializeField] private GameObject protozoaEnemy;
    [SerializeField] private GameObject helminthEnemy;

    private void Start()
    {
        ShowStageSelection();
    }

    // =====================
    // GAMEOBJECT / PANEL BEHAVIOUR
    // =====================
    public void ShowStageSelection()
    {
        if (stageSelectionGO != null) stageSelectionGO.SetActive(true);
        if (bioRangerGO != null) bioRangerGO.SetActive(false);
        if (bioEnemyGO != null) bioEnemyGO.SetActive(false);
        if (howToPlayGO != null) howToPlayGO.SetActive(false);

        DisableAllRangers();
        DisableAllEnemies();
        DisableAllEnemyBioPanels();
    }

    public void ShowBioRanger()
    {
        if (stageSelectionGO != null) stageSelectionGO.SetActive(false);
        if (bioRangerGO != null) bioRangerGO.SetActive(true);
        if (bioEnemyGO != null) bioEnemyGO.SetActive(false);
        if (howToPlayGO != null) howToPlayGO.SetActive(false);

        DisableAllRangers();
        DisableAllEnemies();
        DisableAllEnemyBioPanels();
    }

    public void ShowBioEnemy()
    {
        if (stageSelectionGO != null) stageSelectionGO.SetActive(false);
        if (bioRangerGO != null) bioRangerGO.SetActive(false);
        if (bioEnemyGO != null) bioEnemyGO.SetActive(true);
        if (howToPlayGO != null) howToPlayGO.SetActive(false);

        DisableAllRangers();
        DisableAllEnemies();
        DisableAllEnemyBioPanels();
    }

    public void ShowHowToPlay()
    {
        if (stageSelectionGO != null) stageSelectionGO.SetActive(false);
        if (bioRangerGO != null) bioRangerGO.SetActive(false);
        if (bioEnemyGO != null) bioEnemyGO.SetActive(false);
        if (howToPlayGO != null) howToPlayGO.SetActive(true);

        DisableAllRangers();
        DisableAllEnemies();
        DisableAllEnemyBioPanels();
    }

    // =====================
    // RANGER BEHAVIOUR
    // =====================
    private void DisableAllRangers()
    {
        if (whiteCharacter != null) whiteCharacter.SetActive(false);
        if (blueCharacter != null) blueCharacter.SetActive(false);
        if (pinkCharacter != null) pinkCharacter.SetActive(false);
        if (redCharacter != null) redCharacter.SetActive(false);
        if (greenCharacter != null) greenCharacter.SetActive(false);
        if (yellowCharacter != null) yellowCharacter.SetActive(false);
    }

    private void ShowOnlyRanger(GameObject target)
    {
        if (whiteCharacter != null) whiteCharacter.SetActive(target == whiteCharacter);
        if (blueCharacter != null) blueCharacter.SetActive(target == blueCharacter);
        if (pinkCharacter != null) pinkCharacter.SetActive(target == pinkCharacter);
        if (redCharacter != null) redCharacter.SetActive(target == redCharacter);
        if (greenCharacter != null) greenCharacter.SetActive(target == greenCharacter);
        if (yellowCharacter != null) yellowCharacter.SetActive(target == yellowCharacter);
    }

    public void ShowWhite()  => ShowOnlyRanger(whiteCharacter);
    public void ShowBlue()   => ShowOnlyRanger(blueCharacter);
    public void ShowPink()   => ShowOnlyRanger(pinkCharacter);
    public void ShowRed()    => ShowOnlyRanger(redCharacter);
    public void ShowGreen()  => ShowOnlyRanger(greenCharacter);
    public void ShowYellow() => ShowOnlyRanger(yellowCharacter);

    // =====================
    // ENEMY BEHAVIOUR
    // =====================
    private void DisableAllEnemies()
    {
        if (purpleEnemy != null) purpleEnemy.SetActive(false);
        if (redEnemy != null) redEnemy.SetActive(false);
        if (fungiEnemy != null) fungiEnemy.SetActive(false);
        if (virusEnemy != null) virusEnemy.SetActive(false);
        if (protozoaEnemy != null) protozoaEnemy.SetActive(false);
        if (helminthEnemy != null) helminthEnemy.SetActive(false);
    }

    // =====================
    // ENEMY BIO PANEL BEHAVIOUR (NEW)
    // =====================
    private void DisableAllEnemyBioPanels()
    {
        if (ChooseEnemyBioPanel != null) ChooseEnemyBioPanel.SetActive(false);
        if (EnemyOtherBioPanel != null) EnemyOtherBioPanel.SetActive(false);
        if (EnemyBacteriaBioPanel != null) EnemyBacteriaBioPanel.SetActive(false);
    }

    public void ShowChooseEnemyBioPanel()
    {
        DisableAllEnemyBioPanels();
        if (ChooseEnemyBioPanel != null) ChooseEnemyBioPanel.SetActive(true);
    }

    public void ShowEnemyOtherBioPanel()
    {
        DisableAllEnemyBioPanels();
        if (EnemyOtherBioPanel != null) EnemyOtherBioPanel.SetActive(true);
    }

    public void ShowEnemyBacteriaBioPanel()
    {
        DisableAllEnemyBioPanels();
        if (EnemyBacteriaBioPanel != null) EnemyBacteriaBioPanel.SetActive(true);
    }
}
