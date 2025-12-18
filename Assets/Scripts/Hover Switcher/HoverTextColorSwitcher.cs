using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class HoverTextColorManager : MonoBehaviour
{
    [System.Serializable]
    public class HoverTextData
    {
        [Header("Target")]
        public Button targetButton;      // BUTTON sebagai target hover
        public TMP_Text targetText;      // TEXT yang warnanya diubah

        [Header("Text Color")]
        public Color normalTextColor = Color.black;
        public Color hoverTextColor = Color.white;

        [Header("Audio (Opsional)")]
        public bool playHoverSound = false;
        public AudioClip hoverSound;
    }

    [Header("Daftar Hover Text")]
    public List<HoverTextData> hoverTexts = new List<HoverTextData>();

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = 1f;
        audioSource.spatialBlend = 0f; // 2D
    }

    private void Start()
    {
        foreach (var data in hoverTexts)
        {
            if (data.targetButton == null || data.targetText == null)
                continue;

            // Warna awal text
            data.targetText.color = data.normalTextColor;

            // TEXT tidak boleh menangkap raycast
            data.targetText.raycastTarget = false;

            AddHoverEvents(data);
        }
    }

    private void AddHoverEvents(HoverTextData data)
    {
        GameObject obj = data.targetButton.gameObject;

        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = obj.AddComponent<EventTrigger>();

        trigger.triggers.Clear(); // aman karena kita pasang di BUTTON

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

    private void OnHoverEnter(HoverTextData data)
    {
        data.targetText.color = data.hoverTextColor;

        if (data.playHoverSound && data.hoverSound != null)
            audioSource.PlayOneShot(data.hoverSound);
    }

    private void OnHoverExit(HoverTextData data)
    {
        data.targetText.color = data.normalTextColor;
    }
}