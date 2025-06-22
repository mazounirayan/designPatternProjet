namespace designPatternProject
{
    public class Piece
    {
        public string   name      { get; set; }
        public int      quantite  { get; set; }
        public Category cat       { get; set; }
        public bool     isSystem  { get; set; }

        public Piece(string name, int quantite, Category cat, bool isSystem = false)
        {
            this.name     = name;
            this.quantite = quantite;
            this.cat      = cat;
            this.isSystem = isSystem;
        }
    }
}
