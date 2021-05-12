using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Assets.Scripts.Units;
using static Assets.Scripts.Additions.Addition;

// using System.Runtime.CompilerServices; // inline

public enum Effect
{
    NO,
    DEAD,
    APP
}

public enum AttackType
{
    CLOSE,
    DISTANT
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

    public void GetDamage(CardControler enemyCard, AttackType damageType)
    {
        if (!IsAlive() || !enemyCard.IsAlive()) return;

        if(CurEffect == Effect.APP)
        {
            if (RamdomPersent(damageType == AttackType.CLOSE ? 0.5 : 0.7))
            {
                CurEffect = 0;
                DestroyApp();
                GameManager.PrintMovieInfo(SelfCard.Name + " снял защиту у " + enemyCard.SelfCard.Name);
            }
            else
            {
                GameManager.PrintMovieInfo(SelfCard.Name + " не пробил защиту у " + enemyCard.SelfCard.Name);
            }
            return;
        }

        int enemyAttack = enemyCard.SelfCard.Attack;
        double resultDamage;

        if(damageType == AttackType.CLOSE)
            resultDamage = ((SelfCard.Defence <= enemyAttack) ? (enemyAttack) : (enemyAttack + SelfCard.Defence) / 2);
        else
            resultDamage = ((SelfCard.Defence <= enemyAttack) ? (enemyAttack / 2) : (enemyAttack + SelfCard.Defence) / 4);

        CurrentHealth -= resultDamage;
        GameManager.PrintMovieInfo(enemyCard.SelfCard.Name + " нанес " + resultDamage + " урона " + SelfCard.Name
            + (damageType == AttackType.CLOSE ? "" : "(дист. атака)"));

        CheckAlive();
    }


    // здесь потом картинку, что умер, и после хода удалить карту
    public void CheckAlive()
    {
        if (!IsAlive()) 
        {
            CurrentHealth = 0;
            this.CurEffect = Effect.DEAD;
            CardInfo.ShowEffect(Effect.DEAD);
        }
        CardInfo.UpdateHealth();
    }

    public void GetHeal()
    {
        if (!RamdomPersent(0.3)) return;

        if (CurrentHealth < SelfCard.Health && IsAlive())
        {
            double healedSum = ((SelfCard.Health - CurrentHealth) * 0.5);
            CurrentHealth += healedSum;
            GameManager.PrintMovieInfo(SelfCard.Name + " восстановил " + healedSum + " здоровья");
        }

        CardInfo.UpdateHealth();
    }

    public void GetApp()
    {
        if (RamdomPersent(0.5)) return;

        // проверка возможности апов в gamemanager

        this.CurEffect = Effect.APP;
        CardInfo.ShowEffect(Effect.APP);

        GameManager.PrintMovieInfo("Щит установлен");
    }

    public double Distant()
    {
        return SelfCard.Attack * 0.5;
    }

    public bool Clone()
    {
        if (!RamdomPersent(0.1) || !IsAlive())
            return false;

        GameManager.PrintMovieInfo("Клонирован " + SelfCard.Name);
        return true;
    }

    public void DestroyApp()
    {
        this.CurEffect = Effect.NO;
        CardInfo.ShowEffect(Effect.NO);
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
