using UnityEngine;
using TMPro;
using System;

public class PlayerInputPanel : MonoBehaviour
{
    [Header("Root Panel")]
    [SerializeField] private GameObject panelRoot;

    [Header("Input")]
    [SerializeField] private TMP_InputField nameInputField;

    [Header("Feedback")]
    [SerializeField] private TextMeshProUGUI feedbackText;

    private const int maxChars = 15;

    public Action OnUserAdded;
    
    private void OnEnable()
    {
        if (feedbackText) feedbackText.text = "";
    }

    public void Show()
    {
        panelRoot.SetActive(true);
        if (feedbackText) feedbackText.text = "";
        if (nameInputField) nameInputField.text = "";
    }

    public void Hide()
    {
        panelRoot.SetActive(false);
    }

    public void SubmitUser()
    {
        string raw = nameInputField.text.Trim();

        if (string.IsNullOrEmpty(raw))
        {
            ShowFeedback("nama tidak boleh kosong!", Color.red);
            return;
        }

        if (raw.Length > maxChars)
            raw = raw.Substring(0, maxChars);

        // duplikat user
        if (UserManager.Instance.Users.Contains(raw))
        {
            ShowFeedback($"user \"{raw}\" sudah ada!", Color.red);
            return;
        }

        // add user
        UserManager.Instance.Users.Add(raw);
        UserManager.Instance.SaveUsers();

        // auto switch
        UserManager.Instance.SetUser(raw);

        ShowFeedback($"user \"{raw}\" telah masuk!", Color.green);

        // reset input
        nameInputField.text = "";

        // notify panel lain
        OnUserAdded?.Invoke();
    }

    private void ShowFeedback(string msg, Color c)
    {
        feedbackText.text = msg;
        feedbackText.color = c;
    }

    public void Cancel()
    {
        Hide();
    }
}