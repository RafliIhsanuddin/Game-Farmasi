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
    [SerializeField] private TextMeshProUGUI finalPlayerNameText; // NEW

    private void Start()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        // Ambil data dari GameData
        int flags   = GameData.Data.CurrentFlags;
        int nucleus = GameData.Data.TotalActiveObjects;
        int suns    = GameData.Data.FinalSuns;

        // Display value
        if (finalFlagText != null)
            finalFlagText.text = $"total final flag : {flags}";

        if (finalNucleusText != null)
            finalNucleusText.text = $"total nucleus remain : {nucleus}";

        if (finalSunsText != null)
            finalSunsText.text = $"total suns earned : {suns}";

        // ==== SCORE CALCULATION ====
        int score = (flags * 150) + (nucleus * 50) + (suns * 1);

        // Display Score
        if (finalScoreText != null)
            finalScoreText.text = $"total score : {score}";

        // ==== SAVE SCORE FOR LEADERBOARD (NEW) ====
        GameData.Data.FinalScore = score;

        // ==== DISPLAY PLAYER NAME (NEW) ====
        string playerName = GameData.Data.PlayerName;
        if (finalPlayerNameText != null)
            finalPlayerNameText.text = $"player : {playerName}";
    }
}