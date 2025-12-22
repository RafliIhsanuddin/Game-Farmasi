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
        public GameObject lockObject;
    }

    [Header("Level Buttons")]
    [SerializeField] private List<LevelButtonData> levels = new List<LevelButtonData>();

    private void Awake()
    {
        // 🔒 Clamp = pengaman supaya tidak semua kebuka
        int unlockedLevel = Mathf.Clamp(
            GameData.Data.UnlockedLevel,
            1,
            levels.Count
        );

        for (int i = 0; i < levels.Count; i++)
        {
            bool isUnlocked = i < unlockedLevel;

            if (levels[i].button != null)
                levels[i].button.interactable = isUnlocked;

            if (levels[i].lockObject != null)
                levels[i].lockObject.SetActive(!isUnlocked);
        }
    }

    public void OpenLevel(int levelId)
    {
        SceneManager.LoadScene("Level" + levelId);
    }
    
}
