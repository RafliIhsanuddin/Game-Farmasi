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

    public event Action<string> OnUserChanged;

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

        // fail-safe: minimal 1 user
        if (Users.Count == 0)
        {
            Users.Add("Player");
            SaveUsers();
        }

        // sinkron ke GameData
        GameData.Data.PlayerName = CurrentUser;
    }

    public void LoadUsers()
    {
        string raw = PlayerPrefs.GetString(USERS_KEY, "");
        if (!string.IsNullOrEmpty(raw))
            Users = new List<string>(raw.Split('|'));
        else
            Users = new List<string>() { "Player" };

        string lastUser = PlayerPrefs.GetString(LAST_USER_KEY, "");
        if (!string.IsNullOrEmpty(lastUser) && Users.Contains(lastUser))
            CurrentUser = lastUser;
        else
            CurrentUser = "Player";

        GameData.Data.PlayerName = CurrentUser;
    }

    public void SaveUsers()
    {
        string raw = string.Join("|", Users);
        PlayerPrefs.SetString(USERS_KEY, raw);
        PlayerPrefs.Save();
    }

    public bool SetUser(string name)
    {
        if (!Users.Contains(name))
        {
            Debug.LogWarning($"[UserManager] Cannot set user '{name}' karena tidak ada!");
            return false;
        }

        CurrentUser = name;
        PlayerPrefs.SetString(LAST_USER_KEY, name);
        PlayerPrefs.Save();

        GameData.Data.PlayerName = name;

        Debug.Log($"[UserManager] CurrentUser changed to: {name}");

        OnUserChanged?.Invoke(name);
        return true;
    }

    // digunakan ChangeUserPanel untuk enable delete atau tidak
    public bool CanDeleteUser(string name)
    {
        if (Users.Count <= 1) return false;      // minimal 1 user harus ada
        if (name == "Player") return false;      // Player = fail-safe default
        return true;
    }

    public bool DeleteUser(string name)
    {
        if (!CanDeleteUser(name))
        {
            Debug.LogWarning($"[UserManager] DeleteUser blocked untuk: {name}");
            return false;
        }

        bool removed = Users.Remove(name);
        if (!removed) return false;

        SaveUsers();

        if (name == CurrentUser)
        {
            // switch fallback user
            CurrentUser = Users[0];
            PlayerPrefs.SetString(LAST_USER_KEY, CurrentUser);
            PlayerPrefs.Save();

            GameData.Data.PlayerName = CurrentUser;
            OnUserChanged?.Invoke(CurrentUser);

            Debug.Log($"[UserManager] ActiveUser fallback ke: {CurrentUser}");
        }

        Debug.Log($"[UserManager] Deleted user: {name}");
        return true;
    }
}
