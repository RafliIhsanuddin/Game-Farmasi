using UnityEngine;
using TMPro;

public class PlayerNameInput : MonoBehaviour
{
    [Header("Input Field")]
    [SerializeField] private TMP_InputField nameInputField;

    private const int maxChars = 15;

    public void SaveName()
    {
        if (nameInputField == null) return;

        // === Simpan nama ===
        string raw = nameInputField.text.Trim();
        if (raw.Length > maxChars)
            raw = raw.Substring(0, maxChars);

        GameData.Data.PlayerName = raw;
        Debug.Log($"[PlayerNameInput] Saved player name: {raw}");

        // === Ambil data akhir survival untuk score ===
        int flags   = GameData.Data.CurrentFlags;
        int suns    = GameData.Data.FinalSuns;
        int nucleus = GameData.Data.TotalActiveObjects;

        // === HITUNG SCORE (BENAR) ===
        int score = (nucleus * 3) + (suns * 1) + (flags * 10);
        GameData.Data.FinalScore = score;

        Debug.Log($"[PlayerNameInput] Score Calculated | Flags={flags}, Suns={suns}, Nucleus={nucleus}, Score={score}");

        // === INSERT SCORE KE LEADERBOARD ===
        int rank = HighscoreTable.InsertAndGetRank(
            GameData.Data.PlayerName,
            flags,
            suns,
            nucleus,
            score
        );

        GameData.Data.FinalRank = rank;

        Debug.Log($"[PlayerNameInput] Inserted to Leaderboard | Rank={rank + 1}");
    }
}