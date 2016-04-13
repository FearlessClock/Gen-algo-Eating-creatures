using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Gen_algo_Eating_creatures
{
    class Food
    {
        public int worth;
        public bool isAlive;
        public Vector3 position;

        public Food(int w, Vector3 pos, bool alive = true)
        {
            worth = w;
            position = pos;
            isAlive = alive;
        }

        public bool IsAlive()
        {
            return isAlive;
        }
    }
}
