using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;

using Assets.Scripts.Units;
using Assets.Scripts.Additions;

public class Game
{
    public List<ICard> HandCards;

    public bool BlockChanges = false;

    public Game()
    {
        HandCards = new List<ICard>(CardManager.AllCards);
    }

    // ѕолучить рандомный набор карт на переданную сумму
    public List<ICard> GiveRandomCards(int money)
    {
        //Debug.Log("starts GiveRandomCards");

        List<ICard> cards = new List<ICard>();

        int minCost = CardManager.AllCards.Select(card => card.Cost).Min();

        while (money >= minCost)
        {
            ICard card = CardManager.AllCards[UnityEngine.Random.Range(0, CardManager.AllCards.Count)];
            if((money - card.Cost) >= 0)
            {
                cards.Add(card);
                money = money - card.Cost;
            }
            
        }
        return cards;
    }
}

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript Instance;

    const int GoldAmount = 120;

    public Game CurGame;
    public Transform LField, RField, Hand;
    public GameObject Card;

    public Button NewGame, RandomLeft, RandomRight;
    public Button PrevMovie, NexMovie, MakeMovie,
                  TestButton;

    public Text LMoney, RMoney;
    public int LMoneySum, RMoneySum;

    List<CardControler> LCards = new List<CardControler>(),
                        RCards = new List<CardControler>();

    List<((List<CardControler>, int), (List<CardControler>, int))> history;

    public GameObject LockScreen;

    bool Turn
    {
        get { return UnityEngine.Random.Range(0, 2) == 1; }
    }

    private void Awake()
    {
        if(Instance == null) Instance = this;
    }


    void Start()
    {
        CurGame = new Game();

        NewGame.onClick.AddListener(MakeNewGame);
        RandomLeft.onClick.AddListener(() => { RandomField(LField, LMoney); });
        RandomRight.onClick.AddListener(() => { RandomField(RField, RMoney); });
        MakeMovie.onClick.AddListener(Movie);

        TestButton.onClick.AddListener(() => { Test(121); }); // Test

        GiveFieldCards(CurGame.HandCards, Hand);

        MakeNewGame();
    }

    void ReadCardsToLists()
    {
        foreach(Transform card in RField)
        {
            RCards.Add(card.GetComponent<CardControler>());
        }
        foreach (Transform card in LField)
        {
            LCards.Add(card.GetComponent<CardControler>());
        }
        LCards.Reverse();
    }

    void CreateHistory()
    {

        (List<CardControler>, int) LPair = (LCards, LMoneySum),
                                    RPair = (RCards, RMoneySum);

        ((List<CardControler>, int), (List<CardControler>, int)) pair = ( LPair, RPair );

        history.Add(pair);
    }

    void Test(int i)
    {
        Debug.Log("Test == " + i);
    }

         
    void Movie()
    {
        CheckVictory(); // чтоб не было ошибки при пустом поле (пол€х). Ћучше кнопку хода при этом блокировать, конечно
        // или булю какую сделать, что хоть одна карта есть там и там

        ReadCardsToLists();

        if (RCards.Count + LCards.Count == 0) return; // что-нибудь придумать

        CreateHistory();

        CurGame.BlockChanges = true;

        RandomLeft.interactable = false;
        RandomRight.interactable = false;
        PrevMovie.interactable = true;
        NexMovie.interactable = true;

        // механизм хода
        // .........
        bool turn = Turn;
        Debug.Log("Its turn " + (turn ? "Right " : "Left ") + "player");
        CloseAttack(turn);
        SpecAttacks(turn);

        CheckVictory();
    }

    void MakeNewGame()
    {
        //Debug.Log("starts MakeNewGame");

        CurGame.BlockChanges = false;

        PrevMovie.interactable = false;
        NexMovie.interactable = false;
        RandomLeft.interactable = true;
        RandomRight.interactable = true;

        LMoneySum = RMoneySum = GoldAmount;
        UpdateGold();

        ClearFields();

        history = new List<((List<CardControler>, int), (List<CardControler>, int))>();
    }

    void RandomField(Transform field, Text money)
    {

        int curMoney = int.Parse(money.text);


        List<ICard> cards = CurGame.GiveRandomCards(curMoney);


        GiveFieldCards(cards, field);

        ChangeMoney(field, cards.Select(card => card.Cost).Sum());
    }

    void GiveFieldCards(List<ICard> cards, Transform field)
    {
        //Debug.Log("Cards to " + field.name);

        foreach (ICard card in cards)
        {
            GiveFieldOneCard(card, field, field == LField ? 0 : -1);
        }
    }

    public void GiveFieldOneCard(ICard card, Transform field, int index = -1)
    {
        //Debug.Log("One card to " + field.name + " - (" + card.Name + ")");

        CreateCardPref(card, field, index);
    }

    void CreateCardPref(ICard card, Transform field, int index = -1)
    {
        GameObject cardGO = Instantiate(Card, field, false);
        CardControler cardContr = cardGO.GetComponent<CardControler>();

        if (index != -1)
        {
            cardGO.transform.SetSiblingIndex(index);
        }

        cardContr.Init(card);
    }


    public void ChangeMoney(Transform field, int changeCount)
    {
        if (field == RField) RMoneySum -= changeCount;
        else if (field == LField) LMoneySum -= changeCount;
        else throw new ArgumentException("ChangeMoney get `" + field.name + "` field");

        UpdateGold();
    }

    void ClearFields()
    {
        foreach (Transform card in RField)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in LField)
        {
            Destroy(card.gameObject);
        }
        LCards = RCards = new List<CardControler>();
    }

    void UpdateFields()
    {

    }

    public bool EnoughMoney(Transform field, int cost)
    {
        if (field == RField) return cost <= RMoneySum;
        else if (field == LField) return cost <= LMoneySum;
        else throw new ArgumentException("EnoughMoney get `" + field.name + "` field");
    }

    void UpdateGold()
    {
        LMoney.text = LMoneySum.ToString();
        RMoney.text = RMoneySum.ToString();
    }


    void CloseAttack(bool turn)
    {
        CardControler RFirstCard = RField.GetChild(0).GetComponent<CardControler>();
        CardControler LFirstCard = LField.GetChild(LField.childCount - 1).GetComponent<CardControler>();

        if (turn)
        {
            LFirstCard.GetDamage(RFirstCard.SelfCard.Attack); // механизм сколько отнимать жизни в CC
            RFirstCard.OnDamageDeal(LFirstCard.SelfCard.Attack);

            Debug.Log((turn ? "Right " : "Left ") + RFirstCard.SelfCard.Name + " attack " + (!turn ? "right " : "reft ") + LFirstCard.SelfCard.Name + 
                " with damage " + RFirstCard.SelfCard.Attack + " and back damage " + LFirstCard.SelfCard.Attack * 0.1);
        }
        else
        {
            RFirstCard.GetDamage(LFirstCard.SelfCard.Attack);
            LFirstCard.OnDamageDeal(RFirstCard.SelfCard.Attack);

            Debug.Log((turn ? "Right " : "Left ") + LFirstCard.SelfCard.Name + " attack " + (!turn ? "right " : "reft ") + RFirstCard.SelfCard.Name +
                " with damage " + LFirstCard.SelfCard.Attack + " and back damage " + RFirstCard.SelfCard.Attack * 0.1);
        }
        //Debug.Log("CLOSE ATTACK@@#");
    }

    void DistAttack()
    {

    }

    void CheckKilled() // нужен ли?
    {

    }

    void SpecAttacks(bool turn)
    {
        for(int i = 1; i < RField.childCount; i++)
        {
            CardControler CurCard = RField.GetChild(i).GetComponent<CardControler>();

            if(CurCard.WhichAbilityHas() == HasAbilities.HEAL)
            {
                CardControler PrevCard = RField.GetChild(i - 1).GetComponent<CardControler>();
                if (PrevCard.CanUseAbility(UseAbilities.HEAL))
                {
                    PrevCard.GetHeal();
                    Debug.Log("Right Healer heal " + PrevCard.SelfCard.Name);
                }
                // говнокод
                if (i == RField.childCount - 1) continue;

                CardControler NextCard = RField.GetChild(i + 1).GetComponent<CardControler>();
                if (((AbilityClass)NextCard.SelfCard).CanUseAbility(UseAbilities.HEAL))
                {
                    NextCard.GetHeal();
                    Debug.Log("Right Healer heal " + NextCard.SelfCard.Name);
                }
            }
        }
    }

    void CheckVictory()
    {
        if (LField.childCount + RField.childCount == 0)
        {
            LockScreen.SetActive(true);
        }
        else if (RField.childCount == 0) return;
        else if (LField.childCount == 0) return;
        else return;
    }
}
