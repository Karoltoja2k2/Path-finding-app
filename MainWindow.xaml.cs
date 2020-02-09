using Path_finding.Algorithms;
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
using Path_finding.Support;
using System.Threading;

namespace Path_finding
{


    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Field[,] fieldArray;
        public Field[,] fieldArrayToPass;
        public int visSpeed = 5;

        public bool run;

        public Thread thr;

        public bool vis;

        public Support.Point startPoint = new Support.Point(25,20);
        public Support.Point finishPoint = new Support.Point(25, 80);

        public Button[,] btnArray;

        public int rows;
        public int columns;
        public int size;
        public int chosenAlg = 1;
        public Random random = new Random();

        public bool mouseDown = false;
        public bool movingStart = false; 
        public bool movingFinish = false;
        public Field movedField;

        public MainWindow()
        {
            InitializeComponent();
            rows = 10;
            columns = 20;
            Loaded += Create_Grid;
        }

        #region Grid and board change methods

        public void Create_Grid(object sender = null, RoutedEventArgs e = null)
        {
            fieldArray = new Field[rows, columns];
            btnArray = new Button[rows, columns];
            var style = (Style)App.Current.TryFindResource("fieldButton");


            grid.Children.Clear();
            for (int i = 0; i < rows; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;
                grid.RowDefinitions.Add(row);


                for (int j = 0; j < columns; j++)
                {
                    ColumnDefinition col = new ColumnDefinition();
                    col.Width = GridLength.Auto;
                    grid.ColumnDefinitions.Add(col);

                    Support.Point pnt = new Support.Point(i, j);

                    Button btn = new Button();
                    btn.CommandParameter = pnt;
                    btn.Height = 1260 / columns;
                    btn.Width = 1260 / columns;
                    btn.Style = style;
                    btn.Background = Brushes.White;
                    btn.PreviewMouseLeftButtonDown += LMB_Down;
                    btn.PreviewMouseLeftButtonUp += LMB_Up;
                    btn.MouseEnter += Put_Wall;
                    btnArray[i, j] = btn;

                    Grid.SetColumn(btnArray[i, j], j);
                    Grid.SetRow(btnArray[i, j], i);
                    grid.Children.Add(btnArray[i, j]);


                    fieldArray[i, j] = new Field(i, j);
                }
            }

            foreach (Field field in fieldArray)
            {
                field.Add_Edges(rows, this.columns, fieldArray);
                field.distance = int.MaxValue;                  

            }
            Set_Start(0, 0);
            Set_Finish(rows - 1, columns - 1);
        }



        public void Set_Start(int row, int col)
        {
            fieldArray[row, col].start = true;
            fieldArray[row, col].wall = false;
            fieldArray[row, col].distance = 0;
            startPoint = new Support.Point(row, col);

            Button threadedBtn = btnArray[row, col];
            threadedBtn.MouseEnter -= Put_Wall;
            threadedBtn.PreviewMouseLeftButtonDown -= LMB_Down;
            threadedBtn.PreviewMouseLeftButtonDown += LMB_Down_Move;
            threadedBtn.Background = Brushes.DodgerBlue;
        }

        public void Set_Finish(int row, int col)
        {
            fieldArray[row, col].finish = true;
            fieldArray[row, col].wall = false;
            fieldArray[row, col].distance = int.MaxValue;
            finishPoint = new Support.Point(row, col);

            Button threadedBtn = btnArray[row, col];
            threadedBtn.MouseEnter -= Put_Wall;
            threadedBtn.PreviewMouseLeftButtonDown -= LMB_Down;
            threadedBtn.PreviewMouseLeftButtonDown += LMB_Down_Move;
            threadedBtn.Background = Brushes.Coral;
        }
        
        public void Set_Empty(int row, int col)
        {
            fieldArray[row, col].distance = int.MaxValue;
            fieldArray[row, col].start = fieldArray[row, col].finish = false;

            Button threadedBtn = btnArray[row, col];
            threadedBtn.MouseEnter += Put_Wall;
            threadedBtn.PreviewMouseLeftButtonDown += LMB_Down_Move;
            threadedBtn.PreviewMouseLeftButtonDown += LMB_Down;
            threadedBtn.Background = fieldArray[row, col].wall ? Brushes.Black : Brushes.White;
        }


        public void LMB_Down_Move(object sender, MouseButtonEventArgs e)
        {
            Button clicked = (Button)sender;
            Support.Point pnt = (Support.Point)clicked.CommandParameter;

            if (pnt.Is_Equal_To(startPoint))
            {
                movingStart = true;
            }
            else if (pnt.Is_Equal_To(finishPoint))
            {
                movingFinish = true;
            }

            e.Handled = true;
        }

        public void Move_Point(Support.Point pnt)
        {
            if (!vis)
            {
                if (movingStart)
                {
                    Set_Empty(startPoint.row, startPoint.col);
                    Set_Start(pnt.row, pnt.col);
                }
                else if (movingFinish)
                {
                    Set_Empty(finishPoint.row, finishPoint.col);
                    Set_Finish(pnt.row, pnt.col);
                }
            }
            movingStart = movingFinish = false;
        }

        public void LMB_Down(object sender, MouseButtonEventArgs e)
        {
            mouseDown = true;
            Put_Wall(sender);
            e.Handled = true;
        }

        public void LMB_Up(object sender, MouseButtonEventArgs e)
        {
            mouseDown = false;

            if (movingStart || movingFinish)
            {
                Button clicked = (Button)sender;
                Move_Point((Support.Point)clicked.CommandParameter);
            }
            e.Handled = true;
        }

