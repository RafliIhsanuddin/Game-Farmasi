using UnityEngine;
using TMPro;

public class WelcomeMenuController : MonoBehaviour
{
    [Header("Welcome Text (Main Menu) - Sunnah")]
    [SerializeField] private TextMeshProUGUI welcomeText; 

    [Header("Stage Selection Text - Sunnah (Opsional)")]
    [SerializeField] private TextMeshProUGUI stageSelectionText;

    private void Start()
    {
        Refresh();
        UserManager.Instance.OnUserChanged += HandleUserChanged;
    }

    private void HandleUserChanged(string newUser)
    {
        Refresh();
    }

    private void Refresh()
    {
        string name = UserManager.Instance.CurrentUser;

        // Jika welcomeText di assign → pakai format awal (seperti skrip original)
        if (welcomeText)
            welcomeText.text = $"{name}";

        // Jika stageSelectionText di assign → pakai format "Halo, {name}"
        if (stageSelectionText)
            stageSelectionText.text = $"Halo, {name}";
    }

    private void OnDestroy()
    {
        if (UserManager.Instance != null)
            UserManager.Instance.OnUserChanged -= HandleUserChanged;
    }
}