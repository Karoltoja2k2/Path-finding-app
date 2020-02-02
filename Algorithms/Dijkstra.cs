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

namespace Path_finding.Algorithms
{
    public class Dijkstra
    {
        List<Field> fieldList;
        Button[,] btnArray;
        public Field startField;
        public bool finishFound = false;

        public Field pathTrack;
        public bool run;

        public Queue<Button> btnGreen = new Queue<Button>();

        public Point[] offset = new Point[] {
                                                  new Point(-1, 0),
                                new Point(0, -1),                 new Point(0, 1),
                                                  new Point(1, 0)
                                };

        public Dijkstra(List<Field> fieldList, Button[,] btnArray)
        {
            this.fieldList = fieldList;
            this.btnArray = btnArray;
        }

        public void Move()
        {
            run = true;
            do
            {
                Field threadedField = fieldList.OrderBy(p => p.distance).First();
                fieldList.Remove(threadedField);

                if (threadedField.distance == int.MaxValue) break;

                if (!threadedField.wall)
                {
                    foreach (Edge edge in threadedField.edges)
                    {
                        Field nextField = edge.nextField;

                        if (!nextField.wall && !nextField.visited)
                        {
                            if (edge.distance + threadedField.distance < nextField.distance)
                            {
                                nextField.distance = threadedField.distance + edge.distance;
                                nextField.visited = true;
                                btnGreen.Enqueue(btnArray[nextField.point.row, nextField.point.col]);
                                nextField.prevField = threadedField;
                            }
                            if (nextField.finish)
                            {
                                pathTrack = nextField;
                                break;
                            }
                        }                        
                    }
                    if (fieldList.Count == 0 || pathTrack != null)
                    {
                        run = false;
                    }
                }
            } while (run);

        }

    }
}
