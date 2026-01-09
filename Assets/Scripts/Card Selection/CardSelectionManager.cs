using UnityEngine;
using System.Collections.Generic;

public class CardSelectionManager : MonoBehaviour
{
    [SerializeField] private List<CardSelectable> allCards;
    [SerializeField] private List<CardSlot> slots;
    [SerializeField] private int maxSelection = 3;

    private int currentSelected = 0;
    private bool locked = false;

    public void OnCardClicked(CardSelectable card)
    {
        if (locked) return;

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
