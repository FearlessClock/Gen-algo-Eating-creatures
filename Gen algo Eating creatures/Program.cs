using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Gen_algo_Eating_creatures
{
    class Program
    {
        static void Main(string[] args)
        {
            GameWindow window = new GameWindow(1366, 768, OpenTK.Graphics.GraphicsMode.Default, "Evolve", GameWindowFlags.Fullscreen);
            Game game = new Game(window);

            window.Run();
        }
    }
}
