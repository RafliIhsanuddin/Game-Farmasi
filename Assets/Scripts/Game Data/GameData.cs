using UnityEngine;

public class GameData
{
    public static GameData Data = new GameData();

    // Global linear progression
    public int UnlockedLevel = 1;

    // Jumlah level per stage (URUT)
    // Stage 1 = 4 level
    // Stage 2 = 2 level
    public int[] LevelsPerStage = { 4, 2 };
    
}
