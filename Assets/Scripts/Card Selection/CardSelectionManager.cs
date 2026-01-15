using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CardSelectionManager : MonoBehaviour
{
    [Header("Cards & Slots")]
    [SerializeField] private List<CardSelectable> allCards;
    [SerializeField] private List<CardSlot> slots;

    [Header("Selection Settings")]
    [SerializeField] private int maxSelection = 3;      // ⬅ serialize
    [SerializeField] private float compactSpeed = 12f;

    private int currentSelected = 0;

    private void Update()
    {
        SmoothCompactAnimation();
    }

    public void OnCardClicked(CardSelectable card)
    {
        // CASE: sudah max, user klik kartu lain -> WRONG
        if (currentSelected >= maxSelection)
        {
            bool isSelected = false;

            foreach (var s in slots)
            {
                if (s.occupiedCard == card)
                {
                    isSelected = true;
                    break;
                }
            }

            if (!isSelected)
            {
                SoundManager.Instance?.PlayWrong();
                return;
            }
        }

        // CASE: REMOVE / unselect
        foreach (var s in slots)
        {
            if (s.occupiedCard == card)
            {
                RemoveFromSlot(s);
                return;
            }
        }

        // CASE: ADD
        AddToSlot(card);
    }

    void AddToSlot(CardSelectable card)
    {
        if (currentSelected >= maxSelection)
        {
            SoundManager.Instance?.PlayWrong();
            return;
        }

        CardSlot empty = slots.Find(s => s.IsFree);
        if (empty == null)
        {
            SoundManager.Instance?.PlayWrong();
            return;
        }

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
                card.transform.position = Vector3.Lerp(
                    card.transform.position,
                    target,
                    Time.deltaTime * compactSpeed
                );
            }
        }
    }

    public void ConfirmSelection()
    {
        foreach (var card in allCards)
            card.enabled = false;

        foreach (var s in slots)
        {
            if (s.occupiedCard == null) continue;

            var card = s.occupiedCard;
            var btn = card.GetComponent<Button>();
            var cap = card.GetComponent<CapsuleSlot>();

            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                if (cap != null)
                    btn.onClick.AddListener(cap.SelectPlant);
            }

            if (cap != null)
                cap.enabled = true;
        }

        this.enabled = false;
        gameObject.SetActive(false);
    }

    public int GetSelectedCount()
    {
        return currentSelected;
    }

    public int GetMaxSelection()
    {
        return maxSelection;  // optionally useful
    }
}
