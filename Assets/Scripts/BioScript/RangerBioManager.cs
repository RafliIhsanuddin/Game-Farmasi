using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RangerBioManager : MonoBehaviour
{
    [System.Serializable]
    public class CharacterData
    {
        public GameObject pictureObj;
        public GameObject button;

        public string name;
        [TextArea(3, 6)] public string description;
    }

    [Header("Display Slot (UI Output)")]
    [SerializeField] private TMP_Text slotName;
    [SerializeField] private TMP_Text slotDescription;

    [Header("Characters Data (Fill in Inspector)")]
    [SerializeField] private CharacterData White;
    [SerializeField] private CharacterData Blue;
    [SerializeField] private CharacterData Pink;
    [SerializeField] private CharacterData Red;
    [SerializeField] private CharacterData Green;
    [SerializeField] private CharacterData Yellow;

    private CharacterData[] allCharacters;

    private void Start()
    {
        allCharacters = new CharacterData[] { White, Blue, Pink, Red, Green, Yellow };
        ShowWhite();
    }

    private void ApplyCharacter(CharacterData data)
    {
        if (data == null) return;

        // matikan semua picture + button
        foreach (var c in allCharacters)
        {
            if (c != null)
            {
                if (c.pictureObj != null) c.pictureObj.SetActive(false);
                if (c.button != null) c.button.SetActive(false);
            }
        }

        // aktifkan yang dipilih
        if (data.pictureObj != null) data.pictureObj.SetActive(true);
        if (data.button != null) data.button.SetActive(true);

        // ubah text
        if (slotName != null) slotName.text = data.name;
        if (slotDescription != null) slotDescription.text = data.description;
    }

    // ===== BUTTON CALLS =====

    public void ShowWhite() => ApplyCharacter(White);
    public void ShowBlue() => ApplyCharacter(Blue);
    public void ShowPink() => ApplyCharacter(Pink);
    public void ShowRed() => ApplyCharacter(Red);
    public void ShowGreen() => ApplyCharacter(Green);
    public void ShowYellow() => ApplyCharacter(Yellow);
}
