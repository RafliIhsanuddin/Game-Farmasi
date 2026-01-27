using UnityEngine;
using System.Collections.Generic;
using System;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance;

    private const string USERS_KEY = "Users";
    private const string LAST_USER_KEY = "LastUser";

    public List<string> Users = new();
    public string CurrentUser = "Player";

    public event Action<string> OnUserChanged; // notify welcome panel

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadUsers();
    }

    public void LoadUsers()
    {
        string raw = PlayerPrefs.GetString(USERS_KEY, "");
        if (!string.IsNullOrEmpty(raw))
        {
            Users = new List<string>(raw.Split('|'));
        }

        string lastUser = PlayerPrefs.GetString(LAST_USER_KEY, "");
        if (!string.IsNullOrEmpty(lastUser))
            CurrentUser = lastUser;

        // sync to GameData
        GameData.Data.PlayerName = CurrentUser;
    }

    public void SaveUsers()
    {
        string raw = string.Join("|", Users);
        PlayerPrefs.SetString(USERS_KEY, raw);
        PlayerPrefs.Save();
    }

    public void SetUser(string name)
    {
        CurrentUser = name;
        PlayerPrefs.SetString(LAST_USER_KEY, name);
        PlayerPrefs.Save();

        GameData.Data.PlayerName = name;

        Debug.Log($"[UserManager] CurrentUser changed to: {name}");

        OnUserChanged?.Invoke(name);
    }
}