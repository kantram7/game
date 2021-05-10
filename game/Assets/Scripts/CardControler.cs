using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Assets.Scripts.Units;

using System.Runtime.CompilerServices; // inline

public enum Effect
{
    NO,
    DEAD,
    APP
}

public class CardControler : MonoBehaviour
{
    public ICard SelfCard;
    public CardInfoScript CardInfo;
    public CardMovScript CardMov;

    public Effect CurEffect = 0;

    GameManagerScript GameManager;

    double _currentHaelth;
    public double CurrentHealth
    {
        get
        {
            return _currentHaelth;
        }
        set
        {
            _currentHaelth = value;
        }
    }



    public void Init(ICard card)
    {
        SelfCard = card;
        GameManager = GameManagerScript.Instance;

        CurrentHealth = card.Health;

        CardInfo.ShowCardInfo();
    }

    public AbilityClass ToAbilityClass()
    {
        return (AbilityClass)SelfCard;
    }

    public HasAbilities WhichAbilityHas()
    {
        return ToAbilityClass().HasAbility;
    }
    public bool CanUseAbility(UseAbilities abi)
    {
        return ToAbilityClass().CanUseAbility(abi);
    }

    public void GetDamage(double damage)
    {
        if(CurEffect == Effect.APP)
        {
            if (UnityEngine.Random.Range(0, 2) == 1) CurEffect = 0;
            return;
        }

        CurrentHealth -= ((SelfCard.Defence <= damage) ? (damage) : (damage + SelfCard.Defence) / 2);
        CheckAlive();
    }

    // ответный урон при атаке игнорирует щит
    // разные методы - потом изменить механизм на что-то более логичное (и разное для атаки/ответки)
    public void OnDamageDeal(double counterDamage)
    {
        CurrentHealth -= counterDamage * 0.1;
        CheckAlive();
    }
    // здесь потом картинку, что умер, и после хода удалить карту
    public void CheckAlive()
    {
        if (!IsAlive()) 
        {
            CurrentHealth = 0;
            CardInfo.ShowEffect(Effect.DEAD);
        }
        CardInfo.UpdateHealth();
    }

    public void GetHeal()
    {
        if (CurrentHealth < SelfCard.Health && CurrentHealth != 0) CurrentHealth += (SelfCard.Health - CurrentHealth) * 0.5;
        CardInfo.UpdateHealth();
    }

    public bool IsAlive()
    {
        return CurrentHealth > 0;
    }

    public void DestroyCard()
    {
        //CardMov.OnEndDrag(null);

        Destroy(gameObject);
    }
}
