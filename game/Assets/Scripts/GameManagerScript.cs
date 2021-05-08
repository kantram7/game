using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System; 



public class Game
{
    public List<ICard> LeftFieldCards, RightFieldCards, HandCards;

    public bool BlockChanges = false;

    public Game()
    {
        LeftFieldCards = new List<ICard>();
        RightFieldCards = new List<ICard>();
        HandCards = new List<ICard>(CardManager.AllCards);
    }

    // Получить рандомный набор карт на переданную сумму
    public List<ICard> GiveRandomCards(int money)
    {
        Debug.Log("starts GiveRandomCards");

        List<ICard> cards = new List<ICard>();

        int minCost = CardManager.AllCards.Select(card => card.Cost).Min();

        while (money > minCost)
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
    const int GoldAmount = 101;
    public Game CurGame;
    public Transform LField, RField, Hand;
    public GameObject Card;

    public Button NewGame, RandomLeft, RandomRight;
    public Button PrevMovie, NexMovie, MakeMovie;

    public Text LMoney, RMoney;

    void Start()
    {
        CurGame = new Game();

        NewGame.onClick.AddListener(MakeNewGame);
        RandomLeft.onClick.AddListener(() => { RandomField(LField, LMoney); });
        RandomRight.onClick.AddListener(() => { RandomField(RField, RMoney); });
        MakeMovie.onClick.AddListener(Movie);

        GiveFieldCards(CurGame.HandCards, Hand);

        MakeNewGame();
    }

         
    void Movie()
    {
        CurGame.BlockChanges = true;

        RandomLeft.interactable = false;
        RandomRight.interactable = false;
        PrevMovie.interactable = true;
        NexMovie.interactable = true;

        // механизм хода
        // .........
    }

    void MakeNewGame()
    {
        Debug.Log("starts MakeNewGame");

        CurGame.BlockChanges = false;

        PrevMovie.interactable = false;
        NexMovie.interactable = false;
        RandomLeft.interactable = true;
        RandomRight.interactable = true;

        LMoney.text = RMoney.text = GoldAmount.ToString();

        ClearField(LField);
        ClearField(RField);
    }

    void RandomField(Transform field, Text money)
    {
        //Debug.Log("starts RandomField");

        int curMoney = int.Parse(money.text);

        //Debug.Log(curMoney);

        List<ICard> cards = CurGame.GiveRandomCards(curMoney);

        //Debug.Log("Cards count - " + cards.Count);

        GiveFieldCards(cards, field);

        ChangeMoney(field, cards.Select(card => card.Cost).Sum());
    }

    void GiveFieldCards(List<ICard> cards, Transform field)
    {
        Debug.Log("Cards to " + field.name);

        foreach (ICard card in cards)
        {
            GiveFieldOneCard(card, field, field == LField ? 0 : -1);
        }
    }

    public void GiveFieldOneCard(ICard card, Transform field, int index = -1)
    {
        Debug.Log("One card to " + field.name + " - (" + card.Name + ")");

        GameObject CardEx = Instantiate(this.Card, field, false);

        if (index != -1)
        {
            CardEx.transform.SetSiblingIndex(index);
        }

        CardEx.GetComponent<CardInfoScript>().ShowCardInfo(card);
    }


    public void ChangeMoney(Transform field, int changeCount)
    {
        Text money;
        if (field == RField) money = RMoney;
        else if (field == LField) money = LMoney;
        else throw new ArgumentException("ChangeMoney get `" + field.name + "` field");

        int curMoney = int.Parse(money.text);
        money.text = (curMoney - changeCount).ToString();
    }

    void ClearField(Transform field)
    {
        foreach (Transform card in field)
        {
            Destroy(card.gameObject);
        }
    }

    public bool EnoughMoney(Transform field, int cost)
    {
        Text money;
        if (field == RField) money = RMoney;
        else if (field == LField) money = LMoney;
        else throw new ArgumentException("EnoughMoney get `" + field.name + "` field");

        int curMoney = int.Parse(money.text);

        return (curMoney - cost) >= 0;
    }

}
