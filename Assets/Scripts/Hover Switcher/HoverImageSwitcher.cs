using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class HoverImageSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class HoverUIData
    {
        [Header("Target")]
        public Image targetImage;
        public TMP_Text targetText;

        [Header("Button (cek unlock)")]
        public Button button;

        [Header("Image Sprite")]
        public Sprite normalSprite;
        public Sprite hoverSprite;

        [Header("Text Color")]
        public Color normalTextColor = Color.black;
        public Color hoverTextColor = Color.white;

        [Header("Audio")]
        public bool playHoverSound = true;
        public AudioClip hoverSound;
    }

    [Header("Hover UI List")]
    public List<HoverUIData> hoverUIs = new List<HoverUIData>();

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f; // 2D
        audioSource.ignoreListenerPause = true;
    }

    private void Start()
    {
        foreach (var data in hoverUIs)
        {
            if (data.targetImage == null)
                continue;

            // Set kondisi awal
            if (data.normalSprite != null)
                data.targetImage.sprite = data.normalSprite;

            if (data.targetText != null)
                data.targetText.color = data.normalTextColor;

            // 🔒 Lock = tidak menerima hover
            data.targetImage.raycastTarget = IsUnlocked(data);

            AddHoverEvents(data);
        }
    }

    private bool IsUnlocked(HoverUIData data)
    {
        if (data.button == null)
            return true; // fallback aman

        return data.button.interactable;
    }

    private void AddHoverEvents(HoverUIData data)
    {
        GameObject obj = data.targetImage.gameObject;

        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = obj.AddComponent<EventTrigger>();

        trigger.triggers.Clear();

        // Pointer Enter
        EventTrigger.Entry enter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        enter.callback.AddListener((e) => OnHoverEnter(data));
        trigger.triggers.Add(enter);

        // Pointer Exit
        EventTrigger.Entry exit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        exit.callback.AddListener((e) => OnHoverExit(data));
        trigger.triggers.Add(exit);
    }

    private void OnHoverEnter(HoverUIData data)
    {
        if (!IsUnlocked(data))
            return;

        if (data.targetImage != null && data.hoverSprite != null)
            data.targetImage.sprite = data.hoverSprite;

        if (data.targetText != null)
            data.targetText.color = data.hoverTextColor;

        if (data.playHoverSound && data.hoverSound != null)
            audioSource.PlayOneShot(data.hoverSound);
    }

    private void OnHoverExit(HoverUIData data)
    {
        if (!IsUnlocked(data))
            return;

        if (data.targetImage != null && data.normalSprite != null)
            data.targetImage.sprite = data.normalSprite;

        if (data.targetText != null)
            data.targetText.color = data.normalTextColor;
    }
}
