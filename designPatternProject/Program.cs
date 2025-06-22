// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Linq;
using designPatternProject;

class Program
{
    static Stock init()
    {
        var piecesTotal = new List<Piece>
    {
        new Piece("Core_CM1", 10, Category.M),
        new Piece("Core_CD1", 10, Category.D),
        new Piece("Core_CI1", 10, Category.I),

        new Piece("Generator_GM1", 10, Category.M),
        new Piece("Generator_GD1", 10, Category.D),
        new Piece("Generator_GI1", 10, Category.I),

        new Piece("Arms_AM1", 10, Category.M),
        new Piece("Arms_AD1", 10, Category.D),
        new Piece("Arms_AI1", 10, Category.I),

        new Piece("Legs_LM1", 10, Category.M),
        new Piece("Legs_LD1", 10, Category.D),
        new Piece("Legs_LI1", 10, Category.I),

    };

        var partsXM = new List<Piece>
    {
        new Piece("Core_CM1",       1, Category.M),
        new Piece("Generator_GM1",  1, Category.M),
        new Piece("Arms_AM1",       1, Category.M),
        new Piece("Legs_LM1",       1, Category.M)
    };

        var partsRD = new List<Piece>
    {
        new Piece("Core_CD1",       1, Category.D),
        new Piece("Generator_GD1",  1, Category.D),
        new Piece("Arms_AD1",       1, Category.D),
        new Piece("Legs_LD1",       1, Category.D)
    };

        var partsWI = new List<Piece>
    {
        new Piece("Core_CI1",       1, Category.I),
        new Piece("Generator_GI1",  1, Category.I),
        new Piece("Arms_AI1",       1, Category.I),
        new Piece("Legs_LI1",       1, Category.I)
    };

        var robots = new List<Robot>
    {
        new Robot("XM-1", 2, Category.M, partsXM),
        new Robot("RD-1", 2, Category.D, partsRD),
        new Robot("WI-1", 2, Category.I, partsWI)
    };

        return new Stock(piecesTotal, robots);
    }


