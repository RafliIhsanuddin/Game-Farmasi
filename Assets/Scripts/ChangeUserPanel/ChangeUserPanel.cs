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

    [Header("Delete Button")]
    [SerializeField] private Button deleteButton;

    [Header("Add Panel")]
    [SerializeField] private PlayerInputPanel playerInputPanel;

    // Optional (sunnah)
    [Header("Delete Confirm Panel (Optional)")]
    [SerializeField] private GameObject confirmDeletePanel;
    [SerializeField] private TextMeshProUGUI confirmText;

    private string selectedUser = null;

    private void OnEnable()
    {
        RefreshUserList();
    }

    public void Show()
    {
        panel.SetActive(true);
        RefreshUserList();
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    private void RefreshUserList()
    {
        selectedUser = UserManager.Instance.CurrentUser;

        foreach (Transform child in content)
            Destroy(child.gameObject);

        foreach (var user in UserManager.Instance.Users)
        {
            var btnObj = Instantiate(userButtonPrefab, content);
            var txt = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            txt.text = user;

            var btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                selectedUser = user;
                UserManager.Instance.SetUser(user);
                RefreshUserList();
            });

            Highlight(btnObj, user == selectedUser);
        }
    }

    private void Highlight(GameObject obj, bool active)
    {
        var img = obj.GetComponent<Image>();
        if (img)
            img.color = active ? new Color(0.3f, 0.75f, 0.3f) : Color.white;
    }

    // BUTTON ADD
    public void OpenAddUser()
    {
        playerInputPanel.Show();
        Hide();
    }

    // BUTTON DELETE
    public void DeleteSelectedUser()
    {
        if (string.IsNullOrEmpty(selectedUser))
        {
            Debug.LogWarning("[ChangeUserPanel] tidak ada user terpilih");
            return;
        }

        bool success = UserManager.Instance.DeleteUser(selectedUser);

        if (!success)
            Debug.LogWarning("[ChangeUserPanel] delete gagal (fail-safe)");

        RefreshUserList();
    }
}
