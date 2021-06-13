using System;

namespace Tumbleweed
{
    public class TumbleweedClass
    {
        public string getName {
            get { return "Tumbleweed"; }
            set { name = value; }
        }

        public int getDefence {
            get { return 500; }
            set { defence = value; }
        }

        public int getAttack {
            get { return 0; }
            set { attack = value; }
        }

        public int getHealth {
            get { return 250; }
            set { health = value; }
        }

        string name;
        int defence, health, attack;

    }
}
