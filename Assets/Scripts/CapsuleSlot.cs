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

    // Opsional: efek visual saat tidak cukup suns
    [Header("Optional Visuals")]
    public Image slotButtonImage;   // drag Image dari tombol di Inspector (boleh kosong)
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

        // pastikan listener hanya ditambahkan sekali
        btn.onClick.RemoveListener(SelectPlant);
        btn.onClick.AddListener(SelectPlant);

        // set label harga & ikon
        ApplyStaticUI();
        // set status awal (jika saat start suns belum cukup)
        ApplyAffordability();
    }

    private void Update()
    {
        // Polling ringan supaya tombol selalu sinkron dengan nilai suns saat ini
        ApplyAffordability();
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

    /// <summary>
    /// Aktif/nonaktifkan tombol sesuai cukup/tidaknya suns, plus efek visual opsional.
    /// </summary>
    private void ApplyAffordability()
    {
        if (gms == null) return;

        bool canAfford = gms.suns >= Price;

        // 1) KUNCI tombol agar tidak bisa diklik
        if (btn != null)
            btn.interactable = canAfford;

        // 2) (Opsional) efek visual saat tidak cukup suns
        if (slotButtonImage != null)
        {
            Color c = slotButtonImage.color;
            c.a = canAfford ? 1f : notEnoughAlpha;
            slotButtonImage.color = c;
        }
    }

    /// <summary>
    /// Saat slot diklik (hanya akan terpanggil jika btn.interactable = true).
    /// Tetap ada guard tambahan kalau-kalau interactable belum terset.
    /// </summary>
    private void SelectPlant()
    {
        if (gms == null) return;

        // Guard sekunder (defensive)
        if (gms.suns < Price)
        {
            Debug.LogWarning($"[CapsuleSlot] Suns tidak cukup untuk memilih {capsuleObject?.name} (butuh {Price}, punya {gms.suns})");
            return;
        }

        // Klik ulang slot yang sama = batalkan pilihan
        if (gms.currentCapsule == capsuleObject)
        {
            gms.CancelSelection();
            Debug.Log($"[CapsuleSlot] Cancel selection: {capsuleObject?.name}");
        }
        else
        {
            // Hanya memilih (tanpa mengurangi suns) — suns dipotong saat tanam di tile
            gms.SelectPlant(capsuleObject, capsuleSprite, Price);
            Debug.Log($"[CapsuleSlot] Selected: {capsuleObject?.name} (Price: {Price})");
        }
    }

    // Jika kamu ubah Price/capsuleSprite di Inspector saat play, ini menjaga UI tetap benar
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
