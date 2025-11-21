using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Semua Canvas")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject infeksiParuCanvas;
    [SerializeField] private GameObject infeksiUsusCanvas;
    [SerializeField] private GameObject infeksiKulitCanvas;

    private void Start()
    {
        // Saat pertama kali play, hanya MainMenu yang aktif
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        mainMenuCanvas.SetActive(true);
        infeksiParuCanvas.SetActive(false);
        infeksiUsusCanvas.SetActive(false);
        infeksiKulitCanvas.SetActive(false);
    }

    public void ShowInfeksiParu()
    {
        mainMenuCanvas.SetActive(false);
        infeksiParuCanvas.SetActive(true);
        infeksiUsusCanvas.SetActive(false);
        infeksiKulitCanvas.SetActive(false);
    }

    public void ShowInfeksiUsus()
    {
        mainMenuCanvas.SetActive(false);
        infeksiParuCanvas.SetActive(false);
        infeksiUsusCanvas.SetActive(true);
        infeksiKulitCanvas.SetActive(false);
    }

    public void ShowInfeksiKulit()
    {
        mainMenuCanvas.SetActive(false);
        infeksiParuCanvas.SetActive(false);
        infeksiUsusCanvas.SetActive(false);
        infeksiKulitCanvas.SetActive(true);
    }
}
