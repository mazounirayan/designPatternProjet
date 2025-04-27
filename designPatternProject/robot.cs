namespace designPatternProject
{
    public class Robot
    {
        public string name { get; set; }
        public int quantite { get; set; }
        public List<Piece> requiredPieces { get; set; }

        public Robot(string name, int quantite, List<Piece> requiredPieces)
        {
            this.name = name;
            this.quantite = quantite;
            this.requiredPieces = requiredPieces;
        }
      
        public Dictionary<string, int> GetRequiredPiecesForQuantity(int quantity)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();

            foreach (var piece in requiredPieces)
            {
                result[piece.name] = piece.quantite * quantity;
            }

            return result;
        }

        public void Affichepieces(int quantite)
        {
       
            foreach (var piece in requiredPieces)
            {
                Console.WriteLine($"{piece.quantite * quantite}  Pièce : {piece.name},");
            }
        }
        public void Affichepieces()
        {
            Console.WriteLine($" Quantité : {quantite} , Robot : {name}");
            foreach (var piece in requiredPieces)
            {
                Console.WriteLine($"  Quantité   {piece.quantite}  Pièce : {piece.name},");
            }
        }

    }
}