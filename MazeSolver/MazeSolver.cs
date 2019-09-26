using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeSolver
{
    public class MazeSolver
    {
        private Queue<string> _mazeRows;
        private Stack<MazeCell> _mazeStack;
        private char[,] _maze;

        public char[,] Maze
        {
            get {return _maze;}
            private set {_maze = value;}
        }

        public Queue<string> MazeRows
        {
            get { return _mazeRows; }
            private set { _mazeRows = value; }
        }

        public Stack<MazeCell> MazeStack
        {
            get { return _mazeStack; }
            private set { _mazeStack = value; }
        }

        //Constructor
        public MazeSolver()
        {
            MazeRows = new Queue<string>();
            MazeStack = new Stack<MazeCell>();
            Maze = null;
        }

        public void InitializeMaze()
        {
            Console.Write("Enter the width(cells per row) of the maze: ");
            int mazeRowLength = Convert.ToInt32(Console.ReadLine());

            bool addRow = true;
            int numRowAdded = 1;

            //Determines length of protective walls
            string endWalls = "";
            for (int x = 0; x < mazeRowLength + 2; x++ )
            {
                endWalls += '1';
            }

            //Enqueues the upper protective wall
            MazeRows.Enqueue(endWalls);

            while (addRow == true)
            {
                string inputRow = "1";
                Console.Write("\nEnter layout for row " + numRowAdded + " (Passage = \"0\" Wall = \"1\" Exit = \"e\" Initial Position = \"m\"): ");
                inputRow += Console.ReadLine() + "1";

                if (inputRow.Length > mazeRowLength + 2)
                {
                    Console.WriteLine("\nRow entered is too long! Enter another Row.");
                    continue;
                }

                if (inputRow.Length < mazeRowLength + 2)
                {
                    Console.WriteLine("\nRow entered is too short! Enter another Row.");
                    continue;
                }

                if (CheckInputRow(inputRow) == false)
                {
                    Console.WriteLine("\nRow entered contains illegal characters! Enter another Row.");
                    continue;
                }

                //Enqueues the entered Row
                MazeRows.Enqueue(inputRow);
                numRowAdded++;

                //Asks if the user would like to add another row
                string response = "";
                while (response != "Y" && response != "N")
                {
                    Console.Write("\nWould you like to add another Row? ('Y' / 'N')");
                    response = Console.ReadLine().ToUpper();
                }

                if (response == "N")
                {
                    addRow = false;
                }
            }

            MazeRows.Enqueue(endWalls);
        }

        public void CreateMaze()
        {
            if (MazeRows.Count != 0)
            {
                Maze = new char[MazeRows.Count, MazeRows.Peek().Length];
                int length = MazeRows.Count;

                for (int x = 0; x < length; x++)
                {
                    for (int y = 0; y < MazeRows.Peek().Length; y++)
                    {
                        Maze[x, y] = MazeRows.Peek()[y];
                    }

                    MazeRows.Dequeue();
                }
            }
        }

        public void DisplayMaze()
        {
            if (Maze != null)
            {
                for (int x = 0; x < Maze.GetLength(0); x++)
                {
                    for (int y = 0; y < Maze.GetLength(1); y++)
                    {
                        Console.Write(Maze[x, y] + " ");
                    }
                    Console.WriteLine();
                }
            }
        }

        public string SolveMaze()
        {
            MazeStack = new Stack<MazeCell>();
            MazeCell exitCell = new MazeCell();
            MazeCell entryCell = new MazeCell();
            MazeCell currentCell = new MazeCell();

            for (int y = 0; y < Maze.GetLength(0); y++)
            {
                for (int x = 0; x < Maze.GetLength(1); x++)
                {
                    if (Maze[y,x] == 'm')
                    {
                        entryCell.Y = y;
                        entryCell.X = x;

                        currentCell.Y = y;
                        currentCell.X = x;
                    }

                    else if (Maze[y,x] == 'e')
                    {
                        exitCell.Y = y;
                        exitCell.X = x;
                    }
                }
            }

            while(exitCell.X != currentCell.X || exitCell.Y != currentCell.Y)
            {
                if(Maze[currentCell.Y, currentCell.X] == '0')
                {
                    Maze[currentCell.Y, currentCell.X] = '.';
                }

                if (Maze[currentCell.Y - 1, currentCell.X] == '0' || Maze[currentCell.Y - 1, currentCell.X] == 'e')
                {
                    MazeStack.Push(new MazeCell(currentCell.X, currentCell.Y - 1));
                }

                if (Maze[currentCell.Y + 1, currentCell.X] == '0' || Maze[currentCell.Y + 1, currentCell.X] == 'e')
                {
                    MazeStack.Push(new MazeCell(currentCell.X, currentCell.Y + 1));
                }

                if (Maze[currentCell.Y, currentCell.X - 1] == '0' || Maze[currentCell.Y, currentCell.X - 1] == 'e')
                {
                    MazeStack.Push(new MazeCell(currentCell.X - 1, currentCell.Y));
                }

                if (Maze[currentCell.Y, currentCell.X + 1] == '0' || Maze[currentCell.Y, currentCell.X + 1] == 'e')
                {
                    MazeStack.Push(new MazeCell(currentCell.X + 1, currentCell.Y));
                }

                if (MazeStack.Count == 0)
                {
                    return "The exit can't be reached!";
                }

                else
                {
                    currentCell = MazeStack.Pop();
                }
            }

            return "The exit has been reached!";
        }

        //Checks for illegal characters in inputRow
        public bool CheckInputRow(string inputRow)
        {
            for (int x = 0; x < inputRow.Length; x++)
            {
                if (inputRow[x] != '0' && inputRow[x] != '1' && inputRow[x] != 'e' && inputRow[x] != 'm')
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class MazeCell
    {
        public int X { get; set; }
        public int Y { get; set; }

        public MazeCell()
        {
            X = -1;
            Y = -1;
        }

        public MazeCell(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
