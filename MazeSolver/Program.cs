using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            MazeSolver maze = new MazeSolver();
            maze.InitializeMaze();      
            maze.CreateMaze();
            maze.DisplayMaze();
            Console.WriteLine();
            Console.WriteLine(maze.SolveMaze());
            Console.WriteLine();
            maze.DisplayMaze();
            Console.ReadLine();
        }
    }
}
