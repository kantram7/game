using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

public class CardScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Camera MainCamera;
    Vector3 offset;
    public Transform DefaultParent, DefaultTempCardParent;
    GameObject TempCardGO, TempCardClone;

    void Awake()
    {
        MainCamera = Camera.allCameras[0];
        TempCardGO = GameObject.Find("TempCardGO");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        TempCardClone = Instantiate(this.gameObject, transform.position, transform.rotation, transform.parent) as GameObject;
        TempCardClone.transform.localScale = new Vector3(1, 1); // Нужно?


        //TempCardGO.transform.SetParent(DefaultParent);
        //TempCardGO.transform.localScale = new Vector3(1, 1, 1);
        

        offset = transform.position - MainCamera.ScreenToWorldPoint(eventData.position);

        DefaultParent = transform.parent;
        DefaultTempCardParent = transform.parent;

        TempCardGO.transform.SetParent(DefaultParent);
        TempCardGO.transform.SetSiblingIndex(transform.GetSiblingIndex());

        transform.SetParent(DefaultParent.parent);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 newPos = MainCamera.ScreenToWorldPoint(eventData.position);
        newPos.z = 0;
        transform.position = newPos + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Destroy(this);

        transform.SetParent(DefaultParent);     

        GetComponent<CanvasGroup>().blocksRaycasts = true;

        TempCardGO.transform.SetParent(GameObject.Find("Canvas").transform);
        TempCardGO.transform.localPosition = new Vector3(2500, 0);

    }
}
