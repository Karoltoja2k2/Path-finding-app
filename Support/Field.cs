using Path_finding.Support;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace Path_finding
{
    public class Field
    {
        public Point point { get; set; }

        public bool visited { get; set; }
        public bool finish { get; set; }
        public bool start { get; set; }
        public bool wall { get; set; }

        public List<Edge> edges;
        public int distance { get; set; }
        public Field prevField { get; set; }

        public Button btn;

        public Point[] offset = new Point[] {
                                                  new Point(-1, 0),
                                new Point(0, -1),                 new Point(0, 1),
                                                  new Point(1, 0)
                                };

        public Field()
        {

        }

        public Field(int row, int col)
        {
            this.point = new Point(row, col);
            this.distance = 0;
        }

        public void Reset()
        {
            distance = start ? 0 : int.MaxValue;
            prevField = null;
            visited = false;
        }

        public void Add_Edges(int rows, int cols, Field[,] fieldArray)
        {
            this.edges = new List<Edge>();
            foreach (Point offpoint in offset)
            {
                Point tempPoint = this.point.Add_Point(offpoint);
                if (tempPoint.Inside_Boundries(rows, cols))
                {
                    edges.Add(new Edge(1, fieldArray[tempPoint.row, tempPoint.col]));
                }
            }
        }
    }
}
