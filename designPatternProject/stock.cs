using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace designPatternProject
{
    internal class Stock
    {
        public List<Robot> Robots { get; set; }
        public List<Piece> Pieces { get; set; }

      

        public Stock(List<Piece> pieces, List<Robot> robots)
        {
            Pieces = pieces;
            Robots = robots;
        }
    }
}
