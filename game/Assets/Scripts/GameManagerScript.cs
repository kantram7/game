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

    public bool BlockChanges = false; // отслеживать как-то, если нажата "сделать ьход" при пустых полях

    List<((List<(ICard, double, Effect)>, int), (List<(ICard, double, Effect)>, int))> history;

    public int CurIndex = 0;

    public bool NoPrevHistory()
    {
        return CurIndex == 0;
    }

    public void MakeMovie()
    {
        CurIndex++;
    }

    public bool NoNextHistory()
    {
        return CurIndex >= history.Count - 1;
    }

    public void ClearHistory()
    {
        history = new List<((List<(ICard, double, Effect)>, int), (List<(ICard, double, Effect)>, int))>();
        CurIndex = 0;
    }

    public void CreateHistory(List<(ICard, double, Effect)> lCards, int lSum, List<(ICard, double, Effect)> rCards, int rSum)
    {

        history = history.RemoveFrom(CurIndex); // check

        history.Add(((lCards, lSum), (rCards, rSum)));

    }

    public ((List<(ICard, double, Effect)>, int), (List<(ICard, double, Effect)>, int)) GetHistory(int index)
    {
        return history[index];
    }

    public Game()
    {
        HandCards = new List<ICard>(CardManager.AllCards);
    }

    // Получить рандомный набор карт на переданную сумму
    public List<ICard> GiveRandomCards(int money)
    {

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

public enum Formation
{
    Vertical,
    Horizontal,
    Three
}

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript Instance;

    const int GoldAmount = 120;

    public Game CurGame;
    public Transform LField, RField, Hand, VLField, VRField;
    public GameObject Card, Message;

    public Button NewGame, RandomLeft, RandomRight;
    public Button PrevMovie, NexMovie, MakeMovie,
                  TestButton, TestButton_2;

    public Text LMoney, RMoney, VictoryResult;
    public int LMoneySum, RMoneySum;

    List<(ICard, double, Effect)> LCards = new List<(ICard, double, Effect)>(),
                                  RCards = new List<(ICard, double, Effect)>();

    public ScrollRect log;


    public GameObject LockScreen;

    public Formation form = Formation.Horizontal;

    bool Turn
    {
        get { return _turn; }
        set { _turn = value; }
    }
    bool _turn;
    void UpdateTurn()
    {
        Turn = UnityEngine.Random.Range(0, 2) == 1;
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

        NexMovie.onClick.AddListener(ReturnNexMovie);
        PrevMovie.onClick.AddListener(ReturnPrevMovie);

        TestButton.onClick.AddListener(() => { Test(121); }); // Test
        TestButton_2.onClick.AddListener(ChangeToVertical);

        GiveFieldCards(CurGame.HandCards, Hand);

        MakeNewGame();
    }

    void ChangeToVertical()
    {
        form = Formation.Vertical;


        LField.gameObject.SetActive(false);
        VLField.gameObject.SetActive(true);
        RField.gameObject.SetActive(false);
        VRField.gameObject.SetActive(true);
        LField = VLField;
        RField = VRField;

    }

    void ReturnNexMovie() // test
    {
        PrevMovie.interactable = true;
        ReturnHistory(++CurGame.CurIndex);

        if (CurGame.NoNextHistory()) NexMovie.interactable = false;
    }

    void ReturnPrevMovie() // test
    {
        NexMovie.interactable = true;

        if (CurGame.NoNextHistory())  CreateHistory();

        ReturnHistory(--CurGame.CurIndex);

        if (CurGame.NoPrevHistory()) PrevMovie.interactable = false;
    }

    void ReturnHistory(int index)
    {
        ClearFields();
        ((List<(ICard, double, Effect)>, int), (List<(ICard, double, Effect)>, int)) movie = CurGame.GetHistory(index);

        // возврат золота
        LMoneySum = movie.Item1.Item2;
        RMoneySum = movie.Item2.Item2;
        UpdateGold();

        List<(ICard, double, Effect)> LCardsAdd = movie.Item1.Item1;
        List<(ICard, double, Effect)> RCardsAdd = movie.Item2.Item1;

        foreach (var cardInfo in LCardsAdd)
        {
            CreateCardPref(cardInfo.Item1, LField, -1, (CardControler CC) =>
            {
                CC.AddEffect(cardInfo.Item3);
                CC.UpdateCurrentHealth(cardInfo.Item2);
            });
        }
        foreach (var cardInfo in RCardsAdd)
        {
            CreateCardPref(cardInfo.Item1, RField, -1, (CardControler CC) =>
            {
                CC.AddEffect(cardInfo.Item3);
                CC.UpdateCurrentHealth(cardInfo.Item2);
            });
        }
    }

    void ReadCardsToLists()
    {
        LCards = new List<(ICard, double, Effect)>();
        RCards = new List<(ICard, double, Effect)>();

        foreach (Transform card in RField)
        {
            CardControler CCard = card.GetComponent<CardControler>();
            RCards.Add((CCard.SelfCard, CCard.CurrentHealth, CCard.CurEffect));
        }
        foreach (Transform card in LField)
        {
            CardControler CCard = card.GetComponent<CardControler>();
            LCards.Add((CCard.SelfCard, CCard.CurrentHealth, CCard.CurEffect));
        }
    }

    void CreateHistory()
    {
        ReadCardsToLists();
        CurGame.CreateHistory(LCards, LMoneySum, RCards, RMoneySum);
    }

    void Test(int i)
    {
        UpdateFields();
    }

         
    void Movie()
    {
        if (CheckVictory()) return; // чтоб не было ошибки при пустом поле (полях). Лучше кнопку хода при этом блокировать, конечно
        // или булю какую сделать, что хоть одна карта есть там и там

        PrintMovieInfo("----------- НОВЫЙ ХОД ------------");

        CreateHistory();
        CurGame.MakeMovie();

        CurGame.BlockChanges = true;

        RandomLeft.interactable = false;
        RandomRight.interactable = false;
        PrevMovie.interactable = true;
        NexMovie.interactable = false;

        // механизм хода
        // .........
        UpdateTurn();

        PrintMovieInfo("Its turn " + PrintTurn() + "player");

        CloseAttack(Turn);
        SpecAttacks(Turn);

        if (CheckVictory()) return;

    }

    void MakeNewGame()
    {
        CurGame.ClearHistory();
        ClearLog();

        CurGame.BlockChanges = false;

        LockScreen.SetActive(false);


        PrevMovie.interactable = false;
        NexMovie.interactable = false;
        RandomLeft.interactable = true;
        RandomRight.interactable = true;
        MakeMovie.interactable = true;

        LMoneySum = RMoneySum = GoldAmount;
        UpdateGold();

        ClearFields();
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
        Debug.Log(cards[0].Name);

        foreach (ICard card in cards)
        {
            GiveFieldOneCard(card, field, field == LField && form == Formation.Horizontal ? 0 : -1); // убрать передачу индекса, есть reverse (да?)
        }
    }

    public void GiveFieldOneCard(ICard card, Transform field, int index = -1)
    {
        CreateCardPref(card, field, index);
    }

    void CreateCardPref(ICard card, Transform field, int index = -1, Action<CardControler> MutCard = null)
    {
        GameObject cardGO = Instantiate(Card, field, false);
        CardControler cardContr = cardGO.GetComponent<CardControler>();

        if (index != -1)
        {
            cardGO.transform.SetSiblingIndex(index);
        }

        cardContr.Init(card);

        MutCard?.Invoke(cardContr);
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
        foreach (Transform field in new List<Transform> { RField, LField }) {
            foreach (Transform card in field)
            {
                Destroy(card.gameObject);
            }
        }
        LCards = new List<(ICard, double, Effect)>();
        RCards = new List<(ICard, double, Effect)>();
    }

    void UpdateFields()
    {
        foreach (Transform field in new List<Transform> { RField, LField })
        {
            foreach (Transform card in field)
            {
                CardControler curCard = card.GetComponent<CardControler>();
                if (!curCard.IsAlive()) Destroy(card.gameObject);
            }
        }
        CheckVictory();
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
        CardControler TurnFirstCard, TurnSecondCard;

        if (turn)
        {
            TurnFirstCard = (RField).GetChild(0).GetComponent<CardControler>();
            TurnSecondCard = (LField).GetChild(LField.childCount - 1).GetComponent<CardControler>();
        }
        else
        {
            TurnSecondCard = (RField).GetChild(0).GetComponent<CardControler>();
            TurnFirstCard = (LField).GetChild(LField.childCount - 1).GetComponent<CardControler>();
        }

        TurnSecondCard.GetDamage(TurnFirstCard, AttackType.CLOSE); // механизм сколько отнимать жизни в CC
        TurnFirstCard.GetDamage(TurnSecondCard, AttackType.CLOSE); // ответная атака

    }

    void DistantAttack(CardControler attackCard, Transform enemyField, int index) // нужен ли?
    {
        int atackedPosition = UnityEngine.Random.Range(0, (enemyField.childCount >= 3 ? 3 : enemyField.childCount));
        CardControler enemyCard = enemyField.GetChild(atackedPosition).GetComponent<CardControler>();
        enemyCard.GetDamage(attackCard, AttackType.DISTANT);
    }

    void SpecAttacks(bool turn)
    {
        List<Transform> fields = new List<Transform>{ (turn ? RField : LField), (turn ? LField : RField) };

        for(int j = 0; j < 2; j++)
        {
            Transform curField = fields[j];

            for (int i = 0; i < curField.childCount; i++)
            {
                CardControler CurCard = curField.GetChild(i).GetComponent<CardControler>();

                switch (CurCard.WhichAbilityHas())
                {
                    case HasAbilities.HEAL:
                        foreach(Transform card in NearCards(curField, i))
                        {
                            CardControler cardCC = card.GetComponent<CardControler>();
                            if (cardCC.CanUseAbility(UseAbilities.HEAL)) cardCC.GetHeal();
                        }
                        break;

                    case HasAbilities.APP:
                        //Debug.Log(CurCard.name + " can app " + NearCards(curField, i).Count);
                        foreach (Transform card in NearCards(curField, i))
                        {
                            CardControler cardCC = card.GetComponent<CardControler>();
                            if (cardCC.CanUseAbility(UseAbilities.APP)) cardCC.GetApp();
                        }
                        break;
                    case HasAbilities.CLONE:
                        foreach (Transform card in NearCards(curField, i))
                        {
                            CardControler cardCC = card.GetComponent<CardControler>();
                            if (cardCC.CanUseAbility(UseAbilities.CLONE) && cardCC.Clone())
                            {
                                CreateCardPref(cardCC.SelfCard, curField, card.GetSiblingIndex()); // Нужно клонировать с тем же здоровьем или как новую?
                            }
                        }
                        break;
                    case HasAbilities.DISTANT:
                        if(!FirstCards(i, curField)) DistantAttack(CurCard, (j == 0 ? fields[1] : fields[0]), i);
                        break;

                    default: break;
                }
            }

        }
    }

    List<Transform> NearCards(Transform field, int index)
    {
        List<Transform> nearCards = new List<Transform>();

        if (FirstCards(index, field)) return nearCards; // первые карты

        if(field == LField)
        {
            if (index != 0) nearCards.Add(field.GetChild(index - 1));
            nearCards.Add(field.GetChild(index + 1));
        }
        else
        {
            if (index != field.childCount - 1) nearCards.Add(field.GetChild(index + 1));
            nearCards.Add(field.GetChild(index - 1));
        }
        return nearCards;
    }

    bool FirstCards(int index, Transform field)
    {
        if ((field == RField && index == 0) || (field == LField && index == LField.childCount - 1)) return true;
        return false;
    }

    bool CheckVictory()
    {
        if (LField.childCount != 0 && RField.childCount != 0)
        {
            return false;
        }

        if (LField.childCount == 0 && RField.childCount == 0)
        {
            EndGame("Ничья");
        }
        else if (LField.childCount == 0)
        {
            EndGame("Right win");
        }
        else EndGame("Left win");

        return true;
    }

    void EndGame(string result)
    {
        PrintMovieInfo(result);

        LockScreen.SetActive(true);
        MakeMovie.interactable = false;
        RandomLeft.interactable = false;
        RandomRight.interactable = false;
        VictoryResult.text = result;
    }

    public void PrintMovieInfo(string info)
    {
        //Debug.Log(info);

        GameObject message = Instantiate(Message, log.content, false);

        message.GetComponent<Text>().text = info;

        log.velocity = new Vector2(0f, 1000f);
    }

    public void ClearLog()
    {
        foreach (Transform mes in log.content.transform)
        {
            GameObject.Destroy(mes.gameObject);
        }
    }

    public string PrintTurn(bool b = true)
    {
        if (Turn && b) return "Right";
        return "Left";
    }
}
