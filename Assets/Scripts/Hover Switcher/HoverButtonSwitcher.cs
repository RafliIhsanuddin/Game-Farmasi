using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class HoverButtonSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class ButtonPair
    {
        [Header("Button")]
        public Button buttonNormal; // tombol normal
        public Button buttonHover;  // tombol saat hover

        [Header("Audio (Opsional)")]
        public bool playHoverSound = false;
        public AudioClip hoverSound;
    }

    [Header("Daftar pasangan button")]
    public List<ButtonPair> buttonPairs = new List<ButtonPair>();

    private AudioSource audioSource;

    private void Awake()
    {
        // Siapkan AudioSource satu kali
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void Start()
    {
        foreach (var pair in buttonPairs)
        {
            if (pair.buttonNormal == null || pair.buttonHover == null)
                continue;

            // Awal: normal aktif, hover mati
            pair.buttonNormal.gameObject.SetActive(true);
            pair.buttonHover.gameObject.SetActive(false);

            AddHoverEvents(pair.buttonNormal.gameObject, pair, true);
            AddHoverEvents(pair.buttonHover.gameObject, pair, false);
        }
    }

    private void AddHoverEvents(GameObject buttonObject, ButtonPair pair, bool isNormal)
    {
        EventTrigger trigger = buttonObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = buttonObject.AddComponent<EventTrigger>();

        trigger.triggers.Clear();

        if (isNormal)
        {
            EventTrigger.Entry enterEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            enterEntry.callback.AddListener((data) => OnHoverEnter(pair));
            trigger.triggers.Add(enterEntry);
        }
        else
        {
            EventTrigger.Entry exitEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            exitEntry.callback.AddListener((data) => OnHoverExit(pair));
            trigger.triggers.Add(exitEntry);
        }
    }

    private void OnHoverEnter(ButtonPair pair)
    {
        pair.buttonNormal.gameObject.SetActive(false);
        pair.buttonHover.gameObject.SetActive(true);

        // 🔊 Mainkan suara jika diaktifkan & ada clip
        if (pair.playHoverSound && pair.hoverSound != null)
        {
            audioSource.PlayOneShot(pair.hoverSound);
        }
    }

    private void OnHoverExit(ButtonPair pair)
    {
        pair.buttonNormal.gameObject.SetActive(true);
        pair.buttonHover.gameObject.SetActive(false);
    }
}
