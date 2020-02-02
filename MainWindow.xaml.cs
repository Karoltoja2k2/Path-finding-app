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
        public Dijkstra alg;
        public int visSpeed = 5;
        public bool vis;

        public Support.Point startPoint = new Support.Point(25,20);
        public Support.Point finishPoint = new Support.Point(25, 80);

        public static Button[,] btnArray;

        public int rows;
        public int columns;

        public bool mouseDown = false;
        public bool movingStart = false; 
        public bool movingFinish = false;
        public Field movedField;

        public MainWindow()
        {
            InitializeComponent();
            rows = 30;
            columns = 60;
            Create_Grid(rows, columns);
        }

        public void Create_Grid(int rows, int cols)
        {
            fieldArray = new Field[rows, cols];
            btnArray = new Button[rows, cols];

            grid.Children.Clear();
            for (int i = 0; i < rows; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;
                grid.RowDefinitions.Add(row);


                for (int j = 0; j < cols; j++)
                {
                    ColumnDefinition col = new ColumnDefinition();
                    col.Width = GridLength.Auto;
                    grid.ColumnDefinitions.Add(col);

                    Support.Point pnt = new Support.Point(i, j);

                    Button btn = new Button();
                    btn.CommandParameter = pnt;
                    btn.Height = 15;
                    btn.Width = 15;
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
                field.Add_Edges(rows, columns, fieldArray);
                field.distance = int.MaxValue;                  

            }
            Set_Start(0, 0);
            Set_Finish(rows - 1, cols - 1);
        }



        public void Set_Start(int row, int col)
        {
            fieldArray[row, col].start = true;
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
            finishPoint = new Support.Point(row, col);
            fieldArray[row, col].distance = int.MaxValue;

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
            if (mouseDown)
            {
                Button clicked = (Button)sender;
                Support.Point pnt = (Support.Point)clicked.CommandParameter;

                Field field = fieldArray[pnt.row, pnt.col];
                Button btn = btnArray[pnt.row, pnt.col];
                if (field.distance != 0 && !field.finish)
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

        public void Clear()
        {
            foreach (Field field in fieldArray)
            {
                field.visited = false;
                field.distance = field.point.Is_Equal_To(startPoint) ? 0 : int.MaxValue;
                if (!field.start && !field.finish)
                    btnArray[field.point.row, field.point.col].Background = field.wall ? Brushes.Black : Brushes.White;

            }
        }

        public void Dijkstra_Alg(object sender, RoutedEventArgs e)
        {
            Clear();
            List<Field> fieldList = new List<Field>();
            foreach (Field field in fieldArray)
            {               
                if (!field.wall)
                    fieldList.Add(field);
            }

            var asd = new List<Field>(fieldList);

            alg = new Dijkstra(asd, btnArray);
            Thread thr2 = new Thread(alg.Move);
            thr2.Start();

            vis = true;
            Thread thr = new Thread(Visualisation_Proccess);
            thr.Start();


        }

        public void Show_Path(Field pathTrack)
        {
            Console.WriteLine(alg.pathTrack);
            Button btn = btnArray[pathTrack.point.row, pathTrack.point.col];
            if (pathTrack.start)
            { 
                btn.Background = Brushes.DodgerBlue;
                return; 
            }
            else if (pathTrack.finish)
                btn.Background = Brushes.Coral;
            else
                btn.Background = Brushes.Yellow;
                       
            if (pathTrack.prevField != null)
                Show_Path(pathTrack.prevField);
        }

        public void Visualisation_Proccess()
        {
            while (vis)
            {
                if (alg.btnGreen.Count > 0)
                {
                    Dispatcher.Invoke(Show_Visited_Field);
                }
                else if (alg.run == false && alg.btnGreen.Count == 0)
                {
                    Thread.Sleep(100);
                    vis = false;
                    if (alg.pathTrack != null)
                    {
                        Dispatcher.Invoke(new Action<Field>((pT) => Show_Path(pT)), alg.pathTrack);
                    }
                }
                Thread.Sleep(visSpeed);
            }
        }

        public void Show_Visited_Field()
        {
            try
            {
                Button btn = alg.btnGreen.Dequeue();
                btn.Background = Brushes.Green;
            }
            catch(InvalidOperationException)
            {
                return;
            }
        }
    }
}
