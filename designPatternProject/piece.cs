using System.Xml.Linq;

namespace designPatternProject
{
    public class Piece
    {
        public string name { get; set; }
        public int quantite { get; set; }
        public Piece(string name, int quantite)
        {
            this.name = name;
            this.quantite = quantite;
        }
    }
}