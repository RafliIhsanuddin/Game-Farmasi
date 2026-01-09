using UnityEngine;
using System.Collections.Generic;

public class CardSelectionManager : MonoBehaviour
{
    [SerializeField] private List<CardSelectable> allCards;
    [SerializeField] private List<CardSlot> slots;
    [SerializeField] private int maxSelection = 3;
    [SerializeField] private float compactSpeed = 12f;

    private int currentSelected = 0;
    private bool locked = false;

    private void Update()
    {
        SmoothCompactAnimation();
    }

    public void OnCardClicked(CardSelectable card)
    {
        // cek apakah card sedang berada di slot (=> unequip)
        foreach (var s in slots)
        {
            if (s.occupiedCard == card)
            {
                RemoveFromSlot(s);
                return;
            }
        }

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

        if (currentSelected == maxSelection)
            locked = true;
    }

    void RemoveFromSlot(CardSlot slot)
    {
        if (slot.occupiedCard == null) return;

        slot.occupiedCard.ReturnToSelector();
        slot.occupiedCard = null;

        currentSelected--;
        locked = false;

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
                    pos, target, Time.deltaTime * compactSpeed
                );
            }
        }
    }

    public void ConfirmSelection()
    {
        locked = true;

        foreach (var c in allCards)
            c.enabled = false;

        foreach (var s in slots)
        {
            if (s.occupiedCard == null) continue;

            var cap = s.occupiedCard.GetComponent<CapsuleSlot>();
            if (cap != null) cap.enabled = true;
        }
    }
}
