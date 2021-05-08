using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Units
{
    interface IUnit
    {
        Int32 Id { get; }
        String Name { get; }
        Int32 HitPoints { get; }
        Int32 Attack { get; }
        Int32 Defence { get; }
        Int32 Cost { get; }
        Boolean LongRange { get; }
    }

    interface IAbility
    {
        Boolean HasAbility { get; }
        Boolean Cloned { get; }
        Boolean Healed { get; }
        Boolean Apped { get; }
        //IUnit SAction(IUnit unit);
    }

    class Archer : IUnit, IAbility
    {
        public Archer(Int32 id, Int32 hitPoints, Int32 attack, Int32 defence, Int32 cost, String name = "Archer")
        {
            Id = id;
            HitPoints = hitPoints;
            Attack = attack;
            Defence = defence;
            Cost = cost;
            Name = name;
        }
        public Int32 Id { get; }
        public Int32 HitPoints { get; }
        public Int32 Attack { get; }
        public Int32 Defence { get; }
        public Int32 Cost { get; }
        public String Name { get; }
        public Boolean Cloned { get { return true; } }
        public Boolean Healed { get { return true; } }
        public Boolean Apped { get { return false; } }
        public Boolean LongRange { get { return true; } }
        public Boolean HasAbility { get { return false; } }
    }
}