        public void Put_Wall(object sender, MouseEventArgs e = null)
        {
            if (mouseDown && !vis)
            {
                Button clicked = (Button)sender;
                Support.Point pnt = (Support.Point)clicked.CommandParameter;

                Field field = fieldArray[pnt.row, pnt.col];
                Button btn = btnArray[pnt.row, pnt.col];
                if (!field.start && !field.finish)
                {
                    if (field.wall == false)
                    {
                        btn.Background = Brushes.Black;
                        field.wall = true;
                    }
                    else
                    {
                        btn.Background = Brushes.White;
                        field.wall = false;
                    }
                }
            }
        }

        #endregion

        // Algorithms

        public void Best_First_Search_Alg()
        {
            Clear();

            List<Field> fieldList = new List<Field>();
            foreach (Field field in fieldArray)
            {
                field.prevField = null;
                field.visited = false;
                field.distance = int.MaxValue;
                field.fDistance = 0;

                if (field.finish) continue;

                if (!field.wall)
                    field.hDistance = field.point.Distance_To(finishPoint);
                    fieldList.Add(field);

                if (!field.wall && !field.start && !field.finish)
                {
                    btnArray[field.point.row, field.point.col].Background = Brushes.White;
                }
            }

            var fieldToVisit = new List<Field>(fieldList);

            BestFirstSearch alg = new BestFirstSearch(fieldToVisit);
            thr = new Thread(() => alg.Algorithm());
            thr.Start();


        }

        public void Dijkstra_Alg()
        {
            Clear();
            List<Field> fieldList = new List<Field>();
            foreach (Field field in fieldArray)
            {
                field.prevField = null;
                field.visited = false;

                if (!field.wall)
                    field.distance = field.start ? 0 : int.MaxValue;
                    fieldList.Add(field);

                    
                if (!field.wall && !field.start && !field.finish)
                {
                    btnArray[field.point.row, field.point.col].Background = Brushes.White;
                }
            }

            var fieldToVisit = new List<Field>(fieldList);

            Dijkstra alg = new Dijkstra(fieldToVisit);
            thr = new Thread(() => alg.Algorithm());
            thr.Start();

        }

        // Viusalisation methods

        private void Start_Vis(object sender, RoutedEventArgs e)
        {
            Buttons_Change_State(true);
            switch (chosenAlg)
            {
                case 1:
                    Dijkstra_Alg();
                    break;
                case 2:
                    Best_First_Search_Alg();
                    break;
            }
        }

        public void Stop_Vis(object sender = null, RoutedEventArgs e = null)
        {
            vis = false;
            run = false;
            Buttons_Change_State(false);
        }

        public void Clear()
        {
            if (thr != null)
                if (thr.IsAlive)
                    thr.Abort();
        }

        private void changeLevel(object sender, RoutedEventArgs e)
        {
            if (grid != null)
            {
                Button btn = (Button)sender;
                int newSize;
                int.TryParse((string)btn.CommandParameter, out newSize);
                if (newSize != size)
                {
                    size = newSize;
                    switch (newSize)
                    {
                        case 1:
                            rows = 10;
                            columns = 20;
                            break;
                        case 2:
                            rows = 25;
                            columns = 50;
                            break;
                        case 3:
                            rows = 50;
                            columns = 100;
                            break;
                    }
                    Create_Grid();
                }
            }
        }

        private void Random_Maze(object sender, RoutedEventArgs e)
        {
            var rand = new Random();
            foreach(Field field in fieldArray)
            {
                if (field.start == false && field.finish == false)
                {
                    if (rand.Next(0, 100) > 70)
                    {
                        field.wall = true;
                        btnArray[field.point.row, field.point.col].Background = Brushes.Black;
                    }
                    else
                    {
                        field.wall = false;
                        btnArray[field.point.row, field.point.col].Background = Brushes.White;
                    }    
                }
            }
        }

        public void Change_Alg(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int newAlg;
            int.TryParse((string)btn.CommandParameter, out newAlg);
            chosenAlg = newAlg != chosenAlg ? newAlg : chosenAlg;
        }
        // ASDASD

        public void Buttons_Change_State(bool algOn)
        {
            if (algOn)
            {
                startBtn.Content = "Stop";
                startBtn.Background = new SolidColorBrush(Color.FromRgb(185, 15, 15));
                startBtn.Click -= Start_Vis;
                startBtn.Click += Stop_Vis;
            }
            else
            {
                startBtn.Content = "Visualise";
                startBtn.Background = new SolidColorBrush(Color.FromRgb(72, 201, 176));
                startBtn.Click += Start_Vis;
                startBtn.Click -= Stop_Vis;
            }

            dijkstraButton.IsEnabled = !algOn;
            BFSbutton.IsEnabled =      !algOn;
            randomMazeBtn.IsEnabled =  !algOn;
            size1Btn.IsEnabled =       !algOn;
            size2Btn.IsEnabled =       !algOn;
            size3Btn.IsEnabled =       !algOn;
        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.visSpeed = (int)speedSlider.Value;
        }

        private void grid_MouseLeave(object sender, MouseEventArgs e)
        {
            mouseDown = movingFinish = movingStart = false;
        }

        private void mouseOutExpander(object sender, MouseEventArgs e)
        {
            Expander exp = (Expander)sender;
            exp.IsExpanded = false;
        }

        private void mouseInExpander(object sender, MouseEventArgs e)
        {
            Expander exp = (Expander)sender;
            exp.IsExpanded = true;
        }

        private void Close_Game_Window(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimize_Game_Window(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Drag_Window(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
