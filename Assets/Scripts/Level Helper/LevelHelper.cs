using UnityEngine;

public class LevelHelper
{
    public static void GetStageAndLevel(
        int globalLevel,
        int[] levelsPerStage,
        out int stageIndex,
        out int levelIndex
    )
    {
        int remaining = globalLevel;

        for (int i = 0; i < levelsPerStage.Length; i++)
        {
            if (remaining <= levelsPerStage[i])
            {
                stageIndex = i;          // 0-based
                levelIndex = remaining; // 1-based
                return;
            }

            remaining -= levelsPerStage[i];
        }

        // fallback kalau semua stage sudah selesai
        stageIndex = levelsPerStage.Length - 1;
        levelIndex = levelsPerStage[^1];
    }
}
