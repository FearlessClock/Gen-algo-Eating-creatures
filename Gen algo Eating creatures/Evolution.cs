using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
namespace Gen_algo_Eating_creatures
{
    class Evolution
    {
        static public int lastMaxTotal = 0;

        static public Creature[] Evole(Creature[] population)
        {
            int maxTotal = 0;
            foreach (Creature c in population)
            {
                if (c.totalFood > maxTotal)
                {
                    maxTotal = c.totalFood;
                }
            }
            lastMaxTotal = maxTotal;
            List<Creature> matingPool = new List<Creature>();
            foreach (Creature c in population)
            {
                for (int i = 0; i < c.totalFood / maxTotal * 10; i++)
                {
                    matingPool.Add(c);
                }
            }

            Random rand = new Random();
            Creature[] nextPop = new Creature[population.Length];
            for (int i = 0; i < population.Length; i++)
            {
                nextPop[i] = new Creature(new Vector2((float)Math.Cos(MathHelper.DegreesToRadians(i * 10)) * (30 + i) + 300, (float)Math.Sin(MathHelper.DegreesToRadians(i * 10)) * (30 + i) + 300),
                    CrossOver(population[rand.Next(0, population.Length)], population[rand.Next(0, population.Length)],
                    rand), OpenTK.Vector2.UnitX, (int)population[i].windowSize.X, (int)population[i].windowSize.Y, 3);
            }
            return nextPop;
        }

        static private string CrossOver(Creature a, Creature b, Random rand)
        {
            string dna = "";
            int val = rand.Next(0, 2);
            int middle = rand.Next(1, a.genome.Length-1);
            if (val == 0)
            {
                for (int i = 0; i < middle; i++)
                {
                    if (rand.NextDouble() < 0.1)
                        Mutate(rand);
                    dna += a.genome[i];
                }
                for (int i = middle; i < a.genome.Length; i++)
                {
                    if (rand.NextDouble() < 0.1)
                        Mutate(rand);
                    dna += b.genome[i];
                }
            }
            else
            {
                for (int i = 0; i < middle; i++)
                {
                    if (rand.NextDouble() < 0.1)
                        Mutate(rand);
                    dna += b.genome[i];
                }
                for (int i = middle; i < a.genome.Length; i++)
                {
                    if (rand.NextDouble() < 0.1)
                        Mutate(rand);
                    dna += a.genome[i];
                }
            }

            return dna;
        }

        static private char Mutate(Random rand)
        {
            switch (rand.Next(0, 4))
            {
                case 0:
                case 1:
                    return 'F';
                case 2:
                    return 'L';
                case 3:
                    return 'R';
                default:
                    return 'F';
            }

        }
    }
}
