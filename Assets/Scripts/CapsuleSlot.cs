using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
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
    [SerializeField] private Image cardImage;       
    [SerializeField] private Image capsuleImage;    
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
        btn.onClick.RemoveListener(SelectPlant);
        btn.onClick.AddListener(SelectPlant);
    }

    private void OnEnable()
    {
        // Subscribe event pause supaya UI langsung update saat pause/resume
        PauseManager.OnPauseChanged += HandlePauseChanged;
    }

    private void OnDisable()
    {
        PauseManager.OnPauseChanged -= HandlePauseChanged;
    }

    private void Start()
    {
        if (gms == null)
            gms = GameObject.Find("GameManager")?.GetComponent<GameManager>();

        ApplyStaticUI();
        RefreshAllVisuals();
    }

    private void Update()
    {
        // Suns bisa berubah tiap frame, jadi affordability tetap refresh
        // (tapi pause/resume sudah di-handle event juga)
        ApplyAffordability();
        ApplySelectionVisual();
    }

    private void HandlePauseChanged(bool paused)
    {
        // Saat pause/resume: update interactable & visual
        RefreshAllVisuals();
    }

    private void RefreshAllVisuals()
    {
        ApplyAffordability();
        ApplySelectionVisual();
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

        bool paused = PauseManager.IsPaused;
        bool gameOver = WaveManager.isGameOver;

        bool canAfford = gms.suns >= Price;
        bool canInteract = canAfford && !paused && !gameOver;

        if (btn != null)
            btn.interactable = canInteract;

        // Visual alpha kalau tidak cukup / paused / gameover
        if (slotButtonImage != null)
        {
            Color c = slotButtonImage.color;
            c.a = canInteract ? 1f : notEnoughAlpha;
            slotButtonImage.color = c;
        }
    }

    public void SelectPlant()
    {
        if (gms == null) return;

        // BLOCK saat pause / game over
        if (PauseManager.IsPaused) return;
        if (WaveManager.isGameOver) return;

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

        // visual akan tersync
        ApplySelectionVisual();
    }

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
