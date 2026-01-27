using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeUserPanel : MonoBehaviour
{
    [Header("Root Panel")]
    [SerializeField] private GameObject panelRoot;

    [Header("Scroll Content")]
    [SerializeField] private Transform content;

    [Header("User Button Prefab")]
    [SerializeField] private GameObject userButtonPrefab;

    [Header("Player Input Panel")]
    [SerializeField] private PlayerInputPanel playerInputPanel;

    [Header("Delete Confirm Panel")]
    [SerializeField] private GameObject confirmDeletePanel;
    [SerializeField] private TextMeshProUGUI confirmText;

    [Header("Choose Button")]
    [SerializeField] private Button chooseButton;

    [Header("Delete Button")]
    [SerializeField] private Button deleteButton;

    private string selectedUser = null;

    private void OnEnable()
    {
        Refresh();
    }

    public void Show()
    {
        panelRoot.SetActive(true);
        Refresh();
    }

    public void Hide()
    {
        panelRoot.SetActive(false);
    }

    public void Refresh()
    {
        selectedUser = null;

        foreach (Transform c in content)
            Destroy(c.gameObject);

        foreach (var user in UserManager.Instance.Users)
        {
            var obj = Instantiate(userButtonPrefab, content);
            var txt = obj.GetComponentInChildren<TextMeshProUGUI>();
            txt.text = user;

            var button = obj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                selectedUser = user;
                UpdateButtonsState();   // update choose & delete visibility
                RefreshHighlight();
            });

            Highlight(obj, user == selectedUser);
        }

        UpdateButtonsState();

        if (confirmDeletePanel) confirmDeletePanel.SetActive(false);
    }

    private void RefreshHighlight()
    {
        foreach (Transform c in content)
        {
            var txt = c.GetComponentInChildren<TextMeshProUGUI>();
            Highlight(c.gameObject, txt.text == selectedUser);
        }
    }

    private void Highlight(GameObject obj, bool active)
    {
        var img = obj.GetComponent<Image>();
        if (img)
            img.color = active ? new Color(0.3f, 1f, 0.3f) : Color.white;
    }

    // ====== BUTTON LOGIC ======

    private void UpdateButtonsState()
    {
        // Choose
        chooseButton.gameObject.SetActive(selectedUser != null);

        // Delete (mode B hidden)
        if (selectedUser == null)
        {
            deleteButton.gameObject.SetActive(false);
            return;
        }

        bool canDelete = UserManager.Instance.CanDeleteUser(selectedUser);
        deleteButton.gameObject.SetActive(canDelete);
    }

    public void OnChooseUser()
    {
        if (selectedUser == null) return;

        UserManager.Instance.SetUser(selectedUser);

        Debug.Log($"[ChangeUserPanel] Chosen user: {selectedUser}");

        RefreshHighlight();
    }

    public void RequestDeleteUser()
    {
        if (selectedUser == null) return;

        if (!UserManager.Instance.CanDeleteUser(selectedUser))
        {
            Debug.Log($"[ChangeUserPanel] Tidak bisa hapus user: {selectedUser}");
            return;
        }

        confirmText.text = $"Hapus user \"{selectedUser}\" ?";
        confirmDeletePanel.SetActive(true);
    }

    public void ConfirmDeleteYes()
    {
        if (selectedUser != null)
            UserManager.Instance.DeleteUser(selectedUser);

        confirmDeletePanel.SetActive(false);
        Refresh();
    }

    public void ConfirmDeleteNo()
    {
        confirmDeletePanel.SetActive(false);
    }

    public void OpenAddUserPanel()
    {
        playerInputPanel.Show();
        playerInputPanel.OnUserAdded = () =>
        {
            Refresh();
        };
    }
}
