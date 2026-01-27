using UnityEngine;
using TMPro;

public class WelcomeMenuController : MonoBehaviour
{
    [Header("Welcome Text (Main Menu) - Sunnah (Opsional)")]
    [SerializeField] private TextMeshProUGUI welcomeText;

    [Header("Stage Selection Text - Sunnah (Opsional)")]
    [SerializeField] private TextMeshProUGUI stageSelectionText;

    private void Start()
    {
        // Kalau tidak ada UserManager tidak usah pusing
        if (UserManager.Instance == null)
            return;

        Refresh();
        UserManager.Instance.OnUserChanged += HandleUserChanged;
    }

    private void HandleUserChanged(string newUser)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (UserManager.Instance == null)
            return;

        string name = UserManager.Instance.CurrentUser;

        // Sunnah 1
        if (welcomeText)
            welcomeText.text = $"{name}";

        // Sunnah 2
        if (stageSelectionText)
            stageSelectionText.text = $"Halo, {name}";
    }

    private void OnDestroy()
    {
        if (UserManager.Instance != null)
            UserManager.Instance.OnUserChanged -= HandleUserChanged;
    }
}