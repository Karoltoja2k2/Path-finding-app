using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Path_finding.Support
{
    public abstract class AlgorithmBase
    {
        public MainWindow mainWin;

        public Button[,] btnArray;
        public Queue<Button> btnGreen = new Queue<Button>();

        public Field pathTrack;
        public bool finishFound = false;



        public AlgorithmBase()
        {
            mainWin = (MainWindow)Application.Current.MainWindow;
            btnArray = mainWin.btnArray;
        }

        public abstract void Algorithm();

        public void Show_Visited_Field()
        {
            mainWin.vis = true;
            while (mainWin.vis)
            {
                if (btnGreen.Count != 0)
                    try
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() => btnGreen.Dequeue().Background = Brushes.Green));
                    }
                    catch(Exception) { continue; }

                    if (btnGreen.Count == 0 && mainWin.run == false)
                    {
                        mainWin.vis = false;
                        if (finishFound)
                            Application.Current.Dispatcher.Invoke(Show_Path);
                    }
                    Thread.Sleep(mainWin.visSpeed);
                    }
            Application.Current.Dispatcher.Invoke(() => mainWin.Stop_Vis());
        }

        public void Show_Path()
        {
            while (!pathTrack.start)
            {
                if (pathTrack.finish)
                {
                    pathTrack = pathTrack.prevField;
                    continue;
                }
                Button btn = btnArray[pathTrack.point.row, pathTrack.point.col];
                btn.Background = Brushes.Yellow;
                pathTrack = pathTrack.prevField;
            }
        }
    }
}
