using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CapsuleSlot : MonoBehaviour
{

    [Header("Slot Data")]
    public Sprite capsuleSprite;
    public GameObject capsuleObject;
    public int Price;

    [Header("UI References")]
    public TextMeshProUGUI priceText;
    public Image icon;

    [Header("Card Visuals (Gelap Saat Dipilih)")]
    [SerializeField] private Image cardImage;       // 🔹 latar belakang kartu kapsul
    [SerializeField] private Image capsuleImage;    // 🔹 ikon kapsul
    [SerializeField] private TextMeshProUGUI priceTextVisual;
    [SerializeField, Range(0f, 1f)] private float selectedDarkAlpha = 0.5f;

    [Header("Optional Visuals")]
    public Image slotButtonImage;
    [Range(0.2f, 1f)] public float notEnoughAlpha = 0.5f;

    private GameManager gms;
    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
    }

    private void Start()
    {
        if (gms == null)
            gms = GameObject.Find("GameManager")?.GetComponent<GameManager>();

        btn.onClick.RemoveListener(SelectPlant);
        btn.onClick.AddListener(SelectPlant);

        ApplyStaticUI();
        ApplyAffordability();
        ApplySelectionVisual(); // 🔹 sync awal
    }

    private void Update()
    {
        ApplyAffordability();
        ApplySelectionVisual(); // 🔹 update efek visual sesuai seleksi global
    }

    private void ApplyStaticUI()
    {
        if (icon != null)
        {
            if (capsuleSprite)
            {
                icon.enabled = true;
                icon.sprite = capsuleSprite;
            }
            else
            {
                icon.enabled = false;
            }
        }

        if (priceText != null)
            priceText.text = Price.ToString();
    }

    private void ApplyAffordability()
    {
        if (gms == null) return;

        bool canAfford = gms.suns >= Price;

        if (btn != null)
            btn.interactable = canAfford;

        if (slotButtonImage != null)
        {
            Color c = slotButtonImage.color;
            c.a = canAfford ? 1f : notEnoughAlpha;
            slotButtonImage.color = c;
        }
    }

    private void SelectPlant()
    {
        if (gms == null) return;

        if (gms.suns < Price)
        {
            Debug.LogWarning($"[CapsuleSlot] Suns tidak cukup untuk memilih {capsuleObject?.name} (butuh {Price}, punya {gms.suns})");
            return;
        }

        // Klik ulang slot yang sama = batalkan
        if (gms.currentCapsule == capsuleObject)
        {
            gms.CancelSelection();
            Debug.Log($"[CapsuleSlot] Cancel selection: {capsuleObject?.name}");
        }
        else
        {
            gms.SelectPlant(capsuleObject, capsuleSprite, Price);
            Debug.Log($"[CapsuleSlot] Selected: {capsuleObject?.name} (Price: {Price})");
        }

        // 🔹 Visual akan otomatis sinkron di Update()
    }

    /// <summary>
    /// 🔹 Efek visual saat slot dipilih — dibandingkan langsung dengan GameManager.currentCapsule
    /// </summary>
    /// <summary>
    /// 🔹 Efek visual saat slot dipilih — dibandingkan langsung dengan GameManager.currentCapsule
    /// </summary>
    private void ApplySelectionVisual()
    {
        if (gms == null) return;

        bool isThisSelected = (gms.currentCapsule == capsuleObject);
        float alpha = isThisSelected ? selectedDarkAlpha : 1f;

        if (cardImage != null)
        {
            Color c = cardImage.color;
            c.a = alpha;
            cardImage.color = c;
        }

        if (capsuleImage != null)
        {
            Color c = capsuleImage.color;
            c.a = alpha;
            capsuleImage.color = c;
        }

        if (priceTextVisual != null)
        {
            Color c = priceTextVisual.color;
            c.a = alpha;
            priceTextVisual.color = c;
        }
    }

    private void OnValidate()
    {
        if (priceText != null)
            priceText.text = Price.ToString();

        if (icon != null)
        {
            if (capsuleSprite)
            {
                icon.enabled = true;
                icon.sprite = capsuleSprite;
            }
            else
            {
                icon.enabled = false;
            }
        }
    }
}
