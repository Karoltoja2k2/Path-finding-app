using Path_finding.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Path_finding.Algorithms
{
    public class Dijkstra
    {
        public Field[,] fieldArray { get; set; }
        public Field startField;
        public bool finishFound = false;

        public Point[] offset = new Point[] {
                                                  new Point(-1, 0),
                                new Point(0, -1),                 new Point(0, 1),
                                                  new Point(1, 0)
                                };

        public Dijkstra(Field[,] fieldArray, Field startField)
        {
            this.fieldArray = fieldArray;
            this.startField = startField;

            First_Move();
        }

        public void First_Move()
        {
            Point startPoint = startField.point;

            startField.visited = true;

            foreach (Point offPoint in offset)
            {
                Point tempPoint = startPoint.Add_Point(offPoint);
                if (tempPoint.Inside_Boundries(16, 30))
                    Move(tempPoint, startField);
            }
        }

        public void Move(Point moveTo, Field prevField)
        {
            if (!finishFound)
            {
                Field movedTo = fieldArray[moveTo.row, moveTo.col];
                if (movedTo.visited)
                {
                    return;
                }
                if (movedTo.finish)
                {
                    finishFound = true;
                }

                movedTo.visited = true;
                movedTo.distance += prevField.distance + 1;
                movedTo.prevField = prevField;
                movedTo.btn.Background = Brushes.Green;

                foreach (Point offPoint in offset)
                {
                    Point tempPoint = movedTo.point.Add_Point(offPoint);
                    if (tempPoint.Inside_Boundries(16, 30))
                        Move(tempPoint, movedTo);
                }
            }
        }
    }
}
