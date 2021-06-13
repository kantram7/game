using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using Tumbleweed;


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

    public abstract class ICardClass : AbilityClass, ICardLogo, ICard
    {
        readonly string _name;
        int _id, _defence, _attack, _health, _cost;
        Sprite _img;

        public ICardClass(string path, string name, int id, int defence, int attack, int health, int cost, HasAbilities hasaba, List<UseAbilities> abis) : 
            base(hasaba, abis)
        {
            _name = name;
            _id = id;
            _defence = defence;
            _attack = attack;
            _health = health;
            _cost = cost;

            _img = Resources.Load<Sprite>(path);
        }

        public string Name { get { return _name; } }
        public int Defence { get { return _defence; } }
        public int Attack { get { return _attack; } }
        public int Health { get { return _health; } }
        public int Cost { get { return _cost; } }
        public Sprite Logo { get { return _img; } }
        public int Id { get { return _id; } }
    }



    public class Archer : ICardClass
    {
        public Archer(string path) : base(path, "Archer", 0, 10, 25, 20, 20, HasAbilities.DISTANT, new List<UseAbilities> { UseAbilities.CLONE, UseAbilities.HEAL }) { }
    }

    public class Knight : ICardClass
    {
        public Knight(string path) : base(path, "Knight", 1, 50, 30, 100, 30, HasAbilities.NO, new List<UseAbilities> { UseAbilities.HEAL, UseAbilities.APP }) { }
    }

    public class Swordsman : ICardClass
    {
        public Swordsman(string path) : base(path, "Swordsman", 2, 20, 20, 50, 15, HasAbilities.APP, new List<UseAbilities> { UseAbilities.HEAL, UseAbilities.CLONE }) { }
    }

    public class Healer : ICardClass
    {
        public Healer(string path) : base(path, "Healer", 3, 5, 5, 30, 35, HasAbilities.HEAL, new List<UseAbilities> { UseAbilities.HEAL }) { }
    }

    public class Warlock : ICardClass
    {
        public Warlock(string path) : base(path, "Warlock", 4, 15, 15, 20, 45, HasAbilities.CLONE, new List<UseAbilities> { }) { }
    }


    public class TumbleweedUnit : ICardClass
    {
        public TumbleweedUnit(string path, TumbleweedClass tumbleweed) : base(path, tumbleweed.getName, 5, tumbleweed.getDefence,
            tumbleweed.getAttack, tumbleweed.getHealth, 70, HasAbilities.NO, new List<UseAbilities> { }) {

            _tumbleweed = tumbleweed;
        }

        TumbleweedClass _tumbleweed = null;

    }

    static public class TumbleweedUnitCreator
    {
        static public TumbleweedUnit getTumbleweed(string path)
        {
            return new TumbleweedUnit(path, new TumbleweedClass());
        }
    }
}
