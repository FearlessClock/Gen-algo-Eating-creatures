using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gen_algo_Eating_creatures
{
    class Evolution
    {
        Creature[] creatures;

        public Evolution(Creature[] creat)
        {
            creatures = creat;
        }

        static public Creature[] Evole(Creature[] population)
        {
            int maxTotal = 0;
            foreach(Creature c in population)
            {
                if(c.totalFood > maxTotal)
                {
                    maxTotal = c.totalFood;
                }
            }
            List<Creature> matingPool = new List<Creature>();
            foreach(Creature c in population)
            {
               // for(int i = 0; i < 
            }
            return population;
        }
    }
}
