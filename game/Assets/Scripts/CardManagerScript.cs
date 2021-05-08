using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface ICard
{
    int Id { get; }
    string Name { get; }
    int Defence { get; }
    int Attack { get; }
    int Health { get; }
    int Cost { get; }
    Sprite Logo { get; }

    List<Image> Abitilies { get; }
}

public class Archer : ICard
{
    const string _name = "Archer";
    int id = 0, _defence = 10, _attack = 50, _health = 20, _cost = 20;
    Sprite _img;
    List<Image> _abilities;

    public Archer(string path, List<string> abilitiesPaths)
    {
        this._img = Resources.Load<Sprite>(path);
        _abilities = new List<Image>();
        foreach(string pathImg in abilitiesPaths)
        {
            _abilities.Add(Resources.Load<Image>(pathImg));
        }
    }

    public string Name { get { return _name; } }
    public int Defence { get { return _defence; } }
    public int Attack { get { return _attack; } }
    public int Health { get { return _health; } }
    public int Cost { get { return _cost; } }
    public Sprite Logo { get { return _img; } }
    public int Id { get { return id; } }
    public List<Image> Abitilies { get { return _abilities; } }
}

public class Knight : ICard
{
    const string _name = "Knight";
    int id = 1, _defence = 50, _attack = 30, _health = 100, _cost = 30;
    Sprite _img;

    public Knight(string path)
    {
        this._img = Resources.Load<Sprite>(path);
    }

    public string Name { get { return _name; } }
    public int Defence { get { return _defence; } }
    public int Attack { get { return _attack; } }
    public int Health { get { return _health; } }
    public int Cost { get { return _cost; } }
    public Sprite Logo { get { return _img; } }
    public int Id { get { return id; } }
    public List<Image> Abitilies { get { return new List<Image>(); } }
}

public class Swordsman : ICard
{
    const string _name = "Swordsman";
    int id = 2, _defence = 20, _attack = 20, _health = 50, _cost = 15;
    Sprite _img;

    public Swordsman(string path)
    {
        this._img = Resources.Load<Sprite>(path);
    }

    public string Name { get { return _name; } }
    public int Defence { get { return _defence; } }
    public int Attack { get { return _attack; } }
    public int Health { get { return _health; } }
    public int Cost { get { return _cost; } }
    public Sprite Logo { get { return _img; } }
    public int Id { get { return id; } }
    public List<Image> Abitilies { get { return new List<Image>(); } }
}

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
        CardManager.AllCards.Add(new Archer("Sprites/test_img/archer", new List<string>{ "Sprites/test_img/icon", "Sprites/test_img/icon" }));
    }
}
