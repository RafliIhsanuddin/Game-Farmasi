using UnityEngine;

public class GameData
{
    public static GameData Data = new GameData();

    // --- Adventure Progression ---
    public int UnlockedLevel = 1;
    public int[] LevelsPerStage = { 6, 1 };

    // ============================
    // ENDLESS MODE DATA
    // ============================

    // Last Run
    public int LastCycle = 0;
    public int LastFlags = 0;
    public int LastEndlessScore = 0;

    // Best Run
    public int BestCycle = 0;
    public int BestFlags = 0;
    public int BestEndlessScore = 0;

    public void UpdateEndlessRun(int cycle, int flags)
    {
        int score = cycle * 2 + flags;

        LastCycle = cycle;
        LastFlags = flags;
        LastEndlessScore = score;

        bool improved = false;

        if (cycle > BestCycle) { BestCycle = cycle; improved = true; }
        if (flags > BestFlags) { BestFlags = flags; improved = true; }
        if (score > BestEndlessScore) { BestEndlessScore = score; improved = true; }

        Debug.Log($"[GameData] Endless Run Updated | Last: C={cycle}, F={flags}, S={score} | BestImproved={improved}");
    }

    // ============================
    // SURVIVAL DATA (NEW)
    // ============================
    public int TotalActiveObjects = 0;   // nucleus
    public int FinalSuns = 0;
    public int CurrentFlags = 0;

    // ============================
    // PLAYER NAME (LEADERBOARD)
    // ============================
    public string PlayerName = "";

    // ============================
    // FINAL SCORE & RANK
    // ============================
    public int FinalScore = 0;
    public int FinalRank = -1; // 0-based
}