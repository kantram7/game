using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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

    int _currentHaelth;
    public int CurrentHealth
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
        if (!IsAlive() || !enemyCard.IsAlive() || !enemyCard.CanAttack) return;

        if(CurEffect == Effect.APP)
        {
            if (RamdomPersent(damageType == AttackType.CLOSE ? 0.5 : 0.7))
            {
                AddEffect(Effect.NO);
                DestroyApp();
                GameManager.PrintMovieInfo(enemyCard.SelfCard.Name + " ???? ?????? ? " + GameManager.PrintTurn() + SelfCard.Name);
            }
            else
            {
                GameManager.PrintMovieInfo(enemyCard.SelfCard.Name + " ?? ?????? ?????? ? " + SelfCard.Name);
            }
            return;
        }

        double enemyAttackCorected = enemyCard.SelfCard.Attack * ((1.0 / enemyCard.SelfCard.Health) * enemyCard.CurrentHealth);
        double defenceCorected = SelfCard.Defence * ((1.0 / SelfCard.Health) * CurrentHealth);

        double resultDamage = (enemyAttackCorected / defenceCorected) * enemyAttackCorected;

        if (damageType == AttackType.DISTANT)
            resultDamage *= .5;


        UpdateCurrentHealth(CurrentHealth - resultDamage);
        GameManager.PrintMovieInfo(enemyCard.SelfCard.Name + " ????? " + Math.Round(resultDamage, 2) + " ????? " + SelfCard.Name
            + (damageType == AttackType.CLOSE ? "" : "(????. ?????)"));

        CheckAlive();
    }
    public void AddEffect(Effect effect)
    {
        this.CurEffect = effect;
        CardInfo.ShowEffect(effect);
    }

    public void UpdateCurrentHealth(double newCurHealth)
    {
        CurrentHealth = (int)(newCurHealth < 0 ? 0 : newCurHealth);
        CardInfo.UpdateHealth();
    }


    // ????? ????? ????????, ??? ????, ? ????? ???? ??????? ?????
    public void CheckAlive()
    {
        if (!IsAlive()) 
        {
            AddEffect(Effect.DEAD);
            GameManager.PrintMovieInfo(SelfCard.Name + " ?????????... ");
        }
    }

    public void GetHeal()
    {
        Debug.Log("??????? ???????");

        if (!RamdomPersent(0.3)) return;

        if (CurrentHealth < SelfCard.Health && IsAlive())
        {
            double healedSum = ((SelfCard.Health - CurrentHealth) * 0.5);
            UpdateCurrentHealth(CurrentHealth + healedSum);
            GameManager.PrintMovieInfo(SelfCard.Name + " ??????????? " + healedSum + " ????????");
        }
    }

    public void GetApp()
    {
        Debug.Log("app ???????");

        if (RamdomPersent(0.5) || !IsAlive() || CurEffect == Effect.APP) return;

        // ???????? ??????????? ???? ? gamemanager

        AddEffect(Effect.APP);

        GameManager.PrintMovieInfo("??? ??????????");
    }

    public double Distant()
    {
        return SelfCard.Attack * 0.5;
    }

    public bool HasDistant()
    {
        return WhichAbilityHas() == HasAbilities.DISTANT;
    }

    public bool Clone()
    {
        Debug.Log("clone ???????");

        if (!RamdomPersent(0.1) || !IsAlive())
            return false;

        GameManager.PrintMovieInfo("?????????? " + SelfCard.Name);
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

    public bool CanAttack
    {
        get {
            return SelfCard.Attack > 0;
        }
    }

    public void DestroyCard()
    {
        //CardMov.OnEndDrag(null);

        Destroy(gameObject);
    }
}
