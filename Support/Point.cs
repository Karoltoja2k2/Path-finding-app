using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Path_finding.Support
{
    public class Point
    {
        public int row;
        public int col;

        public Point(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        public Point Add_Point(Point p2)
        {
            return new Point(this.row - p2.row, this.col - p2.col);
        }

        public bool Inside_Boundries(int rowsParam, int colsParams)
        {
            if (row >= 0 && col >= 0 && row < rowsParam && col < colsParams)
                return true;
            else
                return false;
        }

        public bool Is_Equal_To(Point point)
        {
            if (row == point.row && col == point.col)
                return true;
            else
                return false;
        }

        public double Distance_To(Point p2)
        {
            int dy = Math.Abs(p2.row - row);
            int dx = Math.Abs(p2.col - col);

            double f = Math.Sqrt(Math.Pow(dy, 2) + Math.Pow(dx, 2));
            return f;
        }
    }
}


