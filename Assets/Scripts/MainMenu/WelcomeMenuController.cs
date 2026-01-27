using UnityEngine;
using TMPro;

public class WelcomeMenuController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI welcomeText;

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

        if (welcomeText)
            welcomeText.text = $"{name}";
    }

    private void OnDestroy()
    {
        if (UserManager.Instance != null)
            UserManager.Instance.OnUserChanged -= HandleUserChanged;
    }
}
