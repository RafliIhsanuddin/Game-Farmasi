using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyOtherBioManager : MonoBehaviour
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

    [Header("Other Enemies Data (Fill in Inspector)")]
    [SerializeField] private EnemyData Fungi;
    [SerializeField] private EnemyData Virus;
    [SerializeField] private EnemyData Protozoa;
    [SerializeField] private EnemyData Helminth;

    private EnemyData[] allOthers;

    private void Start()
    {
        allOthers = new EnemyData[]
        {
            Fungi,
            Virus,
            Protozoa,
            Helminth
        };

        ShowFungi();
    }

    private void ApplyEnemy(EnemyData data)
    {
        if (data == null) return;

        // Matikan semua picture + button
        foreach (var e in allOthers)
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

    public void ShowFungi() => ApplyEnemy(Fungi);
    public void ShowVirus() => ApplyEnemy(Virus);
    public void ShowProtozoa() => ApplyEnemy(Protozoa);
    public void ShowHelminth() => ApplyEnemy(Helminth);
}