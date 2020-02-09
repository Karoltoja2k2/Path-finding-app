using Path_finding.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;
using System.Windows;

namespace Path_finding.Algorithms
{
    public class Dijkstra : AlgorithmBase
    {
        List<Field> fieldList;

        public Dijkstra(List<Field> fieldList) : base()
        {
            this.fieldList = fieldList;
        }

        public override void Algorithm()
        {
            var thr = new Thread(Show_Visited_Field);
            thr.Start();

            mainWin.run = true;
            while (mainWin.run)
            {
                Field threadedField = fieldList.OrderBy(p => p.distance).First();
                fieldList.Remove(threadedField);

                
                if (threadedField.distance == int.MaxValue)
                {
                    pathTrack = null;
                    mainWin.run = false;
                    break;
                }

                if (!threadedField.wall)
                {
                    foreach (Edge edge in threadedField.edges)
                    {
                        Field nextField = edge.nextField;

                        if (nextField.finish)
                        {
                            nextField.prevField = threadedField;
                            pathTrack = nextField;
                            finishFound = true;
                        }
                        else if (!nextField.wall && !nextField.visited)
                        {
                            if (edge.distance + threadedField.distance < nextField.distance)
                            {
                                nextField.distance = threadedField.distance + edge.distance;
                                nextField.visited = true;
                                btnGreen.Enqueue(btnArray[nextField.point.row, nextField.point.col]);
                                nextField.prevField = threadedField;
                            }
                        }                        
                    }
                    if (fieldList.Count == 0 || pathTrack != null)
                    {
                        mainWin.run = false;
                    }
                }
            }
        }



    }
}
