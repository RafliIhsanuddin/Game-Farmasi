using UnityEngine;

public class DiseaseManagerScript : MonoBehaviour
{
    [Header("Disease Panels")]
    [SerializeField] private GameObject panelDefault;
    [SerializeField] private GameObject panelAcutePharyngitis;
    [SerializeField] private GameObject panelAcuteOtitisMedia;
    [SerializeField] private GameObject panelUrinaryTractInfections;
    [SerializeField] private GameObject panelChronicBronchitis;
    [SerializeField] private GameObject panelCAP; // Community-Acquired Pneumonia
    [SerializeField] private GameObject panelHAP; // Hospital-Acquired Pneumonia

    private void Start()
    {
        ShowDefaultPanel();
    }

    // =========================
    // CORE HELPER
    // =========================
    private void DisableAllPanels()
    {
        if (panelDefault != null) panelDefault.SetActive(false);
        if (panelAcutePharyngitis != null) panelAcutePharyngitis.SetActive(false);
        if (panelAcuteOtitisMedia != null) panelAcuteOtitisMedia.SetActive(false);
        if (panelUrinaryTractInfections != null) panelUrinaryTractInfections.SetActive(false);
        if (panelChronicBronchitis != null) panelChronicBronchitis.SetActive(false);
        if (panelCAP != null) panelCAP.SetActive(false);
        if (panelHAP != null) panelHAP.SetActive(false);
    }

    // =========================
    // SHOW FUNCTIONS
    // =========================
    public void ShowDefaultPanel()
    {
        DisableAllPanels();
        if (panelDefault != null) panelDefault.SetActive(true);
    }

    public void ShowAcutePharyngitis()
    {
        DisableAllPanels();
        if (panelAcutePharyngitis != null) panelAcutePharyngitis.SetActive(true);
    }

    public void ShowAcuteOtitisMedia()
    {
        DisableAllPanels();
        if (panelAcuteOtitisMedia != null) panelAcuteOtitisMedia.SetActive(true);
    }

    public void ShowUrinaryTractInfections()
    {
        DisableAllPanels();
        if (panelUrinaryTractInfections != null) panelUrinaryTractInfections.SetActive(true);
    }

    public void ShowChronicBronchitis()
    {
        DisableAllPanels();
        if (panelChronicBronchitis != null) panelChronicBronchitis.SetActive(true);
    }

    public void ShowCAP()
    {
        DisableAllPanels();
        if (panelCAP != null) panelCAP.SetActive(true);
    }

    public void ShowHAP()
    {
        DisableAllPanels();
        if (panelHAP != null) panelHAP.SetActive(true);
    }
}