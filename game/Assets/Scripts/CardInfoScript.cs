using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Assets.Scripts.Units;


public class CardInfoScript : MonoBehaviour
{
    public CardControler CC;

    public Text Name, Cost, Defence, Attack, Health;
    public Image ImageLogo, CurEffect;
    public GameObject Abilities;

    public void ShowCardInfo()
    {
        transform.name = CC.SelfCard.Name;

        //Debug.Log("Create " + transform.name);

        ImageLogo.sprite = ((ICardLogo)CC.SelfCard).Logo;
        ImageLogo.preserveAspect = true;

        Name.text = CC.SelfCard.Name;
        Cost.text = CC.SelfCard.Cost.ToString();
        Defence.text = CC.SelfCard.Defence.ToString();
        Attack.text = CC.SelfCard.Attack.ToString();
        Health.text = CC.SelfCard.Health.ToString();

        // пример отрисовки абилок
        switch (CC.WhichAbilityHas())
        {
            case HasAbilities.DISTANT:
                Instantiate(GameObject.Find("IconExample"), Abilities.transform, false);
                break;
            case HasAbilities.APP:
                break;
            case HasAbilities.CLONE:
                break;
            case HasAbilities.HEAL:
                break;
            default:
                break;
        }
    }

    public void ShowEffect(Effect effect)
    {
        switch (effect)
        {
            case Effect.APP: return;
            case Effect.DEAD:
                CurEffect.sprite = Resources.Load<Sprite>("Sprites/test_img/dead");
                CurEffect.color = new Color(0, 0, 0, (float).5);
                CurEffect.gameObject.SetActive(true);
                break;
            default: return;
        }
    }

    public void UpdateHealth()
    {
        Health.text = ((int)CC.CurrentHealth).ToString();
    }
}
