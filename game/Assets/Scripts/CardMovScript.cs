using System.Collections;
using System.Collections.Generic;

using System.Runtime.CompilerServices;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

//using Assets.Scripts.Additions; // GetChildWithName

public class CardMovScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Camera MainCamera;
    Vector3 offset;
    public Transform PrevParent, DefaultParent, DefaultTempCardParent;
    GameObject TempCardGO; // временная карта и клон перетаскиваемой, который займет кё место в Hand
    int PrevIndex; // изначальный индекс карты на своём исходном поле

    void Awake()
    {
        MainCamera = Camera.allCameras[0];
        TempCardGO = GameObject.Find("TempCardGO");
    }

    // начало перетаскивания
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (MainCamera.GetComponent<GameManagerScript>().CurGame.BlockChanges) // ну такое
        {
            eventData.pointerDrag = null;
            return;
        }

        offset = transform.position - MainCamera.ScreenToWorldPoint(eventData.position);

        DefaultParent = DefaultTempCardParent = PrevParent = transform.parent;

        PrevIndex = transform.GetSiblingIndex();

        if (FromHand)
        {
            MainCamera.GetComponent<GameManagerScript>().GiveFieldOneCard(SelfCard, PrevParent, PrevIndex);

        }
        else
        {
            TempCardGO.transform.SetParent(DefaultParent);
            TempCardGO.transform.SetSiblingIndex(transform.GetSiblingIndex());
        }

        transform.SetParent(DefaultParent.parent);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    // во время перетаскивания
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 newPos = MainCamera.ScreenToWorldPoint(eventData.position);
        transform.position = newPos + offset;

        if ((TempCardGO.transform.parent != DefaultTempCardParent) && !ToHand && !SideToSide && EnoughMoney(DefaultTempCardParent, SelfCard.Cost))
            TempCardGO.transform.SetParent(DefaultTempCardParent);

        CheckPosition();
    }

    // окончание перетаскивания
    public void OnEndDrag(PointerEventData eventData)
    {
        // если карту перетащили в Hand - карта удаляется
        if (ToHand)
        {
            Debug.Log("Destroy " + this.name);

            if (!FromHand)
                MainCamera.GetComponent<GameManagerScript>().ChangeMoney(PrevParent, - SelfCard.Cost);

            Destroy(this.gameObject);
        }
        // если перетаскивание не с одного поля на другое, то устанавливаем карту в новую позицию
        else if (!SideToSide && EnoughMoney(DefaultParent, SelfCard.Cost))
        {
            transform.SetParent(DefaultParent);
            transform.SetSiblingIndex(TempCardGO.transform.GetSiblingIndex());

            if(DefaultParent != PrevParent)
                MainCamera.GetComponent<GameManagerScript>().ChangeMoney(DefaultParent, SelfCard.Cost);

        }
        else if(FromHand)
        {
            Debug.Log("Destroy " + this.name);
            Destroy(this.gameObject);
        }
        // иначе возвращаем на исходное место
        else
        {
            transform.SetParent(PrevParent);
            transform.SetSiblingIndex(TempCardGO.transform.GetSiblingIndex());
        }

        GetComponent<CanvasGroup>().blocksRaycasts = true;

        // убрать временную обратно за камеру
        TempCardGO.transform.SetParent(GameObject.Find("Canvas").transform);
        TempCardGO.transform.localPosition = new Vector3(4000, 0);

    }

    // Установить временную карту на поле, над которым перетаскивается карта, на нужную позицию (по оси х, тк сейчас карты только горизонтально расположены)
    void CheckPosition()
    {
        // Если карта над Hand или другим полем (с одного на другое нельзя перетаскивать), то временная ставится на изначальную позицию карты
        if (SideToSide || ToHand)
        {
            TempCardGO.transform.SetSiblingIndex(PrevIndex);
        }
        // иначе по оси х подбираем позицию среди детей поля (карт, которые там уже есть)
        else
        {
            int newIndex = DefaultTempCardParent.childCount;
            for (int i = 0; i < DefaultTempCardParent.childCount; i++)
            {
                if (transform.position.x < DefaultTempCardParent.GetChild(i).position.x)
                {
                    newIndex = i;
                    if (TempCardGO.transform.GetSiblingIndex() < newIndex) newIndex--;

                    break;
                }
            }

            TempCardGO.transform.SetSiblingIndex(newIndex);
        }
    }

    // Если перетаскиваемая карта взята на одном игровом поле и сейчас находится над другим
    bool SideToSide { get {
            FieldType start = PrevParent.GetComponent<DropPlaceScript>().Type;
            FieldType end = DefaultTempCardParent.GetComponent<DropPlaceScript>().Type;

            return (start == FieldType.LField && end == FieldType.RField) ||
                (start == FieldType.RField && end == FieldType.LField);
        }
    }

    
    // если карта перетаскивается из Hand куда-то
    bool FromHand { get {
            return PrevParent.GetComponent<DropPlaceScript>().Type == FieldType.HAND;
        }
    }

    
    // если перетаскиваемая карта сейчас над Hand
    bool ToHand { get {
            return DefaultTempCardParent.GetComponent<DropPlaceScript>().Type == FieldType.HAND;
        }
    }

    bool EnoughMoney(Transform field, int cost)
    {
        if (PrevParent == field) return true;

        return MainCamera.GetComponent<GameManagerScript>().EnoughMoney(field, cost);
    }

    ICard SelfCard
    { get {
            return transform.GetComponent<CardInfoScript>().SelfCard;
        }
    }
}
