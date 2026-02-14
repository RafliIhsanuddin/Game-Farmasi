using UnityEngine;
using TMPro;

public class FinalResultDisplay : MonoBehaviour
{
    [Header("Final Flags Text")]
    [SerializeField] private TextMeshProUGUI finalFlagText;

    [Header("Final Nucleus Text")]
    [SerializeField] private TextMeshProUGUI finalNucleusText;

    [Header("Final Suns Text")]
    [SerializeField] private TextMeshProUGUI finalSunsText;

    [Header("Final Score Text")]
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Header("Final Player Name Text")]
    [SerializeField] private TextMeshProUGUI finalPlayerNameText;

    [Header("Leaderboard Rank Text")]
    [SerializeField] private TextMeshProUGUI leaderboardRankText;

    private void Start()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        int flags   = GameData.Data.CurrentFlags;
        int nucleus = GameData.Data.TotalActiveObjects;
        int suns    = GameData.Data.FinalSuns;
        int score   = GameData.Data.FinalScore;
        int rank    = GameData.Data.FinalRank;

        // ✅ Sumber nama utama: UserManager.CurrentUser
        // fallback: GameData.Data.PlayerName
        string playerName =
            (UserManager.Instance != null && !string.IsNullOrEmpty(UserManager.Instance.CurrentUser))
                ? UserManager.Instance.CurrentUser
                : (!string.IsNullOrEmpty(GameData.Data.PlayerName) ? GameData.Data.PlayerName : "Player");

        if (finalFlagText)
            finalFlagText.text = $"total final flag : {flags}";

        if (finalNucleusText)
            finalNucleusText.text = $"total nucleus remain : {nucleus}";

        if (finalSunsText)
            finalSunsText.text = $"total suns earned : {suns}";

        if (finalScoreText)
            finalScoreText.text = $"total score : {score}";

        if (finalPlayerNameText)
            finalPlayerNameText.text = $"player : {playerName}";

        if (leaderboardRankText)
        {
            // rank biasanya 0-based dari HighscoreTable.Insert()
            // Kalau rank belum pernah di-set (misal -1), jangan bikin pesan "ke-0"
            if (rank < 0)
            {
                leaderboardRankText.text =
                    "skor di leaderboard yang di tampilkan hanya urutan 1 sampai 10 : (rank belum tersedia)";
            }
            else
            {
                string msg = $"skor di leaderboard yang di tampilkan hanya urutan 1 sampai 10 : kamu urutan ke-{rank + 1}";

                if (rank >= 10)
                    msg += "\n(kamu tidak masuk 10 besar)";
                else
                    msg += "\n(kamu masuk 10 besar)";

                leaderboardRankText.text = msg;
            }
        }
    }
}
