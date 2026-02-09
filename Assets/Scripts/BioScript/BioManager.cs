using UnityEngine;

public class BioManager : MonoBehaviour
{
    [Header("Semua Panel / GameObject (Optional)")]
    [SerializeField] private GameObject stageSelectionGO;
    [SerializeField] private GameObject bioRangerGO;
    [SerializeField] private GameObject bioEnemyGO;
    [SerializeField] private GameObject howToPlayGO;

    // =====================
    // ENEMY BIO SUB PANELS
    // =====================
    [Header("Enemy Bio Panels")]
    [SerializeField] private GameObject ChooseEnemyBioPanel;
    [SerializeField] private GameObject EnemyOtherBioPanel;
    [SerializeField] private GameObject EnemyBacteriaBioPanel;

    // =====================
    // RANGER CHARACTERS (Optional)
    // =====================
    [Header("Ranger Characters (Optional)")]
    [SerializeField] private GameObject whiteCharacter;
    [SerializeField] private GameObject blueCharacter;
    [SerializeField] private GameObject pinkCharacter;
    [SerializeField] private GameObject redCharacter;
    [SerializeField] private GameObject greenCharacter;
    [SerializeField] private GameObject yellowCharacter;
    [SerializeField] private GameObject blackCharacter; // ✅ NEW

    // =====================
    // BACTERIA ENEMIES
    // =====================
    [Header("Bacteria Enemies (Optional)")]
    [SerializeField] private GameObject bacteriaPurple;
    [SerializeField] private GameObject bacteriaRed;
    [SerializeField] private GameObject bacteriaYellow;
    [SerializeField] private GameObject bacteriaOrange;
    [SerializeField] private GameObject bacteriaPink;
    [SerializeField] private GameObject bacteriaBlue;
    [SerializeField] private GameObject bacteriaGreen;

    // =====================
    // OTHER ENEMIES
    // =====================
    [Header("Other Enemies (Optional)")]
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
        if (blackCharacter != null) blackCharacter.SetActive(false); // ✅ NEW
    }

    private void ShowOnlyRanger(GameObject target)
    {
        if (whiteCharacter != null) whiteCharacter.SetActive(target == whiteCharacter);
        if (blueCharacter != null) blueCharacter.SetActive(target == blueCharacter);
        if (pinkCharacter != null) pinkCharacter.SetActive(target == pinkCharacter);
        if (redCharacter != null) redCharacter.SetActive(target == redCharacter);
        if (greenCharacter != null) greenCharacter.SetActive(target == greenCharacter);
        if (yellowCharacter != null) yellowCharacter.SetActive(target == yellowCharacter);
        if (blackCharacter != null) blackCharacter.SetActive(target == blackCharacter); // ✅ NEW
    }

    public void ShowWhite()  => ShowOnlyRanger(whiteCharacter);
    public void ShowBlue()   => ShowOnlyRanger(blueCharacter);
    public void ShowPink()   => ShowOnlyRanger(pinkCharacter);
    public void ShowRed()    => ShowOnlyRanger(redCharacter);
    public void ShowGreen()  => ShowOnlyRanger(greenCharacter);
    public void ShowYellow() => ShowOnlyRanger(yellowCharacter);
    public void ShowBlack()  => ShowOnlyRanger(blackCharacter); // ✅ NEW

    // =====================
    // ENEMY BEHAVIOUR
    // =====================
    private void DisableAllEnemies()
    {
        if (bacteriaPurple != null) bacteriaPurple.SetActive(false);
        if (bacteriaRed != null) bacteriaRed.SetActive(false);
        if (bacteriaYellow != null) bacteriaYellow.SetActive(false);
        if (bacteriaOrange != null) bacteriaOrange.SetActive(false);
        if (bacteriaPink != null) bacteriaPink.SetActive(false);
        if (bacteriaBlue != null) bacteriaBlue.SetActive(false);
        if (bacteriaGreen != null) bacteriaGreen.SetActive(false);

        if (fungiEnemy != null) fungiEnemy.SetActive(false);
        if (virusEnemy != null) virusEnemy.SetActive(false);
        if (protozoaEnemy != null) protozoaEnemy.SetActive(false);
        if (helminthEnemy != null) helminthEnemy.SetActive(false);
    }

    // =====================
    // ENEMY BIO PANEL BEHAVIOUR
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
        DisableAllEnemies();
        if (ChooseEnemyBioPanel != null) ChooseEnemyBioPanel.SetActive(true);
    }

    public void ShowEnemyOtherBioPanel()
    {
        DisableAllEnemyBioPanels();
        DisableAllEnemies();
        if (EnemyOtherBioPanel != null) EnemyOtherBioPanel.SetActive(true);
    }

    public void ShowEnemyBacteriaBioPanel()
    {
        DisableAllEnemyBioPanels();
        DisableAllEnemies();
        if (EnemyBacteriaBioPanel != null) EnemyBacteriaBioPanel.SetActive(true);
    }
}
