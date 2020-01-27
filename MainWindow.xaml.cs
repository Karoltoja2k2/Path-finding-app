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

namespace Path_finding
{


    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Field[,] fieldArray;
        public Field startField;
        public Field finishField;

        public bool mouseDown = false;
        public bool movingStart = false; 
        public bool movingFinish = false;
        public Field movedField;

        public MainWindow()
        {
            InitializeComponent();

            Create_Grid(16, 30);
        }

        public void Create_Grid(int rows, int cols)
        {
            fieldArray = new Field[rows, cols];
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
                    btn.Height = 40;
                    btn.Width = 40;
                    btn.Background = Brushes.White;

                    fieldArray[i, j] = new Field(i, j);
                    fieldArray[i, j].btn = btn;
                    fieldArray[i, j].btn.PreviewMouseLeftButtonDown += LMB_Down;
                    fieldArray[i, j].btn.PreviewMouseLeftButtonUp += LMB_Up;
                    fieldArray[i, j].btn.MouseEnter += Put_Wall;



                    Grid.SetColumn(btn, j);
                    Grid.SetRow(btn, i);
                    grid.Children.Add(btn);
                }
            }


            Set_Start(fieldArray[8, 5]);
            Set_Finish(fieldArray[8, 25]);

        }

        // start == true for start and start == false for finish
        public void Set_Start(Field fieldToSet)
        {
            startField = fieldToSet;

            startField.start = true;
            startField.btn.MouseEnter -= Put_Wall;
            startField.btn.PreviewMouseLeftButtonDown -= LMB_Down;
            startField.btn.PreviewMouseLeftButtonDown += LMB_Down_Move;

            startField.btn.Background = Brushes.DodgerBlue;

        }

        public void Set_Finish(Field fieldToSet)
        {
            finishField = fieldToSet;

            finishField.finish = true;
            finishField.btn.MouseEnter -= Put_Wall;
            finishField.btn.PreviewMouseLeftButtonDown -= LMB_Down;
            finishField.btn.PreviewMouseLeftButtonDown += LMB_Down_Move;

            finishField.btn.Background = Brushes.Coral;
        }
        
        public void Set_Empty(Field fieldToSet)
        {
            fieldToSet.start = false;
            fieldToSet.btn.MouseEnter += Put_Wall;
            fieldToSet.btn.PreviewMouseLeftButtonDown += LMB_Down_Move;
            fieldToSet.btn.PreviewMouseLeftButtonDown += LMB_Down;

            fieldToSet.btn.Background = Brushes.White;

        }


        public void LMB_Down_Move(object sender, MouseButtonEventArgs e)
        {
            Button clicked = (Button)sender;
            Support.Point pnt = (Support.Point)clicked.CommandParameter;

            if (pnt.Is_Equal_To(startField.point))
            {
                movingStart = true;
            }
            else if (pnt.Is_Equal_To(finishField.point))
            {
                movingFinish = true;
            }

            e.Handled = true;
        }

        public void Move_Point(Support.Point pnt)
        {

            if (movingStart)
            {
                Set_Empty(startField);
                Set_Start(fieldArray[pnt.row, pnt.col]);
            }
            else if (movingFinish)
            {
                Set_Empty(finishField);
                Set_Finish(fieldArray[pnt.row, pnt.col]);
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
                if (!field.start && !field.finish)
                {
                    if (field.wall == false)
                    {
                        field.btn.Background = Brushes.Black;
                        field.wall = true;
                    }
                    else
                    {
                        field.btn.Background = Brushes.White;
                        field.wall = false;
                    }
                }
            }
        }

        public void Dijkstra_Alg(object sender, RoutedEventArgs e)
        {
            Dijkstra alg = new Dijkstra(fieldArray, startField);
        }
    }
}
