using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Assets.Scripts.Units;

public static class CardManager
{
    public static List<ICard> AllCards = new List<ICard>();

    public static ICard GetCard(int id) // Need here?
    {
        return AllCards.Find(card => card.Id == id);
    }
}

public class CardManagerScript : MonoBehaviour
{
    public void Awake()
    {
        CardManager.AllCards.Add(new Swordsman("Sprites/test_img/swordsman"));
        CardManager.AllCards.Add(new Knight("Sprites/test_img/khight"));
        CardManager.AllCards.Add(new Archer("Sprites/test_img/archer"));
        CardManager.AllCards.Add(new Healer("Sprites/test_img/healer"));
        CardManager.AllCards.Add(new Warlock("Sprites/test_img/warlock"));
    }
}
