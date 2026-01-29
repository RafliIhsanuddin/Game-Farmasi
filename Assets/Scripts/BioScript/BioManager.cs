using UnityEngine;

public class BioManager : MonoBehaviour
{
    [Header("Semua Panel / GameObject (Optional)")]
    [SerializeField] private GameObject stageSelectionGO;
    [SerializeField] private GameObject bioRangerGO;
    [SerializeField] private GameObject bioEnemyGO;
    [SerializeField] private GameObject howToPlayGO;

    [Header("Characters (Optional)")]
    [SerializeField] private GameObject whiteCharacter;
    [SerializeField] private GameObject blueCharacter;
    [SerializeField] private GameObject pinkCharacter;
    [SerializeField] private GameObject redCharacter;
    [SerializeField] private GameObject greenCharacter;
    [SerializeField] private GameObject yellowCharacter;

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

        DisableAllCharacters();
    }

    public void ShowBioRanger()
    {
        if (stageSelectionGO != null) stageSelectionGO.SetActive(false);
        if (bioRangerGO != null) bioRangerGO.SetActive(true);
        if (bioEnemyGO != null) bioEnemyGO.SetActive(false);
        if (howToPlayGO != null) howToPlayGO.SetActive(false);

        DisableAllCharacters();
    }

    public void ShowBioEnemy()
    {
        if (stageSelectionGO != null) stageSelectionGO.SetActive(false);
        if (bioRangerGO != null) bioRangerGO.SetActive(false);
        if (bioEnemyGO != null) bioEnemyGO.SetActive(true);
        if (howToPlayGO != null) howToPlayGO.SetActive(false);

        DisableAllCharacters();
    }

    public void ShowHowToPlay()
    {
        if (stageSelectionGO != null) stageSelectionGO.SetActive(false);
        if (bioRangerGO != null) bioRangerGO.SetActive(false);
        if (bioEnemyGO != null) bioEnemyGO.SetActive(false);
        if (howToPlayGO != null) howToPlayGO.SetActive(true);

        DisableAllCharacters();
    }

    // =====================
    // CHARACTER BEHAVIOUR
    // =====================
    private void DisableAllCharacters()
    {
        if (whiteCharacter != null) whiteCharacter.SetActive(false);
        if (blueCharacter != null) blueCharacter.SetActive(false);
        if (pinkCharacter != null) pinkCharacter.SetActive(false);
        if (redCharacter != null) redCharacter.SetActive(false);
        if (greenCharacter != null) greenCharacter.SetActive(false);
        if (yellowCharacter != null) yellowCharacter.SetActive(false);
    }

    private void ShowOnlyCharacter(GameObject target)
    {
        if (whiteCharacter != null) whiteCharacter.SetActive(target == whiteCharacter);
        if (blueCharacter != null) blueCharacter.SetActive(target == blueCharacter);
        if (pinkCharacter != null) pinkCharacter.SetActive(target == pinkCharacter);
        if (redCharacter != null) redCharacter.SetActive(target == redCharacter);
        if (greenCharacter != null) greenCharacter.SetActive(target == greenCharacter);
        if (yellowCharacter != null) yellowCharacter.SetActive(target == yellowCharacter);
    }

    public void ShowWhite() => ShowOnlyCharacter(whiteCharacter);
    public void ShowBlue() => ShowOnlyCharacter(blueCharacter);
    public void ShowPink() => ShowOnlyCharacter(pinkCharacter);
    public void ShowRed() => ShowOnlyCharacter(redCharacter);
    public void ShowGreen() => ShowOnlyCharacter(greenCharacter);
    public void ShowYellow() => ShowOnlyCharacter(yellowCharacter);
}
