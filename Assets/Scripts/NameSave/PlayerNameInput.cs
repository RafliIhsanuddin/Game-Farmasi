using UnityEngine;
using TMPro;

public class PlayerNameInput : MonoBehaviour
{
    [Header("Input Field")]
    [SerializeField] private TMP_InputField nameInputField;

    private const int maxChars = 15;

    public void SaveName()
    {
        if (nameInputField == null)
        {
            Debug.LogError("[PlayerNameInput] ERROR: nameInputField belum di-assign!");
            return;
        }

        string raw = nameInputField.text.Trim();
        if (raw.Length > maxChars)
            raw = raw.Substring(0, maxChars);

        // Save ke GameData
        GameData.Data.PlayerName = raw;

        Debug.Log($"[PlayerNameInput] SaveName DIPANGGIL → raw=\"{raw}\"");
        Debug.Log($"[PlayerNameInput] GameData.Data.PlayerName sekarang = \"{GameData.Data.PlayerName}\"");
    }
}