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

                            if (pieceInStock != null && pieceInStock.quantite>= piece.quantite)
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



        public void VerifyCommand(Dictionary<string, int> commandes)
        {
            if (commandes == null || commandes.Count == 0)
            {
                PrintError("Commande vide ou incorrecte.");
                return;
            }

            foreach (var c in commandes)
            {
                var robot = depot.robots.FirstOrDefault(r => r.name == c.Key);

                if (robot == null)
                {
                    PrintError($"'{c.Key}' is not a recognized robot");
                    return;
                }

                var requiredPieces = robot.GetRequiredPiecesForQuantity(c.Value);
                foreach (var piece in requiredPieces)
                {
                    var pieceInStock = depot.pieces.FirstOrDefault(p => p.name == piece.Key);
                    if (pieceInStock == null)
                    {
                        PrintError($"Pièce inconnue : {piece.Key}");
                        return;
                    }
                    if (pieceInStock.quantite < piece.Value)
                    {
                        Console.WriteLine("UNAVAILABLE");
                        return;
                    }
                }
            }

            Console.WriteLine("AVAILABLE");
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
