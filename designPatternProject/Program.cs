using RobotFactory.Core;
using RobotFactory.Data;
using RobotFactory.Services;

class Program
{
    static void Main(string[] args)
    {
        // Initialisation des composants
        var stock = new Stock();
        InitialStock.LoadInitialStock(stock);

        var templateManager = new TemplateManager();
        var validator = new Validator(templateManager, stock);
        var instructionBuilder = new InstructionBuilder(templateManager);
        var stockCalculator = new StockCalculator(templateManager);
        var orderService = new OrderService();
        var logger = new StockHistoryLogger();
        var templateValidator = new TemplateValidator();

        var factoryManager = new FactoryManager(new[] { "DEFAULT", "Usine1", "Usine2" });

        var parser = new CommandParser(
            stock,
            templateManager,
            validator,
            instructionBuilder,
            stockCalculator,
            orderService,
            logger,
            factoryManager,      // 8e paramètre : FactoryManager
            templateValidator    // 9e paramètre : TemplateValidator
        );


        var defaultFactory = factoryManager.GetFactory("DEFAULT");
        InitialStock.LoadInitialStock(defaultFactory.Stock);


        // Lancement du shell interactif
        Console.WriteLine("Bienvenue dans l'usine de robots !");
        Console.WriteLine("Tapez EXIT pour quitter.");
        while (true)
        {
            Console.Write("\n> ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) continue;
            if (input.Trim().ToUpper() == "EXIT") break;

            parser.Process(input);
        }
    }
}
