using System;
using System.Collections.Generic;
using System.Linq;

namespace designPatternProject
{
    public class Robot
    {
        public string name { get; set; }
        public int quantite { get; set; }
        public Category cat { get; set; }
        public List<Piece> requiredPieces { get; set; }

        public Robot(string name, int quantite, Category cat, List<Piece> requiredPieces)
        {
            if (cat == Category.G)
                throw new ArgumentException("Un robot ne peut pas être de catégorie Généraliste (G).", nameof(cat));

            this.name = name;
            this.quantite = quantite;
            this.cat = cat;
            this.requiredPieces = requiredPieces ?? new List<Piece>();
        }

        public Dictionary<string, int> GetRequiredPiecesForQuantity(int quantity)
        {
            var result = new Dictionary<string, int>();
            foreach (var piece in requiredPieces)
                result[piece.name] = piece.quantite * quantity;
            return result;
        }

        public bool IsBuildable()
        {
            bool categoriesOk = requiredPieces.All(p =>
                Compatibility.IsCompatible(cat,
                                           p.cat,
                                           p.isSystem || p.isModule));
            if (!categoriesOk) return false;

            int moduleCount = requiredPieces.Count(p => p.isModule);
            int maxAllowed = cat switch
            {
                Category.D => 1,
                Category.I => 3,
                Category.M => 2,
                _ => 0
            };

            return moduleCount <= maxAllowed;
        }



        public void Affichepieces(int quantite)
        {
            foreach (var piece in requiredPieces)
                Console.WriteLine($"{piece.quantite * quantite}  Pièce : {piece.name} ({piece.cat}),");
        }

        public void Affichepieces()
        {
            Console.WriteLine($" Quantité : {quantite} , Robot : {name} ({cat})");
            foreach (var piece in requiredPieces)
                Console.WriteLine($"  Quantité {piece.quantite}  Pièce : {piece.name} ({piece.cat}),");
        }
    }
}
