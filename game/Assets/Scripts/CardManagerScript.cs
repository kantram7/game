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
        CardManager.AllCards.Add(new Swordsman("Sprites/UnitsImg/cat_swordsman"));
        CardManager.AllCards.Add(new Knight("Sprites/UnitsImg/cat_knight"));
        CardManager.AllCards.Add(new Archer("Sprites/UnitsImg/cat_archer"));
        CardManager.AllCards.Add(new Healer("Sprites/UnitsImg/cat_healer"));
        CardManager.AllCards.Add(new Warlock("Sprites/UnitsImg/cat_warlock"));

        CardManager.AllCards.Add(TumbleweedUnitCreator.getTumbleweed("Sprites/UnitsImg/cat_tumbleweed"));
    }
}
