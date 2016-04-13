using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Gen_algo_Eating_creatures
{
    class Creature
    {
        public Vector2 position;
        public int totalFood;
        public string genome;
        Vector2 direction;
        int step;
        int lastCommand = 0;
        

        Dictionary<char, Func<bool>> commands = new Dictionary<char, Func<bool>>();

        public Creature(Vector2 pos, string dna, Vector2 dir, int total = 0, int s = 10)
        {
            position = pos;
            genome = dna;
            direction = dir;
            step = s;
            totalFood = total;

            commands.Add('F', MoveForward);
            commands.Add('L', TurnLeft);
            commands.Add('R', TurnRight);
        }

        public void Update(List<Food> food)
        {
            foreach(Food f in food)
            {
                if(DistanceBetween(f.position) < 10)
                {
                    totalFood++;
                    f.isAlive = false;
                }
            }
            char command = genome[lastCommand];
            CommandController(command);
            lastCommand++;
            if (lastCommand > genome.Length - 1)
                lastCommand = 0;
        }

        private bool CommandController(char comm)
        {
            Func<bool> func; 
            if(commands.TryGetValue(comm, out func))
            {
                func();
                return true;
            }
            return false;
        }

        private bool MoveForward()
        {
            position += step * direction;
            return true;
        }
        private bool TurnLeft()
        {
            direction = Rotate(direction, -MathHelper.PiOver2);
            return true;
        }
        private bool TurnRight()
        {
            direction = Rotate(direction, MathHelper.PiOver2);
            return true;
        }
        private Vector2 Rotate(Vector2 a, double angle)
        {
            double[][] rotation = new double[2][];
            rotation[0] = new double[2] { Math.Cos(angle), -Math.Sin(angle) };
            rotation[1] = new double[2] { Math.Sin(angle), Math.Cos(angle) };
            return new Vector2((float)(rotation[0][0] * a.X + rotation[0][1] * a.Y), (float)(rotation[1][0] * a.X + rotation[1][1] * a.Y)); 
        }

        private double DistanceBetween(Vector3 a)
        {
            return Math.Sqrt(Math.Pow(a.X - position.X, 2) + Math.Pow(a.Y - position.Y, 2));
        }

        private float GetDirectionAngle(Vector2 a)
        {
            return (float)Math.Atan(a.Y / a.X);
        }
        
        public DrawStruct Draw()
        {
            Vector3 translation = new Vector3(position.X, position.Y, 0);
            Vector3 scale = new Vector3(totalFood+3, totalFood+3, 0);
            float dir = GetDirectionAngle(direction);
            return new DrawStruct(translation, scale, dir);  
        }
    }
}
