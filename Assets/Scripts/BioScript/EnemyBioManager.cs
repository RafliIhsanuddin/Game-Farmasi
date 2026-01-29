using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyBioManager : MonoBehaviour
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

    [Header("Enemies Data (Fill in Inspector)")]
    [SerializeField] private EnemyData BacteriaPurple;
    [SerializeField] private EnemyData BacteriaRed;
    [SerializeField] private EnemyData Fungi;
    [SerializeField] private EnemyData Virus;
    [SerializeField] private EnemyData Protozoa;
    [SerializeField] private EnemyData Helminth;

    private EnemyData[] allEnemies;

    private void Start()
    {
        allEnemies = new EnemyData[]
        {
            BacteriaPurple,
            BacteriaRed,
            Fungi,
            Virus,
            Protozoa,
            Helminth
        };

        ShowBacteriaPurple();
    }

    private void ApplyEnemy(EnemyData data)
    {
        if (data == null) return;

        // matikan semua picture + button
        foreach (var e in allEnemies)
        {
            if (e != null)
            {
                if (e.pictureObj != null) e.pictureObj.SetActive(false);
                if (e.button != null) e.button.SetActive(false);
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

    public void ShowBacteriaPurple() => ApplyEnemy(BacteriaPurple);
    public void ShowBacteriaRed() => ApplyEnemy(BacteriaRed);
    public void ShowFungi() => ApplyEnemy(Fungi);
    public void ShowVirus() => ApplyEnemy(Virus);
    public void ShowProtozoa() => ApplyEnemy(Protozoa);
    public void ShowHelminth() => ApplyEnemy(Helminth);
}
