using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class StageSelection : MonoBehaviour
{
    [Header("Stage Index (0 = Stage 1, 1 = Stage 2, ...)")]
    [SerializeField] private int stageIndex;

    [Header("Level Buttons (urut dari kiri/atas)")]
    [SerializeField] private List<Button> levelButtons;

    [Header("Optional Lock Icons")]
    [SerializeField] private List<GameObject> lockIcons;

    private void Start()
    {
        int unlockedGlobal = GameData.Data.UnlockedLevel;
        int[] levelsPerStage = GameData.Data.LevelsPerStage;

        LevelHelper.GetStageAndLevel(
            unlockedGlobal,
            levelsPerStage,
            out int unlockedStage,
            out int unlockedLevelInStage
        );

        for (int i = 0; i < levelButtons.Count; i++)
        {
            bool unlocked =
                stageIndex < unlockedStage ||
                (stageIndex == unlockedStage && i + 1 <= unlockedLevelInStage);

            levelButtons[i].interactable = unlocked;

            if (i < lockIcons.Count && lockIcons[i] != null)
                lockIcons[i].SetActive(!unlocked);
        }
    }

    public void OpenLevel(int levelIndex)
    {
        Time.timeScale = 1f;

        // Contoh naming scene: Stage1_Level1
        string sceneName = $"Stage{stageIndex + 1}_Level{levelIndex}";
        SceneManager.LoadScene(sceneName);
    }
}
