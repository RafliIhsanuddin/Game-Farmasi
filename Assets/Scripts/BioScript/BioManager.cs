using UnityEngine;

public class BioManager : MonoBehaviour
{
    [Header("Semua Canvas (Optional)")]
    [SerializeField] private GameObject StageSelectionCanvas;
    [SerializeField] private GameObject BioRangerCanvas;
    [SerializeField] private GameObject BioEnemyCanvas;
    [SerializeField] private GameObject HowToPlayCanvas;

    private void Start()
    {
        ShowStageSelection();
    }

    public void ShowStageSelection()
    {
        if (StageSelectionCanvas != null) StageSelectionCanvas.SetActive(true);
        if (BioRangerCanvas != null) BioRangerCanvas.SetActive(false);
        if (BioEnemyCanvas != null) BioEnemyCanvas.SetActive(false);
        if (HowToPlayCanvas != null) HowToPlayCanvas.SetActive(false);
    }

    public void ShowBioRanger()
    {
        if (StageSelectionCanvas != null) StageSelectionCanvas.SetActive(false);
        if (BioRangerCanvas != null) BioRangerCanvas.SetActive(true);
        if (BioEnemyCanvas != null) BioEnemyCanvas.SetActive(false);
        if (HowToPlayCanvas != null) HowToPlayCanvas.SetActive(false);
    }

    public void ShowBioEnemy()
    {
        if (StageSelectionCanvas != null) StageSelectionCanvas.SetActive(false);
        if (BioRangerCanvas != null) BioRangerCanvas.SetActive(false);
        if (BioEnemyCanvas != null) BioEnemyCanvas.SetActive(true);
        if (HowToPlayCanvas != null) HowToPlayCanvas.SetActive(false);
    }

    public void ShowHowToPlay()
    {
        if (StageSelectionCanvas != null) StageSelectionCanvas.SetActive(false);
        if (BioRangerCanvas != null) BioRangerCanvas.SetActive(false);
        if (BioEnemyCanvas != null) BioEnemyCanvas.SetActive(false);
        if (HowToPlayCanvas != null) HowToPlayCanvas.SetActive(true);
    }
}