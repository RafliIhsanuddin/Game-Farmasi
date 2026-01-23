using UnityEngine;

public class BioManager : MonoBehaviour
{
    [Header("Semua Canvas (Optional)")]
    [SerializeField] private GameObject StageSelectionCanvas;
    [SerializeField] private GameObject BioRangerCanvas;
    [SerializeField] private GameObject BioEnemyCanvas;
    [SerializeField] private GameObject HowToPlayCanvas;

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
    // CANVAS BEHAVIOUR
    // =====================
    public void ShowStageSelection()
    {
        if (StageSelectionCanvas != null) StageSelectionCanvas.SetActive(true);
        if (BioRangerCanvas != null) BioRangerCanvas.SetActive(false);
        if (BioEnemyCanvas != null) BioEnemyCanvas.SetActive(false);
        if (HowToPlayCanvas != null) HowToPlayCanvas.SetActive(false);

        DisableAllCharacters();
    }

    public void ShowBioRanger()
    {
        if (StageSelectionCanvas != null) StageSelectionCanvas.SetActive(false);
        if (BioRangerCanvas != null) BioRangerCanvas.SetActive(true);
        if (BioEnemyCanvas != null) BioEnemyCanvas.SetActive(false);
        if (HowToPlayCanvas != null) HowToPlayCanvas.SetActive(false);

        DisableAllCharacters();
    }

    public void ShowBioEnemy()
    {
        if (StageSelectionCanvas != null) StageSelectionCanvas.SetActive(false);
        if (BioRangerCanvas != null) BioRangerCanvas.SetActive(false);
        if (BioEnemyCanvas != null) BioEnemyCanvas.SetActive(true);
        if (HowToPlayCanvas != null) HowToPlayCanvas.SetActive(false);

        DisableAllCharacters();
    }

    public void ShowHowToPlay()
    {
        if (StageSelectionCanvas != null) StageSelectionCanvas.SetActive(false);
        if (BioRangerCanvas != null) BioRangerCanvas.SetActive(false);
        if (BioEnemyCanvas != null) BioEnemyCanvas.SetActive(false);
        if (HowToPlayCanvas != null) HowToPlayCanvas.SetActive(true);

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
