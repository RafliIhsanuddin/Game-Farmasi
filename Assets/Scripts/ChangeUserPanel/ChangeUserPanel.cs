using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeUserPanel : MonoBehaviour
{
    [Header("Panel Root")]
    [SerializeField] private GameObject panel;

    [Header("Scroll Content")]
    [SerializeField] private Transform content;

    [Header("User Button Prefab")]
    [SerializeField] private GameObject userButtonPrefab;

    [Header("Open Add Panel (PlayerInputPanel)")]
    [SerializeField] private PlayerInputPanel playerInputPanel;

    private void OnEnable()
    {
        RefreshUserList();
    }

    // =============================================================
    // SHOW / HIDE
    // =============================================================
    public void Show()
    {
        panel.SetActive(true);
        RefreshUserList();
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    // =============================================================
    // REFRESH LIST USERS
    // =============================================================
    private void RefreshUserList()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);

        foreach (var user in UserManager.Instance.Users)
        {
            var btn = Instantiate(userButtonPrefab, content);

            var txt = btn.GetComponentInChildren<TextMeshProUGUI>();
            txt.text = user;

            var button = btn.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                UserManager.Instance.SetUser(user);
                Debug.Log($"[ChangeUserPanel] Selected user: {user}");
                RefreshUserList(); // highlight refresh
            });

            HighlightActive(btn, user == UserManager.Instance.CurrentUser);
        }
    }

    // =============================================================
    // OPTIONAL HIGHLIGHT ACTIVE USER
    // =============================================================
    private void HighlightActive(GameObject buttonObj, bool active)
    {
        var img = buttonObj.GetComponent<Image>();
        if (img)
            img.color = active ? new Color(0.2f, 0.8f, 0.2f) : Color.white;
    }

    // =============================================================
    // ADD USER BUTTON (OPEN INPUT PANEL)
    // =============================================================
    public void OpenAddUserPanel()
    {
        //playerInputPanel.ShowAdd();
        Hide();
    }
}
