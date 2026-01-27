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

        // fail-safe: jika list kosong → tambahkan "Player"
        if (Users.Count == 0)
        {
            Users.Add("Player");
            SaveUsers();
        }
    }

    public void LoadUsers()
    {
        string raw = PlayerPrefs.GetString(USERS_KEY, "");
        if (!string.IsNullOrEmpty(raw))
            Users = new List<string>(raw.Split('|'));

        string lastUser = PlayerPrefs.GetString(LAST_USER_KEY, "");
        if (!string.IsNullOrEmpty(lastUser))
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

    public void SetUser(string name)
    {
        CurrentUser = name;
        PlayerPrefs.SetString(LAST_USER_KEY, name);
        PlayerPrefs.Save();

        GameData.Data.PlayerName = name;

        Debug.Log($"[UserManager] CurrentUser changed to: {name}");

        OnUserChanged?.Invoke(name);
    }

    public bool DeleteUser(string name)
    {
        // fail-safe: tidak boleh hapus user terakhir
        if (Users.Count <= 1)
        {
            Debug.LogWarning("[UserManager] Gagal delete karena minimal 1 user harus ada!");
            return false;
        }

        // tidak boleh hapus user terakhir yang sedang aktif (kalau cuma 1)
        if (Users.Count == 1 && name == CurrentUser)
            return false;

        bool removed = Users.Remove(name);
        if (!removed)
            return false;

        SaveUsers();

        // jika user terhapus adalah activeUser → switch ke user pertama
        if (name == CurrentUser)
        {
            CurrentUser = Users[0];
            PlayerPrefs.SetString(LAST_USER_KEY, CurrentUser);
            PlayerPrefs.Save();

            GameData.Data.PlayerName = CurrentUser;
            OnUserChanged?.Invoke(CurrentUser);
        }

        Debug.Log($"[UserManager] Deleted user: {name}");
        return true;
    }
}
