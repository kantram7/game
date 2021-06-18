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
    public Transform LField, RField, Hand, VLField, VRField, LThreeField, RThreeField, HLField, HRField;
    public GameObject Card, Message;

    public Button NewGame, RandomLeft, RandomRight;
    public Button PrevMovie, NexMovie, MakeMovie,
                  TestButton, ButtonThree, ButtonVert, ButtonHori;

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
        if (Instance == null) Instance = this;
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

        TestButton.onClick.AddListener(() => { Test(121); }); // Test // пока тут очистка убитых карточек
        ButtonThree.onClick.AddListener(() => { ChangeFormation(Formation.Three); });
        ButtonVert.onClick.AddListener(() => { ChangeFormation(Formation.Vertical); });
        ButtonHori.onClick.AddListener(() => { ChangeFormation(Formation.Horizontal); });

        GiveFieldCards(CurGame.HandCards, Hand);

        MakeNewGame();
    }

    void ChangeFormation(Formation newForm)
    {
        ClearFields();

        form = newForm;

        LField.gameObject.SetActive(false);
        RField.gameObject.SetActive(false);        

        switch (form) {
            case Formation.Horizontal:
                LField = HLField;
                RField = HRField;
                break;
            case Formation.Vertical:
                LField = VLField;
                RField = VRField;
                break;
            case Formation.Three:
                LField = LThreeField;
                RField = RThreeField;
                UpdateTreeFields();
                break;
            default:
                break;
        }

        LField.gameObject.SetActive(true);
        RField.gameObject.SetActive(true);
    }

    void ReturnNexMovie() // test
    {
        PrintMovieInfo("--- Возврат отмененного хода ---");

        PrevMovie.interactable = true;
        ReturnHistory(++CurGame.CurIndex);

        if (CurGame.NoNextHistory()) NexMovie.interactable = false;
    }

    void ReturnPrevMovie() // test
    {
        PrintMovieInfo("--- Отмена хода ---");

        NexMovie.interactable = true;

        if (CurGame.NoNextHistory()) CreateHistory();

        ReturnHistory(--CurGame.CurIndex);

        if (CurGame.NoPrevHistory()) PrevMovie.interactable = false;
    }

    public void UpdateTreeFields()
    {
        if (!FormThree) return; // на всякий случай проверка, вобще может лучше ошибку кидать

        for (int i = 0; i < LField.childCount - 1; i++)
        {
            if (LField.GetChild(i + 1).childCount == 0) LField.GetChild(i).gameObject.SetActive(false);
            else LField.GetChild(i).gameObject.SetActive(true);
        }

        for (int i = RField.childCount - 1; i > 0; i--)
        {
            if (RField.GetChild(i - 1).childCount == 0) RField.GetChild(i).gameObject.SetActive(false);
            else RField.GetChild(i).gameObject.SetActive(true);
        }

        Debug.Log("update fiels tree");
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

        Debug.Log(LCardsAdd.Count);
        Debug.Log(RCardsAdd.Count);

        if (!FormThree)
        {
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
        else { // 3-3 if here
            int i = 0;
            foreach (var cardInfo in RCardsAdd)
            {
                if (cardInfo == default) continue;

                CreateCardPref(cardInfo.Item1, RField.GetChild(i / 3), -1, (CardControler CC) =>
                {
                    CC.AddEffect(cardInfo.Item3);
                    CC.UpdateCurrentHealth(cardInfo.Item2);
                });

                i++;
            }

            for (int k = LCardsAdd.Count - 1; k >= 0; k--)
            {
                var cardInfo = LCardsAdd[k];

                if (cardInfo == default) continue;

                CreateCardPref(cardInfo.Item1, LField.GetChild(k / 3), -1, (CardControler CC) =>
                {
                    CC.AddEffect(cardInfo.Item3);
                    CC.UpdateCurrentHealth(cardInfo.Item2);
                });

            }
            UpdateTreeFields();
        }
    }

    void ReadThreeCardsToLists()
    {
        LCards = new List<(ICard, double, Effect)>();
        RCards = new List<(ICard, double, Effect)>();

        foreach (Transform subField in RField)
        {
            foreach (Transform card in subField)
            {
                CardControler CCard = card.GetComponent<CardControler>();
                RCards.Add((CCard.SelfCard, CCard.CurrentHealth, CCard.CurEffect));
            }
            if (subField.childCount < 3)
            {
                for (int i = subField.childCount; i < 3; i++)
                {
                    RCards.Add(default);
                }
            }
        }
        foreach (Transform subField in LField)
        {
            foreach (Transform card in subField)
            {
                CardControler CCard = card.GetComponent<CardControler>();
                LCards.Add((CCard.SelfCard, CCard.CurrentHealth, CCard.CurEffect));
            }
            if (subField.childCount < 3)
            {
                for (int j = subField.childCount; j < 3; j++)
                {
                    LCards.Add(default);
                }
            }
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
        if (FormThree) ReadThreeCardsToLists();
        else ReadCardsToLists();

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
        PrintMovieInfo("### НОВАЯ ИГРА ###");

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

        if (form == Formation.Three) UpdateTreeFields();
    }

    void GiveFieldCards(List<ICard> cards, Transform field)
    {
        //Debug.Log(cards[0].Name);

        Debug.Log(cards.Count);

        foreach (ICard card in cards)
        {
            GiveFieldOneCard(card, field, field == LField && form == Formation.Horizontal ? 0 : -1); // убрать передачу индекса, есть reverse (да?)

            Debug.Log(" - " + card.Name);
        }
    }

    public Transform GetFreeSubField(Transform field) // 3-3
    {
        if (!FormThree) return field;

        if (field == LField)
        {
            for (int i = field.childCount - 1; i >= 0; i--)
            {
                if (field.GetChild(i).childCount < 3) return field.GetChild(i);
            }
        }
        else // right
        {
            foreach (Transform subField in field)
            {
                if (subField.childCount < 3) return subField;
            }
        }

        //throw new Exception("Нет места бл.....");
        return field;
    }

    public void GiveFieldOneCard(ICard card, Transform field, int index = -1)
    {
        CreateCardPref(card, GetFreeSubField(field), index);
    }

    void CreateCardPref(ICard card, Transform field, int index = -1, Action<CardControler> MutCard = null)
    {
        GameObject cardGO = Instantiate(Card, (field), false);
        CardControler cardContr = cardGO.GetComponent<CardControler>();

        if (index != -1)
        {
            cardGO.transform.SetSiblingIndex(index);
        }

        cardContr.Init(card);

        getDropPlaceScript(field).countCards++;

        MutCard?.Invoke(cardContr);
    }


    public void ChangeMoney(Transform field, int changeCount)
    {
        if ((field) == RField) RMoneySum -= changeCount;
        else if ((field) == LField) LMoneySum -= changeCount;
        else throw new ArgumentException("ChangeMoney get `" + (field).name + "` field");

        UpdateGold();
    }

    public Transform SpecifyField(Transform field)
    {
        if (form == Formation.Three) return field.parent;
        return field;
    }

    void ClearFields()
    {
        if (!FormThree)
        {
            foreach (Transform field in new List<Transform> { RField, LField })
            {
                foreach (Transform card in field)
                {
                    Destroy(card.gameObject);
                }
                getDropPlaceScript(field).countCards = 0;
            }
        }
        else // for 3-3
        {
            foreach (Transform field in new List<Transform> { RField, LField })
            {
                foreach (Transform subField in field)
                {
                    foreach (Transform card in subField)
                    {
                        Destroy(card.gameObject);
                    }
                    getDropPlaceScript(subField).countCards = 0;
                }
            }
        }
        LCards = new List<(ICard, double, Effect)>();
        RCards = new List<(ICard, double, Effect)>();
    }

    void UpdateFields()
    {
        if (!FormThree)
        {
            foreach (Transform field in new List<Transform> { RField, LField })
            {
                foreach (Transform card in field)
                {
                    CardControler curCard = card.GetComponent<CardControler>();
                    if (!curCard.IsAlive()) Destroy(card.gameObject);
                }
            }
        }
        // Сделать красиво в один цикл
        else
        {
            for (int i = 0; i < RField.childCount; i++)
            {
                foreach (Transform card in RField.GetChild(i))
                {
                    CardControler curCard = card.GetComponent<CardControler>();
                    if (!curCard.IsAlive())
                    {
                        Destroy(card.gameObject);
                        getDropPlaceScript(RField.GetChild(i)).countCards--;
                        MoveTreeCards(RField.GetChild(i));
                    }
                }
            }

            for(int i = LField.childCount - 1; i >= 0; i--)
            {
                foreach (Transform card in LField.GetChild(i))
                {
                    CardControler curCard = card.GetComponent<CardControler>();
                    if (!curCard.IsAlive())
                    {
                        Destroy(card.gameObject);
                        getDropPlaceScript(LField.GetChild(i)).countCards--;
                        MoveTreeCards(LField.GetChild(i));
                    }
                }
            }

        }
        CheckVictory();
    }

    void MoveTreeCards(Transform subField) // in whitch destroy card
    {
        Transform neib = GetBackNeighbour(subField);

        if (getDropPlaceScript(subField).countCards < getDropPlaceScript(neib).countCards)
        {
            ThrowCardTo(neib, subField);
            MoveTreeCards(neib);
        }
        else return;
    }

    void ThrowCardTo(Transform from, Transform to)
    {
        from.GetChild(from.childCount - 1).SetParent(to);
        getDropPlaceScript(from).countCards--;
        getDropPlaceScript(to).countCards++;
    }


    public bool EnoughMoney(Transform field, int cost)
    {
        if (SpecifyField(field) == RField) return cost <= RMoneySum;
        else if (SpecifyField(field) == LField) return cost <= LMoneySum;
        else throw new ArgumentException("EnoughMoney get `" + SpecifyField(field).name + "` field");
    }

    void UpdateGold()
    {
        LMoney.text = LMoneySum.ToString();
        RMoney.text = RMoneySum.ToString();
    }


    void CloseAttack(bool turn)
    {
        List<(Transform, Transform)> closeCards = CloseAttackCards();

        CardControler TurnFirstCard, TurnSecondCard;

        foreach (var pair in closeCards) {
            TurnFirstCard = pair.Item1.GetComponent<CardControler>();
            TurnSecondCard = pair.Item2.GetComponent<CardControler>();

            TurnSecondCard.GetDamage(TurnFirstCard, AttackType.CLOSE); // механизм сколько отнимать жизни в CC
            TurnFirstCard.GetDamage(TurnSecondCard, AttackType.CLOSE); // ответная атака
        }

    }

    // возвращает список карт, которые могут атаковать, как пары <аттакующая, атакуемая>
    List<(Transform, Transform)> CloseAttackCards()
    {
        if (form == Formation.Horizontal)
        {
            (Transform, Transform) pair;

            if (Turn)
            {
                pair = ((RField).GetChild(0), (LField).GetChild(LField.childCount - 1));
                return new List<(Transform, Transform)> { pair };
            }
            pair = ((LField).GetChild(LField.childCount - 1), (RField).GetChild(0));
            return new List<(Transform, Transform)> { pair };
        }

        else if (form == Formation.Vertical)
        {
            List<(Transform, Transform)> result = new List<(Transform, Transform)>();

            for (int i = 0; i < (RField).childCount && i < (LField).childCount; i++)
            {
                if (Turn) result.Add(((RField).GetChild(i), (LField).GetChild(i)));
                else result.Add(((LField).GetChild(i), (RField).GetChild(i)));
            }
            return result;
        }

        else // 3-3
        {
            List<(Transform, Transform)> result = new List<(Transform, Transform)>();

            Transform CurLField = LField.GetChild(LField.childCount - 1), CurRField = RField.GetChild(0);


            for (int i = 0; i < CurLField.childCount && i < CurRField.childCount; i++)
            {
                result.Add((CurLField.GetChild(i), CurRField.GetChild(i)));
            }

            return result;
        }
    }

    void DistantAttack(CardControler attackCard, Transform enemyField, int index) // нужен ли?
    {
        if (FormThree)
             while (enemyField.childCount == 0) enemyField = GetNeighbour(enemyField);

        int atackedPositionIndex = UnityEngine.Random.Range(0, enemyField.childCount);
        CardControler enemyCard = enemyField.GetChild(atackedPositionIndex).GetComponent<CardControler>();
        enemyCard.GetDamage(attackCard, AttackType.DISTANT);
    }

    void SpecAttacks(bool turn)
    {
        List<Transform> fields = new List<Transform> { (turn ? RField : LField), (turn ? LField : RField) };

        for (int j = 0; j < 2; j++)
        {
            Transform curField = fields[j];

            if (FormThree)
            {
                for (int k = 0; k < curField.childCount; k++)
                {
                    for (int i = 0; i < curField.GetChild(k).childCount; i++)
                    {
                        CardControler CurCard = curField.GetChild(k).GetChild(i).GetComponent<CardControler>();

                        SpecAttackForCard(CurCard, curField.GetChild(k), i);
                    }
                }
            }
            else
            {

                for (int i = 0; i < curField.childCount; i++)
                {
                    CardControler CurCard = curField.GetChild(i).GetComponent<CardControler>();

                    SpecAttackForCard(CurCard, curField, i);
                }
            }

        }
    }

    void SpecAttackForCard(CardControler CurCard, Transform curField, int i)
    {
        switch (CurCard.WhichAbilityHas())
        {
            case HasAbilities.HEAL:
                foreach (Transform card in NearCards(curField, AnotherField(curField), i))
                {
                    CardControler cardCC = card.GetComponent<CardControler>();
                    if (cardCC.CanUseAbility(UseAbilities.HEAL)) cardCC.GetHeal();
                }
                break;

            case HasAbilities.APP:
                //Debug.Log(CurCard.name + " can app " + NearCards(curField, i).Count);
                foreach (Transform card in NearCards(curField, AnotherField(curField), i))
                {
                    CardControler cardCC = card.GetComponent<CardControler>();
                    if (cardCC.CanUseAbility(UseAbilities.APP)) cardCC.GetApp();
                }
                break;
            case HasAbilities.CLONE:
                foreach (Transform card in NearCards(curField, AnotherField(curField), i))
                {
                    CardControler cardCC = card.GetComponent<CardControler>();
                    if (cardCC.CanUseAbility(UseAbilities.CLONE) && cardCC.Clone())
                    {
                        if (FormThree) curField = GetFreeSubField(curField.parent);

                        CreateCardPref(cardCC.SelfCard, curField, card.GetSiblingIndex(), (CardControler CC) =>
                        {
                            CC.AddEffect(Effect.NO);
                            CC.UpdateCurrentHealth(cardCC.CurrentHealth);
                        }); // клонировать с тем же здоровьем
                    }
                }
                break;
            case HasAbilities.DISTANT:
                if (!FirstCards(i, curField, AnotherField(curField))) DistantAttack(CurCard, AnotherField(curField), i);
                break;

            default: break;
        }
    }

    List<Transform> NearCards(Transform turnField, Transform enemyField, int index)
    {
        Debug.Log("Near for card " + index + ", field = " + turnField.name);

        List<Transform> nearCards = new List<Transform>();

        if (FirstCards(index, turnField, enemyField)) return nearCards; // первые карты

        if (FormThree)
        {
            Transform neib = GetNeighbour(turnField);
            Transform backNeib = GetNeighbour(turnField);

            if (neib != turnField && index < neib.childCount)
                nearCards.Add(neib.GetChild(index));

            if (backNeib != turnField && index < backNeib.childCount)
                nearCards.Add(backNeib.GetChild(index));

        }

        if (index != 0)
        {
            nearCards.Add(turnField.GetChild(index - 1));
            //Debug.Log("0");
        }
        if (index != turnField.childCount - 1)
        {
            nearCards.Add(turnField.GetChild(index + 1));
            //Debug.Log("-1");
        }

        Debug.Log("NC - " + nearCards.Count);

        return nearCards;
    }

    // проверка что перед картой стоит карта противника
    // если в горизонтальном построении карта первая - очевидно так и есть
    bool FirstCards(int index, Transform turnField, Transform enemyField)
    {
        if (form == Formation.Horizontal)
        {
            if ((turnField == RField && index == 0) || (turnField == LField && index == LField.childCount - 1)) return true;
            return false;
        }
        else if (form == Formation.Vertical)
        {
            if (index < enemyField.childCount) return true;
            return false;
        }
        else // 3-3
        {
            if (turnField.GetSiblingIndex() == 0 && turnField.parent == RField && enemyField.childCount > index) return true;

            if(turnField.GetSiblingIndex() == turnField.childCount - 1 && turnField.parent == LField && enemyField.childCount > index) return true;

            return false;
        }

    }

    // 3-3 +
    Transform AnotherField(Transform field)
    {
        if(!FormThree) return field == RField ? LField : RField;
        else
        {
            Transform anotherParantField = field.parent == RField ? LField : RField;

            return anotherParantField.GetChild(anotherParantField.childCount - 1 - field.GetSiblingIndex());
        }
    }

    bool CheckVictory()
    {
        int countLeft, countRight;

        if(FormThree)
        {
            countLeft = LField.GetChild(LField.childCount - 1).childCount;
            countRight = RField.GetChild(0).childCount;
        }
        else
        {
            countLeft = LField.childCount;
            countRight = RField.childCount;
        }

        Debug.Log("Victory Left count = " + countLeft + "Victory Right count = " + countRight);

        if (countLeft == 0 || countRight == 0)
        {
            if (countLeft == 0 && countRight == 0) EndGame("Ничья");
            else if (countLeft == 0) EndGame("Right win");
            else if (countRight == 0) EndGame("Left win");
        }
        else if (IsDraw()) EndGame("Ничья (нет возможности для хода)");
        else return false;

        Debug.Log("check victory draw");


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

    // for 3-3 subFields, возврат поля что ближе к линии атаки от текущего
    public Transform GetNeighbour(Transform subField)
    {
        Transform field = subField.parent;

        int index = subField.GetSiblingIndex();

        if (getDropPlaceScript(subField).Type == FieldType.LField)
        {
            //Debug.Log("if");
            return index == field.childCount - 1 ? subField : field.GetChild(index + 1);
        }
        // RField
        else
        {
            return index == 0 ? subField : field.GetChild(index - 1);
        }
    }

    public Transform GetBackNeighbour(Transform subField)
    {
        Transform field = subField.parent;

        int index = subField.GetSiblingIndex();

        if (getDropPlaceScript(subField).Type == FieldType.LField)
        {
            return index == 0 ? subField : field.GetChild(index - 1);
        }
        // RField
        else
        {
            return index == field.childCount - 1 ? subField : field.GetChild(index + 1);
        }
    }

    DropPlaceScript getDropPlaceScript(Transform field)
    {
        return field.GetComponent<DropPlaceScript>();
    }

    public bool FormThree {
        get { return form == Formation.Three; }
    }

    // проверка ничьей, когда нет возможности для атаки для обеих сторон (перекати поле против перекати поля)
    bool IsDraw()
    {
        if(FormThree)
        {
            foreach (Transform subField in RField)
            {
                foreach(Transform card in subField)
                {
                    if (card.GetComponent<CardControler>().CanAttack && FirstCards(card.GetSiblingIndex(), subField, AnotherField(subField)) ||
                        card.GetComponent<CardControler>().HasDistant()) return false;
                }
            }
            foreach (Transform subField in LField)
            {
                foreach (Transform card in subField)
                {
                    if (card.GetComponent<CardControler>().CanAttack && FirstCards(card.GetSiblingIndex(), subField, AnotherField(subField)) ||
                        card.GetComponent<CardControler>().HasDistant()) return false;
                }
            }
        }
        else
        {
            foreach (Transform card in RField)
            {
                if (card.GetComponent<CardControler>().CanAttack && FirstCards(card.GetSiblingIndex(), RField, LField) ||
                    card.GetComponent<CardControler>().HasDistant()) return false;
            }
            foreach (Transform card in LField)
            {
                if (card.GetComponent<CardControler>().CanAttack && FirstCards(card.GetSiblingIndex(), LField, RField) ||
                    card.GetComponent<CardControler>().HasDistant()) return false;
            }
        }
        return true;
    }
}
