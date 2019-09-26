using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeSolver;

namespace MazeSolverGUI
{
    public static class MazeSolver
    {
        public static Stack<MazeCell> StackSteps { get; set; }
        public static MazeCell GetStartingPoint(char[,] mazeArray)
        {
            MazeCell startingPoint = new MazeCell();
            for (int y = 0; y < mazeArray.GetLength(0); y++)
            {
                for (int x = 0; x < mazeArray.GetLength(1); x++)
                {
                    if (mazeArray[y, x] == 'm')
                    {
                        startingPoint.Y = y;
                        startingPoint.X = x;
                    }
                }
            }

            return startingPoint;
        }

        public static bool MouseAndExitExists(char[,] mazeArray)
        {
            bool mouse = false;
            bool exit = false;

            for(int y = 0; y < mazeArray.GetLength(0); y++)
            {
                for(int x = 0; x < mazeArray.GetLength(1); x++)
                {
                    if(mazeArray[y,x] == 'e')
                    {
                        exit = true;
                    }

                    else if(mazeArray[y,x] == 'm')
                    {
                        mouse = true;
                    }
                }
            }

            if (mouse == true && exit == true)
            {
                return true;
            }

            else
                return false;
        }
    }
}
