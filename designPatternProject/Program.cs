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
          new Piece ("Core_CM1", 10),
            new Piece ("Core_CD1", 10),
            new Piece ("Core_CI1", 10),
            new Piece ("Generator_GM1", 10),
            new Piece ("Generator_GD1", 10),
            new Piece ("Generator_GI1", 10),
            new Piece ("Arms_AM1", 10),
            new Piece ("Arms_AD1", 10),
            new Piece ("Arms_AI1", 10),
            new Piece ("Legs_LM1", 10),
            new Piece ("Legs_LD1", 10),
            new Piece ("Legs_LI1", 10)


        };
        var pieces1 = new List<Piece>
        {
          new Piece ( "Core_CM1",  1 ),
            new Piece ("Generator_GM1", 1),
            new Piece ("Arms_AM1", 1),
            new Piece ("Legs_LM1", 1),

        };
        var pieces2 = new List<Piece>
        {
            new Piece ("Core_CD1", 1),
            new Piece ("Generator_GD1", 1),
            new Piece ("Arms_AD1", 1),
            new Piece ("Legs_LD1", 1),

        };
        var pieces3 = new List<Piece>
        {
            new Piece ("Core_CI1", 1),
            new Piece ("Generator_GI1", 1),
            new Piece ("Arms_AI1", 1),
            new Piece ("Legs_LI1", 1),

        };
        Robot robot1 = new Robot("XM-1", 2, pieces1);
        Robot robot2 = new Robot("RD-1", 2, pieces2);
        Robot robot3 = new Robot("WI-1", 2, pieces3);

        var robots = new List<Robot> { robot1, robot2, robot3 };

        return new Stock(piecesTotal, robots);
    }

    static void Main()
    {

        var stock = init();
        Factory factory = new Factory(stock);
        Console.WriteLine("Veuillez entrer votre commande :");
        string input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Commande vide !");
            return;
        }

        int indexPremierEspace = input.IndexOf(' ');

        if (indexPremierEspace != -1)
        {
            var USER_INSTRUCTION = input.Substring(0, indexPremierEspace).Trim();

            var elements = input.Substring(indexPremierEspace + 1).Trim();



            Dictionary<string, int> commandes = TraiterCommande(elements);



            switch (USER_INSTRUCTION)
            {
                case "STOCKS":
                    factory.displayStock();
                    break;
                case "NEEDED_STOCKS":

                    factory.displayNeededStock(commandes);


                    break;
                case "INSTRUCTIONS":
                    factory.ProcessInstructionCommand(commandes);
                    break;
                case "VERIFY":
                    string result = factory.VerifyCommand(commandes);
                    if (result.StartsWith("ERROR"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(result);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine(result);
                    }
                    break;
                case "PRODUCE":
                    factory.ProduceCommand(commandes);
                    break;

            }


        }
        else
        {
            Console.WriteLine("Format invalide !");
        }


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

            //if (parts.Length >= 1)
            //{
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
            //}
            //else
            //{
            //    Console.WriteLine($"Commande mal formatée : {robot}");
            //}
        }

        return commandes;
    }

}
