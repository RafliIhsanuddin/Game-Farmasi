using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelMenu : MonoBehaviour
{

    [System.Serializable]
    public class LevelButtonData
    {
        public Button button;

        [Tooltip("Opsional: GameObject gembok / lock icon")]
        public GameObject lockObject;
    }

    [Header("Level Buttons")]
    [SerializeField] private List<LevelButtonData> levels = new List<LevelButtonData>();

    private void Awake()
    {
        int unlockedLevel = GameData.Data.UnlockedLevel;

        for (int i = 0; i < levels.Count; i++)
        {
            bool isUnlocked = i < unlockedLevel;

            // Button state
            if (levels[i].button != null)
                levels[i].button.interactable = isUnlocked;

            // Lock icon
            if (levels[i].lockObject != null)
                levels[i].lockObject.SetActive(!isUnlocked);
        }
    }

    public void OpenLevel(int levelId)
    {
        string levelName = "Level" + levelId;
        SceneManager.LoadScene(levelName);
    }
    
    
}
