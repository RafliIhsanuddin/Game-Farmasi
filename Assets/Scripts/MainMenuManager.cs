using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Semua Canvas (Optional)")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject BioRangerCanvas;
    [SerializeField] private GameObject BioEnemyCanvas;
    [SerializeField] private GameObject HowToPlayCanvas;

    private void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(true);
        if (BioRangerCanvas != null) BioRangerCanvas.SetActive(false);
        if (BioEnemyCanvas != null) BioEnemyCanvas.SetActive(false);
        if (HowToPlayCanvas != null) HowToPlayCanvas.SetActive(false);
    }

    public void ShowBioRanger()
    {
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
        if (BioRangerCanvas != null) BioRangerCanvas.SetActive(true);
        if (BioEnemyCanvas != null) BioEnemyCanvas.SetActive(false);
        if (HowToPlayCanvas != null) HowToPlayCanvas.SetActive(false);
    }

    public void ShowBioEnemy()
    {
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
        if (BioRangerCanvas != null) BioRangerCanvas.SetActive(false);
        if (BioEnemyCanvas != null) BioEnemyCanvas.SetActive(true);
        if (HowToPlayCanvas != null) HowToPlayCanvas.SetActive(false);
    }

    public void ShowHowToPlay()
    {
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
        if (BioRangerCanvas != null) BioRangerCanvas.SetActive(false);
        if (BioEnemyCanvas != null) BioEnemyCanvas.SetActive(false);
        if (HowToPlayCanvas != null) HowToPlayCanvas.SetActive(true);
    }
}