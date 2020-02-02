using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Path_finding.Support
{
    public class Edge
    {
        public int distance { get; set; }
        public Field nextField { get; set; }

        public Edge(int distance, Field nextField)
        {
            this.distance = distance;
            this.nextField = nextField;
        }
    }
}
