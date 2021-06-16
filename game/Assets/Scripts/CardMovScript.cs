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

    GameManagerScript Instance;

    Transform Board;

    public CardControler CC;

    void Awake()
    {
        MainCamera = Camera.allCameras[0];
        TempCardGO = GameObject.Find("TempCardGO");

        Board = GameObject.Find("MainBoard").transform;

        Instance = GameManagerScript.Instance;
    }

    // ������ ��������������
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Instance.CurGame.BlockChanges) // �� �����
        {
            eventData.pointerDrag = null;
            return;
        }

        offset = transform.position - MainCamera.ScreenToWorldPoint(eventData.position);

        DefaultParent = DefaultTempCardParent = PrevParent = transform.parent;

        PrevIndex = transform.GetSiblingIndex();

        if (FromHand)
        {
            Instance.GiveFieldOneCard(SelfCard, PrevParent, PrevIndex);
        }
        else if(ThreeFormation && !CanTakeFromSubField(PrevParent))
        {
            return;
        }
        else
        {
            TempCardGO.transform.SetParent(DefaultParent);
            TempCardGO.transform.SetSiblingIndex(transform.GetSiblingIndex());

            getDropPlaceScript(DefaultParent).countCards--;
        }

        transform.SetParent(Board);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    // �� ����� ��������������
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 newPos = MainCamera.ScreenToWorldPoint(eventData.position);
        transform.position = newPos + offset;

        if ((TempCardGO.transform.parent != DefaultTempCardParent) && !ToHand && !SideToSide && EnoughMoney(DefaultTempCardParent, SelfCard.Cost))
        {
            //Debug.Log("error");
            if (ThreeFormation && (!CanMoveToSubField(DefaultTempCardParent) || !CanTakeFromSubField(PrevParent))) return;

            TempCardGO.transform.SetParent(DefaultTempCardParent);
        }

        CheckPosition();
    }

    // ��������� ��������������
    public void OnEndDrag(PointerEventData eventData)
    {
        // ���� ����� ���������� � Hand - ����� ���������
        // 3-3 +
        if (ToHand)
        {
            //Debug.Log("Destroy " + this.name);

            if (!FromHand)
                Instance.ChangeMoney(ThreeFormation ? Instance.SpecifyField(PrevParent) : PrevParent, -SelfCard.Cost);

            CC.DestroyCard();
        }
        // ���� �������������� �� � ������ ���� �� ������
        else if (!SideToSide && EnoughMoney(DefaultParent, SelfCard.Cost) && (!ThreeFormation || getDropPlaceScript(DefaultParent).countCards != 3))
        {
           // if (!(ThreeFormation && !CanMoveToSubField(DefaultParent)))

            // ���� �������� 3-3 � �������������� � �������� ����. � ������ ��������
            // ���� �������� ������� � ������� �����
            // EnoughMoney ���������� ������� ���� � �������� ������ ����

            //Debug.Log("else");

           transform.SetParent(DefaultParent);
           transform.SetSiblingIndex(TempCardGO.transform.GetSiblingIndex());

            getDropPlaceScript(DefaultParent).countCards++;

           if (getDropPlaceScript(DefaultParent).Type != getDropPlaceScript(PrevParent).Type)
                Instance.ChangeMoney(ThreeFormation ? Instance.SpecifyField(DefaultParent) : DefaultParent, SelfCard.Cost);

        }
        else if(FromHand)
        {
            CC.DestroyCard();
        }
        // ����� ���������� �� �������� �����
        else
        {
            transform.SetParent(PrevParent);
            transform.SetSiblingIndex(TempCardGO.transform.GetSiblingIndex());

            getDropPlaceScript(PrevParent).countCards++;
        }

        GetComponent<CanvasGroup>().blocksRaycasts = true;

        Instance.UpdateTreeFields();

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
        if (Instance.form == Formation.Horizontal) return MoveCardCoordinate.x < CompareCardCoordinate.x;
        return MoveCardCoordinate.y > CompareCardCoordinate.y;
    }

    // ���� ��������������� ����� ����� �� ����� ������� ���� � ������ ��������� ��� ������
    bool SideToSide { get {
            FieldType start = getDropPlaceScript(PrevParent).Type;
            FieldType end = getDropPlaceScript(DefaultTempCardParent).Type;

            return (start == FieldType.LField && end == FieldType.RField) ||
                (start == FieldType.RField && end == FieldType.LField);
        }
    }

    
    // ���� ����� ��������������� �� Hand ����-��
    bool FromHand { get {
            return getDropPlaceScript(PrevParent).Type == FieldType.HAND;
        }
    }

    
    // ���� ��������������� ����� ������ ��� Hand
    bool ToHand { get {
            return getDropPlaceScript(DefaultTempCardParent).Type == FieldType.HAND;
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


    public bool CanTakeFromSubField(Transform from)
    {
        if (getDropPlaceScript(from).Type == FieldType.HAND) return true;

        return from.childCount <= Instance.GetNeighbour(from).childCount;
    }

    public bool CanMoveToSubField(Transform to, int max = 0)
    {
        Debug.Log(to.name);

        return to.childCount <= Instance.GetNeighbour(to).childCount && to.childCount < 3 + max;
    }

    public bool ThreeFormation
    {
        get { return Instance.form == Formation.Three; }
    }


    DropPlaceScript getDropPlaceScript(Transform field)
    {
        return field.GetComponent<DropPlaceScript>();
    }
}
