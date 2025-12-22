using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Data = new GameData();

    public int CurrentScore = 0;
    public int HighScore = 0;

    // 🔓 Level unlock (minimal level 1 terbuka)
    public int UnlockedLevel = 1;
}
