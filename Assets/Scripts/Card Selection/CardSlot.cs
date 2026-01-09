using UnityEngine;

public class CardSlot : MonoBehaviour
{
    public CardSelectable occupiedCard;
    public bool IsFree => occupiedCard == null;
}