    static void Main()
    {
        var stock = init();
        var factory = new Factory(stock);

        Console.WriteLine("Bienvenue. Tapez vos instructions (EXIT pour quitter).");

        while (true)
        {
            Console.Write("> ");
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Commande vide !");
                continue;
            }
            if (input.Trim().Equals("EXIT", StringComparison.OrdinalIgnoreCase))
                break;

            int idxSpace = input.IndexOf(' ');
            if (idxSpace == -1)
            {
                if (input.Equals("STOCKS", StringComparison.OrdinalIgnoreCase))
                    factory.displayStock();
                else if (input.Equals("GET_MOVEMENTS", StringComparison.OrdinalIgnoreCase))
                    factory.DisplayMovements(Array.Empty<string>());
                else
                    Console.WriteLine("ERROR Commande vide ou incorrecte.");
                continue;
            }

            string instr = input[..idxSpace].Trim().ToUpperInvariant();
            string argsStr = input[(idxSpace + 1)..].Trim();

            if (instr == "ADD_TEMPLATE")
            {
                var parts = argsStr.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(p => p.Trim()).ToList();
                if (parts.Count < 2)
                {
                    Console.WriteLine("ERROR Mauvais format : NOM_TEMPLATE, pièce1, …");
                    continue;
                }
                Console.WriteLine(factory.AddTemplate(parts[0], parts.Skip(1).ToList()));
                continue;
            }

            if (instr == "RECEIVE")
            {
                var rec = TraiterCommande(argsStr);
                Console.WriteLine(factory.ReceiveItems(rec, input));
                continue;
            }

            if (instr == "GET_MOVEMENTS")
            {
                var filt = argsStr.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(s => s.Trim())
                                  .Where(s => s.Length > 0)
                                  .ToArray();
                factory.DisplayMovements(filt);
                continue;
            }

            bool containsMods = argsStr.Contains(" WITH ", StringComparison.OrdinalIgnoreCase) ||
                                argsStr.Contains(" WITHOUT ", StringComparison.OrdinalIgnoreCase) ||
                                argsStr.Contains(" REPLACE ", StringComparison.OrdinalIgnoreCase) ||
                                argsStr.Contains(';');

            if (containsMods)
            {
                var orders = ParseRobotOrders(argsStr);

                switch (instr)
                {
                    case "VERIFY":
                        Console.WriteLine(factory.VerifyOrders(orders));
                        break;

                    case "PRODUCE":
                        factory.ProduceOrders(orders, input);
                        break;

                    case "NEEDED_STOCKS":
                        factory.DisplayNeededStockOrders(orders);
                        break;

                    case "INSTRUCTIONS":
                        factory.ProcessInstructionOrders(orders);
                        break;

                    default:
                        Console.WriteLine("ERROR Commande inconnue.");
                        break;
                }
            }
            else
            {
                Dictionary<string, int> cmds = TraiterCommande(argsStr);

                switch (instr)
                {
                    case "NEEDED_STOCKS":
                        factory.displayNeededStock(cmds);
                        break;

                    case "INSTRUCTIONS":
                        factory.ProcessInstructionCommand(cmds);
                        break;

                    case "VERIFY":
                        Console.WriteLine(factory.VerifyCommand(cmds));
                        break;

                    case "PRODUCE":
                        factory.ProduceCommand(cmds, input);
                        break;

                    default:
                        Console.WriteLine("ERROR Commande inconnue.");
                        break;
                }
            }
        }
        Console.WriteLine("Programme terminé.");
    }


    static Dictionary<string, int> TraiterCommande(string elements)
    {
        var commandes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(elements))
            return commandes;

        var robots = elements.Split(',');

        foreach (var robot in robots)
        {
            var parts = robot.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);


            if (int.TryParse(parts[0], out int quantite))
            {
                string robotName = string.Join(" ", parts.Skip(1)).Trim();
                if (commandes.ContainsKey(robotName))
                {
                    commandes[robotName] += quantite;
                }
                else
                {
                    commandes[robotName] = quantite;
                }
            }
            else
            {
                Console.WriteLine($"Quantité invalide pour la commande : {robot}");
            }
        }

        return commandes;
    }

    static List<RobotOrder> ParseRobotOrders(string args)
    {
        char robotSep = args.Contains(" WITH ", StringComparison.OrdinalIgnoreCase) ||
                        args.Contains(" WITHOUT ", StringComparison.OrdinalIgnoreCase) ||
                        args.Contains(" REPLACE ", StringComparison.OrdinalIgnoreCase)
                        ? ';' : ',';

        var result = new List<RobotOrder>();

        foreach (var raw in args.Split(robotSep, StringSplitOptions.RemoveEmptyEntries))
        {
            var tokens = raw.Trim().Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length < 2 || !int.TryParse(tokens[0], out int qty))
            {
                Console.WriteLine($"ERROR Bad robot spec : {raw.Trim()}");
                continue;
            }

            string robotName = tokens[1];
            var mods = new List<ModItem>();

            if (tokens.Length == 3)
            {
                string tail = tokens[2];

                var opChunks = tail.Split(new[] { " WITH ", " WITHOUT ", " REPLACE " },
                                          StringSplitOptions.RemoveEmptyEntries);

                int pos = 0;
                foreach (var chunk in opChunks)
                {
                    int kwIdx = tail.IndexOf(chunk, pos, StringComparison.Ordinal);
                    string kw = tail.Substring(pos == 0 ? 0 : pos - 7, 7).Trim();
                    pos = kwIdx + chunk.Length;

                    ModOp op = kw.Equals("WITH", StringComparison.OrdinalIgnoreCase) ? ModOp.WITH :
                               kw.Equals("WITHOUT", StringComparison.OrdinalIgnoreCase) ? ModOp.WITHOUT :
                                                                                            ModOp.REPLACE;

                    foreach (var p in chunk.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        var pt = p.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                        if (pt.Length != 2 || !int.TryParse(pt[0], out int pQty))
                        {
                            Console.WriteLine($"ERROR Bad piece spec : {p.Trim()}");
                            continue;
                        }
                        string pieceName = pt[1].Trim();
                        if (op == ModOp.REPLACE)
                        {
                            mods.Add(new ModItem(ModOp.REPLACE, pieceName, pQty));
                        }
                        else mods.Add(new ModItem(op, pieceName, pQty));
                    }
                }
            }

            result.Add(new RobotOrder(robotName, qty, mods));
        }
        return result;
    }


}
