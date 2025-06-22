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

                if (robot.cat == Category.G)
                    return $"ERROR Robot '{robot.name}' cannot be of category G";

                if (!robot.IsBuildable())
                    return $"ERROR Incompatible pieces for robot '{robot.name}'";

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

        public string AddTemplate(string templateName, List<string> pieceNames)
        {
            if (depot.robots.Any(r => r.name.Equals(templateName,
                                StringComparison.OrdinalIgnoreCase)))
                return $"ERROR Template '{templateName}' already exists";

            var piecesForTemplate = new List<Piece>();

            foreach (var pieceName in pieceNames)
            {
                var pieceInCatalog = depot.pieces
                    .FirstOrDefault(p => p.name.Equals(pieceName,
                                       StringComparison.OrdinalIgnoreCase));

                if (pieceInCatalog == null)
                    return $"ERROR Unknown piece : {pieceName}";

                piecesForTemplate.Add(new Piece(pieceInCatalog.name,
                                                1,
                                                pieceInCatalog.cat,
                                                pieceInCatalog.isSystem));
            }

            Category robotCat = DeduceRobotCategory(piecesForTemplate);
            if (robotCat == Category.G)
                return "ERROR A robot cannot be of category G";

            bool allOk = piecesForTemplate.All(p =>
                Compatibility.IsCompatible(robotCat, p.cat, p.isSystem));

            if (!allOk)
                return "ERROR Pieces are not compatible with the deduced robot category";

            var newRobot = new Robot(templateName, 0, robotCat, piecesForTemplate);
            depot.robots.Add(newRobot);

            return "TEMPLATE_ADDED";
        }


        private Category DeduceRobotCategory(IEnumerable<Piece> pieces)
        {
            if (pieces.Any(p => p.cat == Category.M && !p.isSystem))
                return Category.M;

            if (pieces.Any(p => p.cat == Category.D))
                return Category.D;

            return Category.I;
        }

        public string ReceiveItems(Dictionary<string, int> items)
        {
            foreach (var kv in items)
            {
                string name = kv.Key;
                int qty = kv.Value;

                var robot = depot.robots
                                 .FirstOrDefault(r => r.name.Equals(name,
                                                    StringComparison.OrdinalIgnoreCase));
                if (robot != null)
                {
                    robot.quantite += qty;
                    continue;
                }

                var piece = depot.pieces
                                 .FirstOrDefault(p => p.name.Equals(name,
                                                    StringComparison.OrdinalIgnoreCase));
                if (piece != null)
                {
                    piece.quantite += qty;
                    continue;
                }

                return $"ERROR Unknown reference: {name}";
            }

            return "STOCK_UPDATED";
        }

        private static Dictionary<string, int> ApplyMods(Robot baseRobot,
                                                        List<ModItem> mods)
        {
            var req = baseRobot.GetRequiredPiecesForQuantity(1)
                               .ToDictionary(kv => kv.Key, kv => kv.Value);

            foreach (var m in mods)
            {
                if (m.Op == ModOp.WITH)
                {
                    req[m.PieceName] = req.GetValueOrDefault(m.PieceName, 0) + m.Qty;
                }
                else if (m.Op == ModOp.WITHOUT)
                {
                    if (!req.TryGetValue(m.PieceName, out int cur) || cur < m.Qty)
                        throw new InvalidOperationException(
                            $"Cannot remove {m.Qty} × {m.PieceName}");
                    req[m.PieceName] = cur - m.Qty;
                    if (req[m.PieceName] == 0) req.Remove(m.PieceName);
                }
                else
                {
                    if (!req.TryGetValue(m.PieceName, out int cur) || cur < m.Qty)
                        throw new InvalidOperationException(
                            $"Cannot replace {m.Qty} × {m.PieceName}");
                    req[m.PieceName] = cur - m.Qty;
                    if (req[m.PieceName] == 0) req.Remove(m.PieceName);
                }
            }
            return req;
        }
        internal string VerifyOrders(List<RobotOrder> orders)
        {
            foreach (var ord in orders)
            {
                var template = depot.robots.FirstOrDefault(r =>
                    r.name.Equals(ord.RobotName, StringComparison.OrdinalIgnoreCase));

                if (template is null)
                    return $"ERROR '{ord.RobotName}' unknown robot";

                Dictionary<string, int> pieces;
                try
                {
                    pieces = ApplyMods(template, ord.Mods);
                }
                catch (Exception ex)
                {
                    return $"ERROR {ex.Message}";
                }

                var virtualRobot = new Robot(template.name, 0, template.cat,
                    pieces.Select(kv =>
                    {
                        var pObj = depot.pieces.First(p => p.name == kv.Key);
                        return new Piece(pObj.name, kv.Value,
                                         pObj.cat, pObj.isSystem, pObj.isModule);
                    }).ToList());

                if (!virtualRobot.IsBuildable())
                    return $"ERROR Incompatible modifications on {ord.RobotName}";

                foreach (var kv in pieces)
                {
                    var ps = depot.pieces.First(p => p.name == kv.Key);
                    if (ps.quantite < kv.Value * ord.Qty)
                        return "UNAVAILABLE";
                }
            }
            return "AVAILABLE";
        }

        internal void ProduceOrders(List<RobotOrder> orders)
        {
            string chk = VerifyOrders(orders);
            if (chk != "AVAILABLE")
            {
                Console.WriteLine(chk);
                return;
            }

            foreach (var ord in orders)
            {
                var template = depot.robots.First(r => r.name.Equals(ord.RobotName,
                                             StringComparison.OrdinalIgnoreCase));

                var pieces1 = ApplyMods(template, ord.Mods);
                foreach (var kv in pieces1)
                {
                    var pieceObj = depot.pieces.First(p => p.name == kv.Key);
                    pieceObj.quantite -= kv.Value * ord.Qty;
                }
                template.quantite += ord.Qty;
            }
            Console.WriteLine("STOCK_UPDATED");
        }

        internal void DisplayNeededStockOrders(List<RobotOrder> orders)
        {
            foreach (var ord in orders)
            {
                var template = depot.robots.First(r => r.name.Equals(ord.RobotName,
                                             StringComparison.OrdinalIgnoreCase));

                var pieces1 = ApplyMods(template, ord.Mods);
                Console.WriteLine($"Robot : {ord.RobotName}, Quantité : {ord.Qty}");
                foreach (var kv in pieces1)
                    Console.WriteLine($"  {kv.Value * ord.Qty}  Pièce : {kv.Key}");
            }
        }

        internal void ProcessInstructionOrders(List<RobotOrder> orders)
        {
            foreach (var ord in orders)
            {
                Console.WriteLine($"[INSTRUCTIONS] {ord.Qty} × {ord.RobotName}");

            }
        }




    }










}
