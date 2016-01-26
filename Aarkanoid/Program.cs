using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEngine;

namespace Aarkanoid
{
    class Program
    {
        static void Main(string[] args)
        {
            var engine = new Engine("Aarkanoid", 1280, 768, 60);
            engine.debugCollisions = false;

            engine.SpawnObject("gameManager", GameManager.I);

            engine.Run();
        }
    }
}
