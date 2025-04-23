// See https://aka.ms/new-console-template for more information
using System.Text;
using System.Xml.Linq;
using designPatternProject;


class TestClass
{
    static void Main(string[] args)
    {


        StringBuilder builder = new();
        builder.AppendLine("The following arguments are passed:");
        Console.WriteLine("Entrez votre commande:");
        string nom = Console.ReadLine();


        Console.WriteLine(builder.ToString());

        Console.WriteLine(args.Length);
        string instruc = "1";
        Robot robot = new Robot("robot1", 2);
        Robot robot2 = new Robot("robot2", 2);
        Robot robot3 = new Robot("robot3", 2);
        robot.Pieces.Add(new Piece());
        robot.Pieces.Add(new Piece() { Name = "piece2", Quantite = 2 });
        robot.Pieces.Add(new Piece() { Name = "piece3", Quantite = 2 });
        var pieces = new List<Piece>();

        pieces.Add(new Piece() { Name = "piece1", Quantite = 2 });
        pieces.Add(new Piece() { Name = "piece2", Quantite = 3 });


       var  robots = new List<Robot>();
        robots.Add(robot);
        robots.Add(robot2);
        robots.Add(robot3);





        Stock stock = new Stock(pieces, robots);
        switch (nom)
        {
            case "STOCKS":
                foreach (var item in stock.Robots  )
                {
                    Console.WriteLine(item.Quantite + " " + item.Name);
                }
                foreach (var item in stock.Pieces)
                {
                    Console.WriteLine(item.Quantite +" "+ item.Name);
                }
                break;
        }
    }
}