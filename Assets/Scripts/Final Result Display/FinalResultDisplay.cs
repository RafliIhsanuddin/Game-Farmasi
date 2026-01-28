using UnityEngine;
using TMPro;

public class FinalResultDisplay : MonoBehaviour
{
    [Header("Final Flags Text")]    [SerializeField] private TextMeshProUGUI finalFlagText;
    [Header("Final Nucleus Text")]  [SerializeField] private TextMeshProUGUI finalNucleusText;
    [Header("Final Suns Text")]     [SerializeField] private TextMeshProUGUI finalSunsText;
    [Header("Final Score Text")]    [SerializeField] private TextMeshProUGUI finalScoreText;
    [Header("Final Player Name Text")] [SerializeField] private TextMeshProUGUI finalPlayerNameText;
    [Header("Leaderboard Rank Text")] [SerializeField] private TextMeshProUGUI leaderboardRankText;

    private void Start()
    {
        CalculateScoreAndInsert();
        UpdateDisplay();
    }

    private void CalculateScoreAndInsert()
    {
        // ambil nama user dari UserManager
        string playerName = UserManager.Instance.CurrentUser;
        GameData.Data.PlayerName = playerName;

        // ambil data survival
        int flags   = GameData.Data.CurrentFlags;
        int suns    = GameData.Data.FinalSuns;
        int nucleus = GameData.Data.TotalActiveObjects;

        // hitung score (BENAR)
        int score = (nucleus * 3) + (suns * 1) + (flags * 10);
        GameData.Data.FinalScore = score;

        Debug.Log($"[FinalResultDisplay] Score = Flags:{flags}, Suns:{suns}, Nucleus:{nucleus}, Score:{score}");

        // insert ke leaderboard
        int rank = HighscoreTable.Insert(
            playerName,
            flags,
            suns,
            nucleus,
            score
        );

        GameData.Data.FinalRank = rank;
        Debug.Log($"[FinalResultDisplay] Leaderboard Rank = {rank+1}");
    }
    
    

    public void UpdateDisplay()
    {
        int flags   = GameData.Data.CurrentFlags;
        int nucleus = GameData.Data.TotalActiveObjects;
        int suns    = GameData.Data.FinalSuns;
        int score   = GameData.Data.FinalScore;
        int rank    = GameData.Data.FinalRank;

        if (finalFlagText)     finalFlagText.text     = $"total final flag : {flags}";
        if (finalNucleusText)  finalNucleusText.text  = $"total nucleus remain : {nucleus}";
        if (finalSunsText)     finalSunsText.text     = $"total suns earned : {suns}";
        if (finalScoreText)    finalScoreText.text    = $"total score : {score}";
        if (finalPlayerNameText) finalPlayerNameText.text = $"player : {GameData.Data.PlayerName}";

        if (leaderboardRankText)
        {
            string msg = $"skor di leaderboard yang di tampilkan hanya urutan 1 sampai 10 : kamu urutan ke-{rank + 1}";

            if (rank >= 10) msg += "\n(kamu tidak masuk 10 besar)";
            else msg += "\n(kamu masuk 10 besar)";

            leaderboardRankText.text = msg;
        }
    }
}
