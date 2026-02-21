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

    [Header("Card Visuals")]
    [SerializeField] private Image cardImage;
    [SerializeField] private Image capsuleImage;
    [SerializeField] private TextMeshProUGUI priceTextVisual;

    [Header("Selection Visual")]
    [SerializeField] private Color selectedColor = Color.green;

    [Header("Dark Overlay")]
    [SerializeField] private Image darkOverlay;

    [Header("Cooldown Settings")]
    [SerializeField] private float cooldownDuration = 5f;

    [Header("Cooldown Mask (Vertical Bottom → Top)")]
    [SerializeField] private Image cooldownMask;

    private float cooldownTimer;
    private bool isCoolingDown;
    private float debugTimer;

    private GameManager gms;
    private Button btn;

    private Color originalCardColor;
    private Color originalCapsuleColor;
    private Color originalPriceColor;

    // =====================================================
    // INIT
    // =====================================================

    private void Awake()
    {
        btn = GetComponent<Button>();

        btn.onClick.RemoveListener(SelectPlant);
        btn.onClick.AddListener(SelectPlant);

        if (cardImage) originalCardColor = cardImage.color;
        if (capsuleImage) originalCapsuleColor = capsuleImage.color;
        if (priceTextVisual) originalPriceColor = priceTextVisual.color;
    }

    private void Start()
    {
        gms = GameObject.Find("GameManager")?.GetComponent<GameManager>();

        ApplyStaticUI();

        if (darkOverlay)
            darkOverlay.gameObject.SetActive(false);

        StartCooldown();
    }

    private void Update()
    {
        UpdateCooldown();
        ApplyAffordability();
        ApplySelectionVisual();
    }

    // =====================================================
    // COOLDOWN SYSTEM
    // =====================================================

    public void StartCooldown()
    {
        Debug.Log($"[CapsuleSlot] CooldownDuration = {cooldownDuration}");

        if (cooldownDuration <= 0f)
        {
            isCoolingDown = false;

            if (cooldownMask)
                cooldownMask.fillAmount = 0;

            Debug.Log($"[CapsuleSlot] cooldown skipped");
            return;
        }

        isCoolingDown = true;
        cooldownTimer = cooldownDuration;
        debugTimer = cooldownDuration;

        if (cooldownMask)
            cooldownMask.fillAmount = 1f;

        Debug.Log($"[CapsuleSlot] Cooldown START ({cooldownDuration}s)");
    }

    private void UpdateCooldown()
    {
        if (!isCoolingDown)
            return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownMask)
            cooldownMask.fillAmount = cooldownTimer / cooldownDuration;

        debugTimer -= Time.deltaTime;

        if (debugTimer <= 0f)
        {
            float progress =
                (1f - (cooldownTimer / cooldownDuration)) * 100f;

            Debug.Log(
                $"[CapsuleSlot] {capsuleObject.name} Cooldown Progress: {progress:F0}% | {cooldownTimer:F1}s left"
            );

            debugTimer = 1f;
        }

        if (cooldownTimer <= 0f)
        {
            isCoolingDown = false;

            if (cooldownMask)
                cooldownMask.fillAmount = 0f;

            Debug.Log($"[CapsuleSlot] {capsuleObject.name} Cooldown COMPLETE");
        }
    }

    public void OnCapsulePlaced()
    {
        Debug.Log($"[CapsuleSlot] {capsuleObject.name} placed → restarting cooldown");
        StartCooldown();
    }

    // =====================================================
    // AFFORDABILITY
    // =====================================================

    private void ApplyAffordability()
    {
        if (!gms) return;

        bool canAfford = gms.suns >= Price;

        bool canInteract =
            canAfford &&
            !PauseManager.IsPaused &&
            !WaveManager.isGameOver &&
            !isCoolingDown;

        btn.interactable = canInteract;

        if (darkOverlay)
            darkOverlay.gameObject.SetActive(!canAfford);
    }

    // =====================================================
    // SELECTION VISUAL
    // =====================================================

    private void ApplySelectionVisual()
    {
        if (!gms) return;

        bool isSelected =
            gms.currentCapsule == capsuleObject;

        bool canAfford =
            gms.suns >= Price;

        if (isCoolingDown || !canAfford)
        {
            RestoreOriginalColor();
            return;
        }

        if (isSelected)
            SetColor(selectedColor);
        else
            RestoreOriginalColor();
    }

    private void SetColor(Color color)
    {
        if (cardImage) cardImage.color = color;
        if (capsuleImage) capsuleImage.color = color;
        if (priceTextVisual) priceTextVisual.color = color;
    }

    private void RestoreOriginalColor()
    {
        if (cardImage) cardImage.color = originalCardColor;
        if (capsuleImage) capsuleImage.color = originalCapsuleColor;
        if (priceTextVisual) priceTextVisual.color = originalPriceColor;
    }

    // =====================================================
    // STATIC UI
    // =====================================================

    private void ApplyStaticUI()
    {
        if (icon)
        {
            icon.enabled = capsuleSprite != null;
            icon.sprite = capsuleSprite;
        }

        if (priceText)
            priceText.text = Price.ToString();
    }

    // =====================================================
    // SELECT
    // =====================================================

    public void SelectPlant()
    {
        if (!gms) return;

        if (PauseManager.IsPaused) return;
        if (WaveManager.isGameOver) return;
        if (isCoolingDown) return;
        if (gms.suns < Price) return;

        if (gms.currentCapsule == capsuleObject)
            gms.CancelSelection();
        else
            gms.SelectPlant(capsuleObject, capsuleSprite, Price);
    }
}