using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class HoverButtonSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class ButtonPair
    {
        public Button buttonNormal; // tombol normal
        public Button buttonHover;  // tombol saat hover
    }

    [Header("Daftar pasangan button")]
    public List<ButtonPair> buttonPairs = new List<ButtonPair>();

    private void Start()
    {
        foreach (var pair in buttonPairs)
        {
            if (pair.buttonNormal == null || pair.buttonHover == null)
                continue;

            // Awal: tampilkan tombol normal, sembunyikan tombol hover
            pair.buttonNormal.gameObject.SetActive(true);
            pair.buttonHover.gameObject.SetActive(false);

            // Tambahkan event ke kedua tombol
            AddHoverEvents(pair.buttonNormal.gameObject, pair, true);
            AddHoverEvents(pair.buttonHover.gameObject, pair, false);
        }
    }

    private void AddHoverEvents(GameObject buttonObject, ButtonPair pair, bool isNormal)
    {
        EventTrigger trigger = buttonObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = buttonObject.AddComponent<EventTrigger>();

        if (isNormal)
        {
            // Saat pointer masuk ke tombol normal → ganti ke tombol hover
            EventTrigger.Entry enterEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            enterEntry.callback.AddListener((data) => OnHoverEnter(pair));
            trigger.triggers.Add(enterEntry);
        }
        else
        {
            // Saat pointer keluar dari tombol hover → kembalikan ke tombol normal
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
        // Masuk ke area tombol normal → tampilkan tombol hover
        pair.buttonNormal.gameObject.SetActive(false);
        pair.buttonHover.gameObject.SetActive(true);
    }

    private void OnHoverExit(ButtonPair pair)
    {
        // Keluar dari area tombol hover → tampilkan tombol normal lagi
        pair.buttonNormal.gameObject.SetActive(true);
        pair.buttonHover.gameObject.SetActive(false);
    }
}
