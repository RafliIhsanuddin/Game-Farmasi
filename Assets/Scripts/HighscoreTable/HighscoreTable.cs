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

        // Load PlayerPrefs
        string jsonString = PlayerPrefs.GetString("HighscoreTable", "");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null || highscores.highscoreEntryList == null)
        {
            highscoreEntryList = new List<HighscoreEntry>();
        }
        else
        {
            highscoreEntryList = highscores.highscoreEntryList;
        }

        // Sort DESC
        highscoreEntryList.Sort((a, b) => b.score.CompareTo(a.score));

        // Display only TOP 10
        int displayCount = Mathf.Min(10, highscoreEntryList.Count);

        highscoreEntryTransformList = new List<Transform>();
        for (int i = 0; i < displayCount; i++)
        {
            CreateHighscoreEntryTransform(highscoreEntryList[i], entryContainer, highscoreEntryTransformList);
        }
    }

    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 40f;

        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString = rank switch
        {
            1 => "1ST",
            2 => "2ND",
            3 => "3RD",
            _ => rank + "TH"
        };

        var posText     = entryTransform.Find("Pos").GetComponent<TextMeshProUGUI>();
        var nameText    = entryTransform.Find("Name").GetComponent<TextMeshProUGUI>();
        var flagText    = entryTransform.Find("Flag").GetComponent<TextMeshProUGUI>();
        var sunText     = entryTransform.Find("Sun").GetComponent<TextMeshProUGUI>();
        var nucleusText = entryTransform.Find("Nucleus").GetComponent<TextMeshProUGUI>();
        var scoreText   = entryTransform.Find("Score").GetComponent<TextMeshProUGUI>();

        posText.text     = rankString;
        nameText.text    = highscoreEntry.name;
        flagText.text    = highscoreEntry.flag.ToString();
        sunText.text     = highscoreEntry.sun.ToString();
        nucleusText.text = highscoreEntry.nucleus.ToString();
        scoreText.text   = highscoreEntry.score.ToString();

        transformList.Add(entryTransform);
    }

    // ==== INSERT + RANK (LOGIKA ARCADE) ====
    public static int InsertAndGetRank(string name, int flag, int sun, int nucleus, int score)
    {
        string jsonString = PlayerPrefs.GetString("HighscoreTable", "");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null || highscores.highscoreEntryList == null)
        {
            highscores = new Highscores();
            highscores.highscoreEntryList = new List<HighscoreEntry>();
        }

        HighscoreEntry newEntry = new HighscoreEntry(name, flag, sun, nucleus, score);
        highscores.highscoreEntryList.Add(newEntry);

        highscores.highscoreEntryList.Sort((a, b) => b.score.CompareTo(a.score));

        int rank = highscores.highscoreEntryList.FindIndex(e =>
            e.name == name && e.score == score);

        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("HighscoreTable", json);
        PlayerPrefs.Save();

        return rank; // 0-based
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
