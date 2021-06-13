using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumbleweed
{
    public class TumbleweedClass
    {
        public string getName {
            get { return name; }
            set { name = value; }
        }

        public int getDefence
        {
            get { return 500; }
        }

        public int getAttack
        {
            get { return 0; }
        }

        public int getHealth
        {
            get { return health; }
            set { health = value; }
        }


        string name = "Tumbleweed";
        int health = 250;
    }
}
