using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class HighscoreTable : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;
    private List<HighscoreEntry> entryList;
    private List<Transform> entryTransformList;

    private const string PREF_KEY = "HighscoreTable";

    private void Awake()
    {
        entryContainer = transform.Find("HighscoreEntryContainer");
        entryTemplate = entryContainer.Find("HighScoreEntryTemplate");

        if (entryTemplate == null)
        {
            Debug.LogError("[HighscoreTable] Tidak menemukan 'HighScoreEntryTemplate'!");
            return;
        }

        entryTemplate.gameObject.SetActive(false);
        LoadAndDisplay();
    }

    private void LoadAndDisplay()
    {
        entryList = LoadEntries();

        entryList.Sort((a, b) => b.score.CompareTo(a.score));

        int displayCount = Mathf.Min(10, entryList.Count);
        entryTransformList = new List<Transform>();

        for (int i = 0; i < displayCount; i++)
        {
            CreateEntry(entryList[i], entryContainer, entryTransformList);
        }
    }

    private List<HighscoreEntry> LoadEntries()
    {
        string json = PlayerPrefs.GetString(PREF_KEY, "");
        Highscores highscores = JsonUtility.FromJson<Highscores>(json);

        if (highscores == null || highscores.list == null)
            return new List<HighscoreEntry>();

        return highscores.list;
    }

    private void CreateEntry(HighscoreEntry entry, Transform container, List<Transform> list)
    {
        float height = 40f;

        Transform t = Instantiate(entryTemplate, container);
        RectTransform rt = t.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, -height * list.Count);
        t.gameObject.SetActive(true);

        int rank = list.Count + 1;
        string rankString = rank switch
        {
            1 => "1ST",
            2 => "2ND",
            3 => "3RD",
            _ => rank + "TH"
        };

        t.Find("Pos").GetComponent<TextMeshProUGUI>().text = rankString;
        t.Find("Name").GetComponent<TextMeshProUGUI>().text = entry.name;
        t.Find("Flag").GetComponent<TextMeshProUGUI>().text = entry.flag.ToString();
        t.Find("Sun").GetComponent<TextMeshProUGUI>().text = entry.sun.ToString();
        t.Find("Nucleus").GetComponent<TextMeshProUGUI>().text = entry.nucleus.ToString();
        t.Find("Score").GetComponent<TextMeshProUGUI>().text = entry.score.ToString();

        list.Add(t);
    }

    public static int Insert(string name, int flag, int sun, int nucleus, int score)
    {
        string json = PlayerPrefs.GetString(PREF_KEY, "");
        Highscores highscores = JsonUtility.FromJson<Highscores>(json);

        if (highscores == null || highscores.list == null)
            highscores = new Highscores() { list = new List<HighscoreEntry>() };

        HighscoreEntry entry = new HighscoreEntry(name, flag, sun, nucleus, score);
        highscores.list.Add(entry);

        highscores.list.Sort((a, b) => b.score.CompareTo(a.score));

        int rank = highscores.list.FindIndex(x =>
            x.name == name &&
            x.flag == flag &&
            x.sun == sun &&
            x.nucleus == nucleus &&
            x.score == score
        );

        json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString(PREF_KEY, json);
        PlayerPrefs.Save();

        Debug.Log($"[HighscoreTable] Insert Score={score} Rank={rank + 1}");
        return rank;
    }

    public static void ResetGlobal()
    {
        PlayerPrefs.DeleteKey(PREF_KEY);
        PlayerPrefs.Save();
        Debug.Log($"[HighscoreTable] ALL SCORE RESET!");
    }

    // =====================================================
    // FUNGSI UNTUK RESET BUTTON (NON STATIC)
    // =====================================================
    public void ResetScoresFromButton()
    {
        Debug.Log("[HighscoreTable] ResetScoresFromButton() dipanggil");

        // reset data
        ResetGlobal();

        // hapus semua entry UI yang sudah ada
        foreach (Transform child in entryContainer)
        {
            if (child != entryTemplate)
                Destroy(child.gameObject);
        }

        // reload (akan kosong)
        LoadAndDisplay();
    }

    public static void DeleteScoresByName(string name)
    {
        string json = PlayerPrefs.GetString(PREF_KEY, "");
        Highscores highscores = JsonUtility.FromJson<Highscores>(json);

        if (highscores == null || highscores.list == null)
            return;

        int before = highscores.list.Count;

        highscores.list.RemoveAll(e => e.name == name);

        int after = highscores.list.Count;
        int removed = before - after;

        json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString(PREF_KEY, json);
        PlayerPrefs.Save();

        Debug.Log($"[HighscoreTable] Removed {removed} score entries milik '{name}'");
    }

    [System.Serializable]
    public class Highscores
    {
        public List<HighscoreEntry> list;
    }

    [System.Serializable]
    public class HighscoreEntry
    {
        public string name;
        public int flag;
        public int sun;
        public int nucleus;
        public int score;

        public HighscoreEntry(string name, int flag, int sun, int nucleus, int score)
        {
            this.name = name;
            this.flag = flag;
            this.sun = sun;
            this.nucleus = nucleus;
            this.score = score;
        }
    }
}