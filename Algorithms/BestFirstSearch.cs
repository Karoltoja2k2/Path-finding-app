using Path_finding.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Path_finding.Algorithms
{
    public class BestFirstSearch : AlgorithmBase
    {
        List<Field> fieldList;

        public BestFirstSearch(List<Field> fieldList) : base()
        {
            this.fieldList = fieldList;
        }

        public override void Algorithm()
        {
            List<Field> openList = new List<Field>();
            openList.Add(fieldList.Find(x => x.start == true));
        
            var thr = new Thread(Show_Visited_Field);
            thr.Start();
        
            mainWin.run = true;
            while (mainWin.run)
            {
                if (openList.Count == 0)
                {
                    mainWin.run = false;
                    continue;
                }
        
                Field q = openList.OrderBy(p => p.hDistance).First();
                openList.Remove(q);
                q.visited = true;


                if (!q.start) btnGreen.Enqueue(btnArray[q.point.row, q.point.col]);

                foreach (Edge edge in q.edges)
                {
                    Field nextField = edge.nextField;
                    if (nextField.wall || nextField.visited) continue;
                    nextField.visited = true;
                    nextField.prevField = q;

                    if (nextField.finish)
                    {
                        pathTrack = nextField;
                        mainWin.run = false;
                        finishFound = true;
                        break;
                    }

                    openList.Add(nextField);
                }        
            }
        }
    }
}
