namespace designPatternProject
{
    public class Robot
    {
        public string Name { get; set; }
        public int Quantite { get; set; }
        public List<Piece> Pieces { get; set; }
        public Robot(string name, int quantite)
        {
            Name = name;
            Quantite = quantite;
            Pieces = new List<Piece>();
        }
       
    }
}