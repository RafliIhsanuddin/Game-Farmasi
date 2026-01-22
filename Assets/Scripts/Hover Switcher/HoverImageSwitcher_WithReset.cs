using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class HoverImageSwitcher_WithReset : MonoBehaviour
{
    [System.Serializable]
    public class HoverUIData
    {
        [Header("Target")]
        public Image targetImage;
        public TMP_Text targetText; // boleh null

        [Header("Button (cek unlock)")]
        public Button button;

        [Header("Image Sprite")]
        public Sprite normalSprite;
        public Sprite hoverSprite;

        [Header("Text Behavior")]
        public bool textHoverEnabled = true;

        [Header("Text Color")]
        public Color normalTextColor = Color.black;
        public Color hoverTextColor = Color.white;

        [Header("Audio")]
        public bool playHoverSound = true;
        public AudioClip hoverSound;

        // INTERNAL
        [HideInInspector] public bool isHovering = false;
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
        audioSource.spatialBlend = 0f;
        audioSource.ignoreListenerPause = true;
    }

    private void Start()
    {
        foreach (var data in hoverUIs)
        {
            if (data.targetImage == null) continue;

            if (data.normalSprite)
                data.targetImage.sprite = data.normalSprite;

            if (data.textHoverEnabled && data.targetText)
                data.targetText.color = data.normalTextColor;

            data.targetImage.raycastTarget = IsUnlocked(data);
            AddHoverEvents(data);
        }
    }

    private bool IsUnlocked(HoverUIData data)
    {
        if (data.button == null) return true;
        return data.button.interactable;
    }

    private void AddHoverEvents(HoverUIData data)
    {
        EventTrigger trig = data.targetImage.gameObject.GetComponent<EventTrigger>();
        if (trig == null)
            trig = data.targetImage.gameObject.AddComponent<EventTrigger>();

        trig.triggers.Clear();

        EventTrigger.Entry enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        enter.callback.AddListener((e) => OnHoverEnter(data));
        trig.triggers.Add(enter);

        EventTrigger.Entry exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exit.callback.AddListener((e) => OnHoverExit(data));
        trig.triggers.Add(exit);
    }

    private void OnHoverEnter(HoverUIData data)
    {
        if (!IsUnlocked(data)) return;

        data.isHovering = true;

        if (data.targetImage && data.hoverSprite)
            data.targetImage.sprite = data.hoverSprite;

        if (data.textHoverEnabled && data.targetText)
            data.targetText.color = data.hoverTextColor;

        if (audioSource && data.playHoverSound && data.hoverSound)
            audioSource.PlayOneShot(data.hoverSound);
    }

    private void OnHoverExit(HoverUIData data)
    {
        if (!IsUnlocked(data)) return;

        data.isHovering = false;
        ResetToNormal(data);
    }

    private void ResetToNormal(HoverUIData data)
    {
        if (data.targetImage && data.normalSprite)
            data.targetImage.sprite = data.normalSprite;

        if (data.textHoverEnabled && data.targetText)
            data.targetText.color = data.normalTextColor;
    }

    private void Update()
    {
        // Reset globlal saat timelapse == 0 (atau pause)
        if (Time.timeScale == 0f)
        {
            foreach (var data in hoverUIs)
            {
                data.isHovering = false;
                ResetToNormal(data);
            }
        }
    }
}
