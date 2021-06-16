using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

public enum FieldType
{
    HAND,
    LField,
    RField
}

public class DropPlaceScript : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public FieldType Type;

    public int countCards = 0;

    public void OnDrop(PointerEventData eventData)
    {
        CardMovScript card = eventData.pointerDrag.GetComponent<CardMovScript>();

        if (card)
        {
            if (card.ThreeFormation)
            {
                if(card.CanMoveToSubField(transform, 1) && card.CanTakeFromSubField(card.PrevParent))
                {
                    card.DefaultParent = transform;
                }
            }
            else
            {
                card.DefaultParent = transform;
            }

        }

        //Debug.Log(" 1 " + transform.name);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        CardMovScript card = eventData.pointerDrag.GetComponent<CardMovScript>();

        if (card) card.DefaultTempCardParent = transform;

        //Debug.Log(" 2 " + transform.name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        CardMovScript card = eventData.pointerDrag.GetComponent<CardMovScript>();

        if (card && card.DefaultTempCardParent == transform)
            card.DefaultTempCardParent = card.DefaultParent;
    }
}
