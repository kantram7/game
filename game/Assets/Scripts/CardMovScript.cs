using System.Collections;
using System.Collections.Generic;

using System.Runtime.CompilerServices;
using Assets.Scripts.Units;

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
    GameObject TempCardGO; // ��������� ����� � ���� ���������������, ������� ������ � ����� � Hand
    int PrevIndex; // ����������� ������ ����� �� ���� �������� ����

    public CardControler CC;

    void Awake()
    {
        MainCamera = Camera.allCameras[0];
        TempCardGO = GameObject.Find("TempCardGO");
    }

    // ������ ��������������
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameManagerScript.Instance.CurGame.BlockChanges) // �� �����
        {
            eventData.pointerDrag = null;
            return;
        }

        offset = transform.position - MainCamera.ScreenToWorldPoint(eventData.position);

        DefaultParent = DefaultTempCardParent = PrevParent = transform.parent;

        PrevIndex = transform.GetSiblingIndex();

        if (FromHand)
        {
            GameManagerScript.Instance.GiveFieldOneCard(SelfCard, PrevParent, PrevIndex);
        }
        else
        {
            TempCardGO.transform.SetParent(DefaultParent);
            TempCardGO.transform.SetSiblingIndex(transform.GetSiblingIndex());
        }

        transform.SetParent(DefaultParent.parent);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    // �� ����� ��������������
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 newPos = MainCamera.ScreenToWorldPoint(eventData.position);
        transform.position = newPos + offset;

        if ((TempCardGO.transform.parent != DefaultTempCardParent) && !ToHand && !SideToSide && EnoughMoney(DefaultTempCardParent, SelfCard.Cost))
            TempCardGO.transform.SetParent(DefaultTempCardParent);

        CheckPosition();
    }

    // ��������� ��������������
    public void OnEndDrag(PointerEventData eventData)
    {
        // ���� ����� ���������� � Hand - ����� ���������
        if (ToHand)
        {
            //Debug.Log("Destroy " + this.name);

            if (!FromHand)
                GameManagerScript.Instance.ChangeMoney(PrevParent, - SelfCard.Cost);

            CC.DestroyCard();
        }
        // ���� �������������� �� � ������ ���� �� ������, �� ������������� ����� � ����� �������
        else if (!SideToSide && EnoughMoney(DefaultParent, SelfCard.Cost))
        {
            transform.SetParent(DefaultParent);
            transform.SetSiblingIndex(TempCardGO.transform.GetSiblingIndex());

            if(DefaultParent != PrevParent)
                GameManagerScript.Instance.ChangeMoney(DefaultParent, SelfCard.Cost);

        }
        else if(FromHand)
        {
            //Debug.Log("Destroy " + this.name);
            CC.DestroyCard();
        }
        // ����� ���������� �� �������� �����
        else
        {
            transform.SetParent(PrevParent);
            transform.SetSiblingIndex(TempCardGO.transform.GetSiblingIndex());
        }

        GetComponent<CanvasGroup>().blocksRaycasts = true;

        // ������ ��������� ������� �� ������
        TempCardGO.transform.SetParent(GameObject.Find("Canvas").transform);
        TempCardGO.transform.localPosition = new Vector3(4000, 0);

    }

    // ���������� ��������� ����� �� ����, ��� ������� ��������������� �����, �� ������ ������� (�� ��� �, �� ������ ����� ������ ������������� �����������)
    void CheckPosition()
    {
        // ���� ����� ��� Hand ��� ������ ����� (� ������ �� ������ ������ �������������), �� ��������� �������� �� ����������� ������� �����
        if (SideToSide || ToHand)
        {
            TempCardGO.transform.SetSiblingIndex(PrevIndex);
        }
        // ����� �� ��� � ��������� ������� ����� ����� ���� (����, ������� ��� ��� ����)
        else
        {
            int newIndex = DefaultTempCardParent.childCount;
            for (int i = 0; i < DefaultTempCardParent.childCount; i++)
            {
                if (CompareAxis(transform.position, DefaultTempCardParent.GetChild(i).position))
                {
                    newIndex = i;
                    if (TempCardGO.transform.GetSiblingIndex() < newIndex) newIndex--;

                    break;
                }
            }

            TempCardGO.transform.SetSiblingIndex(newIndex);
        }
    }

    bool CompareAxis(Vector3 MoveCardCoordinate, Vector3 CompareCardCoordinate)
    {
        if (GameManagerScript.Instance.form == Formation.Horizontal) return MoveCardCoordinate.x < CompareCardCoordinate.x;
        return MoveCardCoordinate.y > CompareCardCoordinate.y;
    }

    // ���� ��������������� ����� ����� �� ����� ������� ���� � ������ ��������� ��� ������
    bool SideToSide { get {
            FieldType start = PrevParent.GetComponent<DropPlaceScript>().Type;
            FieldType end = DefaultTempCardParent.GetComponent<DropPlaceScript>().Type;

            return (start == FieldType.LField && end == FieldType.RField) ||
                (start == FieldType.RField && end == FieldType.LField);
        }
    }

    
    // ���� ����� ��������������� �� Hand ����-��
    bool FromHand { get {
            return PrevParent.GetComponent<DropPlaceScript>().Type == FieldType.HAND;
        }
    }

    
    // ���� ��������������� ����� ������ ��� Hand
    bool ToHand { get {
            return DefaultTempCardParent.GetComponent<DropPlaceScript>().Type == FieldType.HAND;
        }
    }

    bool EnoughMoney(Transform field, int cost)
    {
        if (PrevParent == field) return true;

        return GameManagerScript.Instance.EnoughMoney(field, cost);
    }

    ICard SelfCard
    { get {
            return transform.GetComponent<CardControler>().SelfCard;
        }
    }
}
