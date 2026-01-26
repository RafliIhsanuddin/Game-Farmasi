using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class HighscoreTable : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;
    private List<HighscoreEntry> highscoreEntryList;
    private List<Transform> highscoreEntryTransformList;

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

        // Dummy testing leaderboard (Top 5)
        /*
        highscoreEntryList = new List<HighscoreEntry>()
        {
            new HighscoreEntry("RAF", 5, 120, 80),
            new HighscoreEntry("KEN", 2,  90, 50),
            new HighscoreEntry("JOE", 1, 150, 70),
            new HighscoreEntry("MAX", 3, 110, 40),
            new HighscoreEntry("RYU", 12, 200, 20),
        };
        */

        // ============================
        // LOAD PLAYER PREFS
        // ============================
        string jsonString = PlayerPrefs.GetString("HighscoreTable", "");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        // Null handling
        if (highscores == null || highscores.highscoreEntryList == null)
        {
            highscoreEntryList = new List<HighscoreEntry>();

            Debug.Log("[HighscoreTable] Tidak ada data leaderboard (pertama kali run)");
        }
        else
        {
            highscoreEntryList = highscores.highscoreEntryList;
        }

        // Sort score DESC (tinggi ke rendah)
        highscoreEntryList.Sort((a, b) => b.score.CompareTo(a.score));

        // Display
        highscoreEntryTransformList = new List<Transform>();
        foreach (HighscoreEntry entry in highscoreEntryList)
        {
            CreateHighscoreEntryTransform(entry, entryContainer, highscoreEntryTransformList);
        }

        // ============================
        // SAVE PLAYER PREFS (COMMENTED)
        // ============================
        /*
        Highscores highscores = new Highscores { highscoreEntryList = highscoreEntryList };
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("HighscoreTable", json);
        PlayerPrefs.Save();
        Debug.Log(PlayerPrefs.GetString("HighscoreTable"));
        */
    }

    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 40f;

        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        // Rank / Pos Display
        int rank = transformList.Count + 1;
        string rankString = rank switch
        {
            1 => "1ST",
            2 => "2ND",
            3 => "3RD",
            _ => rank + "TH"
        };

        // Data from entry (NO random)
        string name = highscoreEntry.name;
        int flag = highscoreEntry.flag;
        int sun = highscoreEntry.sun;
        int nucleus = highscoreEntry.nucleus;
        int score = highscoreEntry.score;

        // Ambil TMP Fields
        var posText     = entryTransform.Find("Pos").GetComponent<TextMeshProUGUI>();
        var nameText    = entryTransform.Find("Name").GetComponent<TextMeshProUGUI>();
        var flagText    = entryTransform.Find("Flag").GetComponent<TextMeshProUGUI>();
        var sunText     = entryTransform.Find("Sun").GetComponent<TextMeshProUGUI>();
        var nucleusText = entryTransform.Find("Nucleus").GetComponent<TextMeshProUGUI>();
        var scoreText   = entryTransform.Find("Score").GetComponent<TextMeshProUGUI>();

        // Display Data
        posText.text     = rankString;
        nameText.text    = name;
        flagText.text    = flag.ToString();
        sunText.text     = sun.ToString();
        nucleusText.text = nucleus.ToString();
        scoreText.text   = score.ToString();

        transformList.Add(entryTransform);
    }

    private void AddHighscoreEntry(string name, int flag, int sun, int nucleus)
    {
        // Entry baru (score di-generate otomatis)
        HighscoreEntry newEntry = new HighscoreEntry(name, flag, sun, nucleus);

        // Load JSON
        string jsonString = PlayerPrefs.GetString("HighscoreTable", "");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        // First-run handling
        if (highscores == null || highscores.highscoreEntryList == null)
        {
            highscores = new Highscores();
            highscores.highscoreEntryList = new List<HighscoreEntry>();
        }

        // Tambah entry baru
        highscores.highscoreEntryList.Add(newEntry);

        // Sorting DESC by score
        highscores.highscoreEntryList.Sort((a, b) => b.score.CompareTo(a.score));

        // Save kembali
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("HighscoreTable", json);
        PlayerPrefs.Save();
    }

    [System.Serializable]
    public class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList;
    }

    [System.Serializable]
    public class HighscoreEntry
    {
        public string name;
        public int flag;
        public int sun;
        public int nucleus;
        public int score;

        public HighscoreEntry(string name, int flag, int sun, int nucleus)
        {
            this.name = name;
            this.flag = flag;
            this.sun = sun;
            this.nucleus = nucleus;

            // Score formula survival endless (skala 0–1000)
            score = (nucleus * 3) + (sun * 1) + (flag * 10);
        }
    }
}
