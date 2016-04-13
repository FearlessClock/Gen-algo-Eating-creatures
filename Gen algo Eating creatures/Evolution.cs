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
                for(int i = 0; i < c.totalFood/maxTotal*10; i++)
                {
                    matingPool.Add(c);
                }
            }

            Random rand = new Random();
            Creature[] nextPop = new Creature[population.Length];
            for (int i = 0; i < population.Length; i++)
            {
                nextPop[i] = new Creature(population[i].position, 
                    CrossOver(population[rand.Next(0, population.Length)], population[rand.Next(0, population.Length)],
                    rand), OpenTK.Vector2.UnitX, 3);
            }
            return nextPop;
        }

    static private string CrossOver(Creature a, Creature b, Random rand)
    {
        string dna = "";
        int val = 0;
        for(int i = 0; i < a.genome.Length; i++)
        {
            val = rand.Next(0, 2);
            if(val == 0)
            {
                dna += a.genome[i];
            }
            else
            {
                dna += b.genome[i];
            }
        }
        return dna;
    }
    }
}
