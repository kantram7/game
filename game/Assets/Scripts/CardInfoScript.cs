using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CardInfoScript : MonoBehaviour
{
    public ICard SelfCard;
    public Text Name, Cost, Defence, Attack, Health;
    public Image ImageLogo;
    public GameObject Abilities;

    public void ShowCardInfo(ICard card)
    {
        SelfCard = card;

        transform.name = card.Name;

        Debug.Log("Create " + transform.name);

        ImageLogo.sprite = card.Logo;
        ImageLogo.preserveAspect = true;

        Name.text = card.Name;
        Cost.text = card.Cost.ToString();
        Defence.text = card.Defence.ToString();
        Attack.text = card.Attack.ToString();
        Health.text = card.Health.ToString();

        if (card.Abitilies.Count != 0)
        {
            foreach (Image abi in card.Abitilies) {

                Debug.Log("Abi");

                Instantiate(GameObject.Find("IconExample"), Abilities.transform, false);

            }
        }
    }

    private void Start()
    {
        //ShowCardInfo(CardManager.AllCards[transform.GetSiblingIndex()]);
    }
}
