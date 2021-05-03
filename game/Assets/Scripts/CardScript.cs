using System.Collections;
using System.Collections.Generic;

using System.Runtime.CompilerServices;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Camera MainCamera;
    Vector3 offset;
    public Transform PrevParent, DefaultParent, DefaultTempCardParent;
    GameObject TempCardGO, TempCardClone;
    int PrevIndex;

    void Awake()
    {
        MainCamera = Camera.allCameras[0];
        TempCardGO = GameObject.Find("TempCardGO");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        //TempCardGO.transform.SetParent(DefaultParent);
        //TempCardGO.transform.localScale = new Vector3(1, 1, 1);


        offset = transform.position - MainCamera.ScreenToWorldPoint(eventData.position);

        DefaultParent = DefaultTempCardParent = PrevParent = transform.parent;

        PrevIndex = transform.GetSiblingIndex();

        Debug.Log("Z- " + transform.position.z + ", localZ- " + transform.localPosition.z);

        if (FromHand())
        {
            TempCardClone = Instantiate(this.gameObject, transform.position, transform.rotation, transform.parent);
            TempCardClone.transform.SetSiblingIndex(transform.GetSiblingIndex());
            TempCardClone.name = name;

            //TempCardClone.transform.localPosition = new Vector3(1, 1, 0); // Нужно? 
            // z координата картинки уменьшается в 2 раза
            // а первончальная вообще хуй знает где по отношению к ост. элементам

            Debug.Log("Clone Z- " + transform.position.z + ", localZ- " + transform.localPosition.z);
        }
        else
        {
            TempCardGO.transform.SetParent(DefaultParent);
            TempCardGO.transform.SetSiblingIndex(transform.GetSiblingIndex());
        }

        transform.SetParent(DefaultParent.parent);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {

        Vector3 newPos = MainCamera.ScreenToWorldPoint(eventData.position);
        newPos.z = 0;
        transform.position = newPos + offset;

        // TempCardGO.transform.parent != DefaultTempCardParent --- ?
        if (TempCardGO.transform.parent != DefaultTempCardParent && !ToHand() && !SideToSide())
            TempCardGO.transform.SetParent(DefaultTempCardParent);

        CheckPosition();

        //Debug.Log("Current- " + DefaultParent.name);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("EndDir-2- " + DefaultParent.name

        if (ToHand())
        {
            Debug.Log("Destroy " + this.name);
            Destroy(this.gameObject);
        }

        else if (!SideToSide())
        {
            transform.SetParent(DefaultParent);
            transform.SetSiblingIndex(TempCardGO.transform.GetSiblingIndex());

            //GameObject gold = GameObject.Find("LeftPlayerGold");
            //gold.transform.GetChild(1).gameObject.t
        }
        else
        {
            transform.SetParent(PrevParent);
            transform.SetSiblingIndex(TempCardGO.transform.GetSiblingIndex());
        }

        GetComponent<CanvasGroup>().blocksRaycasts = true;

        // убрать временную обраттно за камеру
        TempCardGO.transform.SetParent(GameObject.Find("Canvas").transform);
        TempCardGO.transform.localPosition = new Vector3(2500, 0);

        //Debug.Log("EndDir-1- " + DefaultParent.name);

        //transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
    }

    void CheckPosition()
    {
        if (SideToSide() || ToHand())
        {
            TempCardGO.transform.SetSiblingIndex(PrevIndex);
            return;
        }
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool SideToSide()
    {
        return (PrevParent.name == "LField" && DefaultTempCardParent.name == "RField") ||
            (PrevParent.name == "RField" && DefaultTempCardParent.name == "LField");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool FromHand()
    {
        return PrevParent.name == "Hand";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool ToHand()
    {
        return DefaultTempCardParent.name == "Hand";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool OutOfField()
    {
        return (PrevParent.name == "LField" && DefaultTempCardParent.name == "LField") ||
            (PrevParent.name == "RField" && DefaultTempCardParent.name == "RField");
    }
}
