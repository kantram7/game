using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace Assets.Scripts.Units
{
    public interface ICard
    {
        int Id { get; }
        string Name { get; }
        int Defence { get; }
        int Attack { get; }
        int Health { get; }
        int Cost { get; }
    }

    public interface ICardLogo
    {
        Sprite Logo { get; }
    }

    public enum HasAbilities
    {
        NO,
        CLONE,
        HEAL,
        APP,
        DISTANT
    }

    public enum UseAbilities
    {
        NO,
        CLONE,
        HEAL,
        APP,
    }

    public abstract class AbilityClass
    {
        public HasAbilities HasAbility;

        List<UseAbilities> AvalAbility;

        public bool CanUseAbility(UseAbilities abi)
        {
            return AvalAbility.Contains(abi);
        }

        protected AbilityClass(HasAbilities hasAbi, List<UseAbilities> list)
        {
            HasAbility = hasAbi;
            AvalAbility = new List<UseAbilities>(list);
        }
    }


    public class Archer : AbilityClass, ICard, ICardLogo
    {
        const string _name = "Archer";
        int id = 0, _defence = 10, _attack = 25, _health = 20, _cost = 20;
        Sprite _img;

        public Archer(string path) : base(HasAbilities.DISTANT, new List<UseAbilities>{ UseAbilities.CLONE, UseAbilities.HEAL })
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

    }

    public class Knight : AbilityClass, ICard, ICardLogo
    {
        const string _name = "Knight";
        int id = 1, _defence = 50, _attack = 30, _health = 100, _cost = 30;
        Sprite _img;

        public Knight(string path) : base(HasAbilities.APP, new List<UseAbilities> { UseAbilities.HEAL, UseAbilities.APP })
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
    }

    public class Swordsman : AbilityClass, ICard, ICardLogo
    {
        const string _name = "Swordsman";
        int id = 2, _defence = 20, _attack = 20, _health = 50, _cost = 15;
        Sprite _img;

        public Swordsman(string path) : base(HasAbilities.APP, new List<UseAbilities> { UseAbilities.CLONE, UseAbilities.HEAL })
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
    }

    public class Healer : AbilityClass, ICard, ICardLogo
    {
        const string _name = "Healer";
        int id = 3, _defence = 5, _attack = 5, _health = 30, _cost = 35;
        Sprite _img;

        public Healer(string path) : base(HasAbilities.HEAL, new List<UseAbilities> { UseAbilities.HEAL })
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
    }

    public class Warlock : AbilityClass, ICard, ICardLogo
    {
        const string _name = "Warlock";
        int id = 4, _defence = 15, _attack = 15, _health = 20, _cost = 45;
        Sprite _img;

        public Warlock(string path) : base(HasAbilities.CLONE, new List<UseAbilities>())
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
    }
}
