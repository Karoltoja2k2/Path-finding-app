using Path_finding.Support;
using System.Windows.Controls;

namespace Path_finding
{
    public class Field
    {
        public Point point { get; set; }

        public bool visited { get; set; }
        public bool finish { get; set; }
        public bool start { get; set; }
        public bool wall { get; set; }

        public int distance { get; set; }
        public Field prevField { get; set; }

        public Button btn;

        public Field(int row, int col)
        {
            this.point = new Point(row, col);
            this.distance = 0;
        }
    }
}
