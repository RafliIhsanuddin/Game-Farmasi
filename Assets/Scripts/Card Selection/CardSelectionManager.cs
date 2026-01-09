using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CardSelectionManager : MonoBehaviour
{
    [Header("Cards & Slots")]
    [SerializeField] private List<CardSelectable> allCards;
    [SerializeField] private List<CardSlot> slots;

    [Header("Selection Settings")]
    [SerializeField] private int maxSelection = 3;
    [SerializeField] private float compactSpeed = 12f;

    private int currentSelected = 0;

    private void Update()
    {
        SmoothCompactAnimation();
    }

    public void OnCardClicked(CardSelectable card)
    {
        // UNEQUIP
        foreach (var s in slots)
        {
            if (s.occupiedCard == card)
            {
                RemoveFromSlot(s);
                return;
            }
        }

        // ADD
        AddToSlot(card);
    }

    void AddToSlot(CardSelectable card)
    {
        if (currentSelected >= maxSelection) return;

        CardSlot empty = slots.Find(s => s.IsFree);
        if (empty == null) return;

        empty.occupiedCard = card;
        card.MoveToSlot(empty.transform);

        currentSelected++;
    }

    void RemoveFromSlot(CardSlot slot)
    {
        if (slot.occupiedCard == null) return;

        slot.occupiedCard.ReturnToSelector();
        slot.occupiedCard = null;

        currentSelected--;

        CompactSlots();
    }

    void CompactSlots()
    {
        List<CardSelectable> temp = new List<CardSelectable>();

        foreach (var s in slots)
        {
            if (s.occupiedCard != null)
                temp.Add(s.occupiedCard);
        }

        foreach (var s in slots)
            s.occupiedCard = null;

        for (int i = 0; i < temp.Count; i++)
            slots[i].occupiedCard = temp[i];
    }

    void SmoothCompactAnimation()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].occupiedCard != null)
            {
                var card = slots[i].occupiedCard;
                var target = slots[i].transform.position;
                var pos = card.transform.position;

                card.transform.position = Vector3.Lerp(
                    pos,
                    target,
                    Time.deltaTime * compactSpeed
                );
            }
        }
    }

    // DIPANGGIL ReadyButton
    public void ConfirmSelection()
    {
        // 1. Disable selectable scripts
        foreach (var card in allCards)
            card.enabled = false;

        // 2. Swap Button Listener to CapsuleSlot
        foreach (var s in slots)
        {
            if (s.occupiedCard == null) continue;

            var card = s.occupiedCard;
            var btn = card.GetComponent<Button>();
            var cap = card.GetComponent<CapsuleSlot>();

            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();

                // ini penting: langsung gunakan SelectPlant milik CapsuleSlot
                if (cap != null)
                    btn.onClick.AddListener(cap.SelectPlant);
            }

            // aktifkan script CapsuleSlot supaya Update() affordability & highlight jalan
            if (cap != null)
                cap.enabled = true;
        }

        // 3. Disable manager
        this.enabled = false;
        gameObject.SetActive(false); // seperti permintaanmu
    }
}
