using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyBacteriaBioManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemyData
    {
        public GameObject pictureObj;
        public GameObject button;

        public string name;
        [TextArea(3, 6)] public string description;
    }

    [Header("Display Slot (UI Output)")]
    [SerializeField] private TMP_Text slotName;
    [SerializeField] private TMP_Text slotDescription;

    [Header("Bacteria Data (Fill in Inspector)")]
    [SerializeField] private EnemyData BacteriaPurple;
    [SerializeField] private EnemyData BacteriaRed;
    [SerializeField] private EnemyData BacteriaGreen;
    [SerializeField] private EnemyData BacteriaBlue;
    [SerializeField] private EnemyData BacteriaPink;
    [SerializeField] private EnemyData BacteriaOrange;
    [SerializeField] private EnemyData BacteriaYellow;

    private EnemyData[] allBacteria;

    private void Start()
    {
        allBacteria = new EnemyData[]
        {
            BacteriaPurple,
            BacteriaRed,
            BacteriaGreen,
            BacteriaBlue,
            BacteriaPink,
            BacteriaOrange,
            BacteriaYellow
        };

        ShowBacteriaPurple();
    }

    private void ApplyEnemy(EnemyData data)
    {
        if (data == null) return;

        // Matikan semua picture + button
        foreach (var e in allBacteria)
        {
            if (e != null)
            {
                if (e.pictureObj != null) e.pictureObj.SetActive(false);
                if (e.button != null) e.button.SetActive(false);
            }
        }

        // Aktifkan yang dipilih
        if (data.pictureObj != null) data.pictureObj.SetActive(true);
        if (data.button != null) data.button.SetActive(true);

        // Update text
        if (slotName != null) slotName.text = data.name;
        if (slotDescription != null) slotDescription.text = data.description;
    }

    // ===== BUTTON CALLS =====

    public void ShowBacteriaPurple() => ApplyEnemy(BacteriaPurple);
    public void ShowBacteriaRed() => ApplyEnemy(BacteriaRed);
    public void ShowBacteriaGreen() => ApplyEnemy(BacteriaGreen);
    public void ShowBacteriaBlue() => ApplyEnemy(BacteriaBlue);
    public void ShowBacteriaPink() => ApplyEnemy(BacteriaPink);
    public void ShowBacteriaOrange() => ApplyEnemy(BacteriaOrange);
    public void ShowBacteriaYellow() => ApplyEnemy(BacteriaYellow);
}
