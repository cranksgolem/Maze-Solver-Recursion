using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Windows.Threading;
using MazeSolver;

namespace MazeSolverGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public double GridCurrentWidth = 1;
        public double GridCurrentHeight = 1;

        public void GenerateGrid(int rows, int columns)
        {
            GridCurrentHeight = CnvMainCanvas.ActualHeight;
            GridCurrentWidth = CnvMainCanvas.ActualWidth;

            CnvMainCanvas.Children.Clear();

            double canvasWidth = CnvMainCanvas.ActualWidth;
            double canvasHeight = CnvMainCanvas.ActualHeight;

            double cellHeight = canvasHeight / rows;
            double cellWidth = canvasWidth / columns;

            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < columns; y++)
                {
                    var mazeCell = CreateMazeCell(cellWidth, cellHeight);
                    AddToGrid(mazeCell, cellHeight * x, cellWidth * y);
                }
            }
        }

        public System.Windows.Shapes.Rectangle CreateMazeCell(double width, double height)
        {
            System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle();
            rect.Stroke = new SolidColorBrush(Colors.White);
            rect.StrokeThickness = 2;
            rect.Fill = new SolidColorBrush(Colors.Black);
            rect.Width = width;
            rect.Height = height;
            return rect;
        }

        public void AdjustMazeCell(UIElement cell, double width, double height)
        {
            (cell as Rectangle).Width = width;
            (cell as Rectangle).Height = height;
        }

        public void AddToGrid(UIElement cell, double top, double left)
        {
            Canvas.SetTop(cell, top);
            Canvas.SetLeft(cell, left);
            CnvMainCanvas.Children.Add(cell);
        }

        public void TurnGreenToGray()
        {
            foreach (Rectangle cell in CnvMainCanvas.Children)
            {
                if (cell.Fill.ToString() == Brushes.Green.ToString())
                {
                    cell.Fill = new SolidColorBrush(Colors.LightGray);
                }
            }
        }

        public void AdjustCellPosition(UIElement cell, double top, double left)
        {
            Canvas.SetTop(cell, top);
            Canvas.SetLeft(cell, left);
        }

        public void AdjustCellInGrid()
        {
            double canvasWidth = CnvMainCanvas.ActualWidth;
            double canvasHeight = CnvMainCanvas.ActualHeight;

            double cellHeight = canvasHeight / Convert.ToInt32(SliderRows.Value);
            double cellWidth = canvasWidth / Convert.ToInt32(SliderColumns.Value);

            int cellNumber = 0;
            for (int x = 0; x < Convert.ToInt32(SliderRows.Value); x++)
            {
                for (int y = 0; y < Convert.ToInt32(SliderColumns.Value); y++)
                {
                    AdjustMazeCell(CnvMainCanvas.Children[cellNumber], cellWidth, cellHeight);
                    AdjustCellPosition(CnvMainCanvas.Children[cellNumber], cellHeight * x, cellWidth * y);
                    cellNumber++;
                }
            }
        }

        public char[,] TransformGridToArray(UIElementCollection grid)
        {
            var array = new char[Convert.ToInt32(SliderRows.Value), Convert.ToInt32(SliderColumns.Value)];

            int canvasCollectionIndex = 0;
            for (int y = 0; y < Convert.ToInt32(SliderRows.Value); y++)
            {
                for (int x = 0; x < Convert.ToInt32(SliderColumns.Value); x++)
                {
                    if(((CnvMainCanvas.Children[canvasCollectionIndex]) as Rectangle).Fill.ToString() == Brushes.Black.ToString())
                    {
                        array[y, x] = '1';
                    }

                    else if(((CnvMainCanvas.Children[canvasCollectionIndex]) as Rectangle).Fill.ToString() == Brushes.LightGray.ToString())
                    {
                        array[y, x] = '0';
                    }

                    else if (((CnvMainCanvas.Children[canvasCollectionIndex]) as Rectangle).Fill.ToString() == Brushes.Yellow.ToString())
                    {
                        array[y, x] = 'm';
                    }

                    else if (((CnvMainCanvas.Children[canvasCollectionIndex]) as Rectangle).Fill.ToString() == Brushes.Red.ToString())
                    {
                        array[y, x] = 'e';
                    }

                    canvasCollectionIndex++;
                }
            }

            return array;
        }

        public void TransformSolvedArrayToGrid(char[,] solvedArray)
        {
            int canvasCollectionIndex = 0;

            for (int y = 0; y < solvedArray.GetLength(0);y++)
            {
                for (int x = 0; x < solvedArray.GetLength(1); x++)
                {
                    
                    if (solvedArray[y, x] == '1')
                    {
                        ((CnvMainCanvas.Children[canvasCollectionIndex]) as Rectangle).Fill = new SolidColorBrush(Colors.Black);
                    }

                    else if (solvedArray[y, x] == '0')
                    {
                        ((CnvMainCanvas.Children[canvasCollectionIndex]) as Rectangle).Fill = new SolidColorBrush(Colors.LightGray);
                    }

                    else if (solvedArray[y, x] == 'm')
                    {
                        ((CnvMainCanvas.Children[canvasCollectionIndex]) as Rectangle).Fill = new SolidColorBrush(Colors.Yellow);
                    }

                    else if (solvedArray[y, x] == 'e')
                    {
                        ((CnvMainCanvas.Children[canvasCollectionIndex]) as Rectangle).Fill = new SolidColorBrush(Colors.Red);
                    }

                    else if (solvedArray[y, x] == '.')
                    {
                        ((CnvMainCanvas.Children[canvasCollectionIndex]) as Rectangle).Fill = new SolidColorBrush(Colors.Green);
                    }

                    canvasCollectionIndex++;
                }
            }
        }

        public void ShowStepByStep(char[,] mazeArray)
        {
            Stack<MazeCell> InOrderStackSteps = new Stack<MazeCell>();

            while(MazeSolver.StackSteps.Count != 1)
            {
                InOrderStackSteps.Push(MazeSolver.StackSteps.Pop());
            }

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += delegate
            {
                if (InOrderStackSteps.Count != 0)
                {
                    var nextStep = InOrderStackSteps.Pop();
                    mazeArray[nextStep.Y, nextStep.X] = '.';
                    TransformSolvedArrayToGrid(mazeArray);
                }

                if (InOrderStackSteps.Count == 0)
                {
                    timer.Stop();
                    MessageBox.Show("The exit has been found!", "Exit Found", MessageBoxButton.OK, MessageBoxImage.Information);
                    BtnReset.IsEnabled = true;
                }
            };
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Start();
        }

        private void SliderRows_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                GenerateGrid(Convert.ToInt32(SliderRows.Value), Convert.ToInt32(SliderColumns.Value));
            }
            catch (Exception)
            {

            }
        }

        private void SliderColumns_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                GenerateGrid(Convert.ToInt32(SliderRows.Value), Convert.ToInt32(SliderColumns.Value));
            }
            catch (Exception)
            {

            }
        }

        private void CnvMainCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (CnvMainCanvas.Children.Count != 0)
                AdjustCellInGrid();
        }

        private void RBWall_Checked(object sender, RoutedEventArgs e)
        {
            RBExit.IsChecked = false;
            RBMouse.IsChecked = false;
            RBPath.IsChecked = false;
        }

        private void RBPath_Checked(object sender, RoutedEventArgs e)
        {
            RBExit.IsChecked = false;
            RBMouse.IsChecked = false;
            RBWall.IsChecked = false;
        }

        private void RBMouse_Checked(object sender, RoutedEventArgs e)
        {
            RBExit.IsChecked = false;
            RBWall.IsChecked = false;
            RBPath.IsChecked = false;
        }

        private void RBExit_Checked(object sender, RoutedEventArgs e)
        {
            RBWall.IsChecked = false;
            RBMouse.IsChecked = false;
            RBPath.IsChecked = false;
        }

        private void CnvMainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (BtnSolve.Visibility == Visibility.Visible)
            {
                Rectangle elementName = new Rectangle();
                var mouseWasDownOn = e.Source as FrameworkElement;
                if (mouseWasDownOn != null)
                {
                    elementName = mouseWasDownOn as Rectangle;
                }

                if (RBWall.IsChecked == true)
                {
                    if (Convert.ToString(elementName.Fill) == Convert.ToString(Brushes.Black))
                    {
                        elementName.Fill = new SolidColorBrush(Colors.LightGray);
                    }

                    else
                        elementName.Fill = new SolidColorBrush(Colors.Black);
                }

                else if (RBPath.IsChecked == true)
                {
                    if (Convert.ToString(elementName.Fill) == Convert.ToString(Brushes.LightGray))
                    {
                        elementName.Fill = new SolidColorBrush(Colors.LightGray);
                    }

                    else
                        elementName.Fill = new SolidColorBrush(Colors.LightGray);
                }

                else if (RBMouse.IsChecked == true)
                {
                    Rectangle temp = new Rectangle();
                    foreach (Rectangle cell in CnvMainCanvas.Children)
                    {
                        if (cell.Fill.ToString() == Brushes.Yellow.ToString())
                        {
                            cell.Fill = new SolidColorBrush(Colors.LightGray);
                            temp = cell;
                        }
                    }

                    if (temp != elementName)
                        elementName.Fill = new SolidColorBrush(Colors.Yellow);
                }

                else if (RBExit.IsChecked == true)
                {
                    Rectangle temp = new Rectangle();
                    foreach (Rectangle cell in CnvMainCanvas.Children)
                    {
                        if (cell.Fill.ToString() == Brushes.Red.ToString())
                        {
                            cell.Fill = new SolidColorBrush(Colors.LightGray);
                            temp = cell;
                        }
                    }

                    if (temp != elementName)
                        elementName.Fill = new SolidColorBrush(Colors.Red);
                }
            }
        }

        private void BtnSolve_Click(object sender, RoutedEventArgs e)
        {
            char[,] mazeArray = TransformGridToArray(CnvMainCanvas.Children);
            MazeCell startingPoint = MazeSolver.GetStartingPoint(mazeArray);
            MazeSolver.StackSteps = new Stack<MazeCell>();
            bool mouseAndExit = MazeSolver.MouseAndExitExists(mazeArray);

            if (mouseAndExit == true && RecursiveSolveMaze(mazeArray, startingPoint) == true)
            {
                BtnSolve.Visibility = Visibility.Collapsed;
                BtnReset.Visibility = Visibility.Visible;
                BtnReset.IsEnabled = false;
                SliderColumns.IsEnabled = false;
                SliderRows.IsEnabled = false;

                ShowStepByStep(mazeArray);                
            }

            else
            {
                if (mouseAndExit == false)
                    MessageBox.Show("Either the mouse or exit is missing!", "Mouse / Exit Missing", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                else
                    MessageBox.Show("The exit can't be reached!", "Unreachable Exit", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            MazeSolver.StackSteps.Clear();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            TurnGreenToGray();
            BtnSolve.Visibility = Visibility.Visible;
            BtnReset.Visibility = Visibility.Collapsed;
            SliderRows.IsEnabled = true;
            SliderColumns.IsEnabled = true;
        }

        public bool RecursiveSolveMaze(char[,] currentArray, MazeCell currentMazeCell)
        {
            if (currentMazeCell.Y == -1 || currentMazeCell.X == -1 || currentMazeCell.Y == currentArray.GetLength(0) || currentMazeCell.X == currentArray.GetLength(1))
            {
                return false;
            }

            if (currentArray[currentMazeCell.Y, currentMazeCell.X] == 'e')
            {   
                return true;
            }

            if (currentArray[currentMazeCell.Y, currentMazeCell.X] == '1' || currentArray[currentMazeCell.Y, currentMazeCell.X] == 'n')
            {
                return false;
            }

            currentArray[currentMazeCell.Y, currentMazeCell.X] = 'n';
            MazeSolver.StackSteps.Push(new MazeCell(currentMazeCell.X, currentMazeCell.Y));
           
            MazeCell upCell = new MazeCell(currentMazeCell.X, currentMazeCell.Y - 1);
            MazeCell downCell = new MazeCell(currentMazeCell.X, currentMazeCell.Y + 1);
            MazeCell leftCell = new MazeCell(currentMazeCell.X - 1, currentMazeCell.Y);
            MazeCell rightCell = new MazeCell(currentMazeCell.X + 1, currentMazeCell.Y);

            if (RecursiveSolveMaze(currentArray, upCell) == true)
            {
                return true;
            }

            if (RecursiveSolveMaze(currentArray, downCell) == true)
            {
                return true;
            }

            if (RecursiveSolveMaze(currentArray, leftCell) == true)
            {
                return true;
            }

            if (RecursiveSolveMaze(currentArray, rightCell) == true)
            {
                return true;
            }

            currentArray[currentMazeCell.Y, currentMazeCell.X] = '0';
            MazeSolver.StackSteps.Pop();

            return false;
        }
    }
}

