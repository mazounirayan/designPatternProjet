using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace designPatternProject
{
    internal class Factory
    {
        public Stock depot;

        public Factory(Stock depot)
        {
            this.depot = depot;
        }



        public void totalStock(Dictionary<string, int> commandes)
        {
            var totalPieces = new Dictionary<string, int>();

            foreach (var c in commandes)
            {
                foreach (var robot in depot.robots)
                {
                    if (robot.name == c.Key)
                    {
                        var requiredPieces = robot.GetRequiredPiecesForQuantity(c.Value);
                        foreach (var piece in requiredPieces)
                        {
                            string pieceName = piece.Key;
                            int pieceQty = piece.Value;

                            if (totalPieces.ContainsKey(pieceName))
                            {
                                totalPieces[pieceName] += pieceQty;
                            }
                            else
                            {
                                totalPieces[pieceName] = pieceQty;
                            }
                        }
                    }
                }

            }
            foreach (var piece in totalPieces)
            {
                Console.WriteLine($"SommeDesQuantités1 : {piece.Value}  , Pièce : {piece.Key}");
            }

        }

        public void displayStock()
        {

            depot.AfficherStock();
        }

        public void displayNeededStock(Dictionary<string, int> commandes)
        {
            foreach (var robot in commandes)
            {
                depot.pieceParRobot(robot.Key, robot.Value);
            }
            Console.WriteLine("Total :\n");

            totalStock(commandes);
        }


        public void ProcessInstructionCommand(Dictionary<string, int> robotOrders)
        {
            foreach (var order in robotOrders)
            {
                string robotType = order.Key;
                int quantity = order.Value;


                for (int i = 0; i < quantity; i++)
                {

                    var robot = depot.robots.FirstOrDefault(r => r.name == robotType);
                    if (robot != null)
                    {

                        Console.WriteLine($"PRODUCING {robotType}");
                        List<string> stockPieces = new List<string>();

                        foreach (var piece in robot.requiredPieces)
                        {
                            var pieceInStock = depot.pieces.FirstOrDefault(r => r.name == piece.name);

                            if (pieceInStock != null && pieceInStock.quantite >= piece.quantite)
                            {
                                Console.WriteLine($"GET_OUT_STOCK {piece.quantite} {piece.name}");
                                pieceInStock.quantite -= piece.quantite;
                                stockPieces.Add(piece.name);
                                if (NeedsSystem(piece.name))
                                {
                                    Console.WriteLine($"INSTALL System_SB1 {piece.name}");

                                }
                            }
                            else
                            {
                                Console.WriteLine($"Erreur: Stock insuffisant pour {piece.name}");
                                break;
                            }


                        }

                        if (stockPieces.Count == 4)
                        {
                            string core = stockPieces[0];
                            string generator = stockPieces[1];
                            string arms = stockPieces[2];
                            string legs = stockPieces[3];

                            Console.WriteLine($"ASSEMBLE TMP1 {core} {generator}");

                            Console.WriteLine($"ASSEMBLE TMP2 TMP1 {arms}");

                            Console.WriteLine($"ASSEMBLE TMP3 TMP2 {legs}");
                        }



                        Console.WriteLine($"FINISHED {robotType}");
                    }
                    else
                    {
                        Console.WriteLine($"Erreur: Type de robot inconnu '{robotType}'");
                    }
                }
            }
        }

        private bool NeedsSystem(string pieceName)
        {
            return pieceName.StartsWith("Core_");
        }
        public string VerifyCommand(Dictionary<string, int> commandes)
        {
            if (commandes is null || commandes.Count == 0)
                return "ERROR Commande vide ou incorrecte.";

            foreach (var c in commandes)
            {
                var robot = depot.robots
                                 .FirstOrDefault(r => r.name.Equals(c.Key,
                                                     StringComparison.OrdinalIgnoreCase));

                if (robot is null)
                    return $"ERROR '{c.Key}' is not a recognized robot";

                // Interdit : un robot de catégorie G
                if (robot.cat == Category.G)
                    return $"ERROR Robot '{robot.name}' cannot be of category G";

                // Vérifie la compatibilité Règle 4.1
                if (!robot.IsBuildable())
                    return $"ERROR Incompatible pieces for robot '{robot.name}'";

                // Vérifie qu’on a assez de stock
                foreach (var kv in robot.GetRequiredPiecesForQuantity(c.Value))
                {
                    var pieceInStock = depot.pieces.FirstOrDefault(p => p.name == kv.Key);
                    if (pieceInStock is null)
                        return $"ERROR Pièce inconnue : {kv.Key}";
                    if (pieceInStock.quantite < kv.Value)
                        return "UNAVAILABLE";
                }
            }

            return "AVAILABLE";
        }


        public void ProduceCommand(Dictionary<string, int> commandes)
        {
            var result = VerifyCommand(commandes);
            if (result != "AVAILABLE")
            {
                Console.WriteLine(result);
                return;
            }

            foreach (var order in commandes)
            {
                var robot = depot.robots.First(r =>
                    r.name.Equals(order.Key, StringComparison.OrdinalIgnoreCase));
                robot.quantite += order.Value;

                foreach (var kv in robot.GetRequiredPiecesForQuantity(order.Value))
                {
                    var piece = depot.pieces.First(p =>
                        p.name.Equals(kv.Key, StringComparison.OrdinalIgnoreCase));
                    piece.quantite -= kv.Value;
                }
            }

            Console.WriteLine("STOCK_UPDATED");
        }

        private void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("ERROR ");
            Console.ResetColor();
            Console.WriteLine(message);
            Thread.Sleep(1000);
        }

    }




}
