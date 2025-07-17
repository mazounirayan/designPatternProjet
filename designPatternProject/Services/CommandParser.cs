using RobotFactory.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RobotFactory.Services
{
    public class CommandParser
    {
        private readonly Stock _stock;
        private readonly TemplateManager _templateManager;
        private readonly Validator _validator;
        private readonly InstructionBuilder _instructionBuilder;
        private readonly StockCalculator _stockCalculator;
        private readonly OrderService _orderService;
        private readonly StockHistoryLogger _logger;
        private readonly FactoryManager _factoryManager;

        private readonly TemplateValidator _templateValidator;




        public CommandParser(Stock stock, TemplateManager templateManager, Validator validator, InstructionBuilder instructionBuilder, StockCalculator stockCalculator,
        OrderService orderService, StockHistoryLogger logger, FactoryManager factoryManager, TemplateValidator templateValidator)
        {
            _stock = stock;
            _templateManager = templateManager;
            _validator = validator;
            _instructionBuilder = instructionBuilder;
            _stockCalculator = stockCalculator;
            _orderService = orderService;
            _logger = logger;
            _factoryManager = factoryManager;
            _templateValidator = templateValidator;
        }

        public void Process(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return;

            var parts = input.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var command = parts[0].ToUpper();

            switch (command)
            {
                case "STOCKS":
                    _stock.PrintInventory();
                    break;

                case "VERIFY":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("ERROR No arguments provided.");
                        break;
                    }

                    var args = ParseArgs(parts[1]);
                    var result = _validator.Verify(args);
                    Console.WriteLine(result);
                    break;

                case "RECEIVE":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("ERROR No arguments provided.");
                        break;
                    }

                    var receiveArgs = ParseArgs(parts[1]);

                    foreach (var kv in receiveArgs)
                    {
                        var name = kv.Key;
                        var qty = kv.Value;

                        if (_templateManager.Contains(name))
                        {
                            _stock.AddRobot(name, qty);
                            _logger.Log("RECEIVE", $"RECEIVE {qty} {name} (Robot)");
                        }
                        else
                        {
                            _stock.AddPiece(name, qty);
                            _logger.Log("RECEIVE", $"RECEIVE {qty} {name} (Piece)");
                        }
                    }

                    Console.WriteLine("STOCK_UPDATED");
                    break;


                case "INSTRUCTIONS":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("ERROR No arguments provided.");
                        break;
                    }

                    var robotArgs = ParseArgs(parts[1]);
                    var instructions = _instructionBuilder.BuildMultiple(robotArgs);
                    foreach (var instr in instructions)
                        Console.WriteLine(instr);
                    break;

                case "NEEDED_STOCKS":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("ERROR No arguments provided.");
                        break;
                    }

                    var neededArgs = ParseArgs(parts[1]);
                    _stockCalculator.PrintNeededStock(neededArgs);
                    break;

                case "PRODUCE":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("ERROR No arguments provided.");
                        break;
                    }

                    var (rawArgs, targetFactory) = ExtractFactoryTarget(parts[1]);

                    if (!_factoryManager.HasFactory(targetFactory))
                    {
                        Console.WriteLine($"ERROR Unknown factory `{targetFactory}`.");
                        Console.WriteLine("Available factories: " + string.Join(", ", _factoryManager.GetFactoryNames()));
                        break;
                    }

                    var factory = _factoryManager.GetFactory(targetFactory)!;

                    if (parts[1].Contains("WITH") || parts[1].Contains("WITHOUT") || parts[1].Contains("REPLACE") || parts[1].Contains(";"))
                    {
                        // 👇 MODIFIED version
                        var modCommands = ParseModifiedArgs(rawArgs);

                        

                        foreach (var cmd in modCommands)
                        {

                            if (!_templateManager.Contains(cmd.RobotName))
                            {
                                Console.WriteLine($"ERROR Unknown robot {cmd.RobotName}");
                                continue;
                            }

                            var template = _templateManager.GetTemplate(cmd.RobotName)!;
                            var required = new Dictionary<string, int>();

                            // 1. Base template
                            foreach (var piece in template.RequiredPieces)
                            {
                                if (!required.ContainsKey(piece)) required[piece] = 0;
                                required[piece] += 1;
                            }

                            // 2. Apply REPLACE
                           foreach (var (oldPiece, newPiece, qtyFrom, qtyTo) in cmd.Replaces)
                            {
                                if (required.ContainsKey(oldPiece))
                                    required[oldPiece] -= qtyFrom;

                                if (!required.ContainsKey(newPiece))
                                    required[newPiece] = 0;

                                required[newPiece] += qtyTo;
                            }

                            // 3. Apply WITHOUT
                            foreach (var wo in cmd.Without)
                            {
                                if (required.ContainsKey(wo.Key))
                                    required[wo.Key] -= wo.Value;
                            }

                            // 4. Apply WITH
                            foreach (var w in cmd.With)
                            {
                                if (!required.ContainsKey(w.Key)) required[w.Key] = 0;
                                required[w.Key] += w.Value;
                            }

                            // 5. Vérification des contraintes de catégorie
                            var usedCategories = required.Keys
                                .Where(pieceName => template != null)
                                .Select(p => _templateValidator.GetPieceCategory(p)) // à ajouter
                                .Distinct()
                                .ToList();
                            
                            if (!_templateValidator.IsValidPieceSetForCategory(template.Category, usedCategories, out var catError))
                            {
                                Console.WriteLine($"ERROR {cmd.RobotName} violates category rules: {catError}");
                                continue;
                            }

                            // 5. Vérif stock
                            var totalNeeded = required.Where(kv => kv.Value > 0)
                                                    .ToDictionary(kv => kv.Key, kv => kv.Value * cmd.Quantity);

                            bool stockOk = true;
                            foreach (var kv in totalNeeded)
                            {
                                var have = factory.Stock.GetPieceQuantity(kv.Key);
                                if (have < kv.Value)
                                {
                                    Console.WriteLine($"ERROR Not enough {kv.Key} in stock.");
                                    stockOk = false;
                                }
                            }

                            if (!stockOk) continue;

                            // 6. Mise à jour du stock
                            foreach (var kv in totalNeeded)
                            {
                                factory.Stock.PiecesStock[kv.Key] -= kv.Value;
                                _logger.Log("PRODUCE", $"GET_OUT_STOCK {kv.Value} {kv.Key} IN {targetFactory}");
                            }

                            for (int i = 0; i < cmd.Quantity; i++)
                            {
                                factory.Stock.AddRobot(cmd.RobotName, 1);
                                _logger.Log("PRODUCE", $"FINISHED {cmd.RobotName} (MODIFIED) IN {targetFactory}");
                            }

                            Console.WriteLine($"STOCK_UPDATED {cmd.RobotName} x{cmd.Quantity}");
                        }
                    }
                    else
                    {
                        // 👇 NORMAL version
                        var produceArgs = ParseArgs(rawArgs);

                        if (!_validator.IsValidCommand(produceArgs, out var err))
                        {
                            Console.WriteLine($"ERROR {err}");
                            break;
                        }

                        if (!_validator.IsStockSufficient(produceArgs, out var missing, factory.Stock))
                        {
                            Console.WriteLine("ERROR Stock insuffisant pour :");
                            foreach (var m in missing)
                                Console.WriteLine($"{m.Value} manquant(s) pour {m.Key}");
                            break;
                        }

                        var instructionsToRun = _instructionBuilder.BuildMultiple(produceArgs);

                        foreach (var instr in instructionsToRun)
                        {
                            if (instr.Type == "GET_OUT_STOCK")
                            {
                                var qty = int.Parse(instr.Args[0]);
                                var piece = instr.Args[1];
                                factory.Stock.PiecesStock[piece] -= qty;
                                _logger.Log("PRODUCE", instr.ToString() + $" IN {targetFactory}");
                            }
                            else if (instr.Type == "FINISHED")
                            {
                                var robot = instr.Args[0];
                                factory.Stock.AddRobot(robot, 1);
                                _logger.Log("PRODUCE", instr.ToString() + $" IN {targetFactory}");
                            }
                        }

                        Console.WriteLine("STOCK_UPDATED");
                    }

                     break;


                case "ORDER":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("ERROR No arguments provided.");
                        break;
                    }

                    var orderArgs = ParseArgs(parts[1]);

                    if (!_validator.IsValidCommand(orderArgs, out var orderErr))
                    {
                        Console.WriteLine($"ERROR {orderErr}");
                        break;
                    }

                    var orderId = _orderService.CreateOrder(orderArgs);
                    Console.WriteLine($"ORDER_CREATED {orderId}");
                    break;

                case "SEND":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("ERROR No arguments provided.");
                        break;
                    }

                    var sendParts = parts[1].Split(',', 2, StringSplitOptions.RemoveEmptyEntries);
                    if (sendParts.Length < 2)
                    {
                        Console.WriteLine("ERROR Invalid SEND format");
                        break;
                    }

                    var sendOrderId = sendParts[0].Trim();
                    var sendArgs = ParseArgs(sendParts[1]);

                    var sendResult = _orderService.SendRobots(sendOrderId, sendArgs, _stock);
                    Console.WriteLine(sendResult);
                    break;

                case "LIST_ORDER":
                    var list = _orderService.GetOrderList();
                    Console.WriteLine(list);
                    break;

                case "GET_MOVEMENTS":
                    if (parts.Length == 1)
                    {
                        foreach (var log in _logger.GetAll())
                            Console.WriteLine(log);
                    }
                    else
                    {
                        var filters = parts[1].Split(',', StringSplitOptions.RemoveEmptyEntries);
                        var logs = _logger.GetMatching(filters);

                        foreach (var log in logs)
                            Console.WriteLine(log);
                    }
                    break;


                case "ADD_TEMPLATE":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("ERROR No arguments provided.");
                        break;
                    }

                    var elements = parts[1].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(e => e.Trim()).ToList();
                    if (elements.Count < 2)
                    {
                        Console.WriteLine("ERROR Template must have at least one piece.");
                        break;
                    }

                    var templateName = elements[0];
                    var pieceList = elements.Skip(1).ToList();

                    if (_templateManager.Contains(templateName))
                    {
                        Console.WriteLine($"ERROR Template `{templateName}` already exists.");
                        break;
                    }

                    if (!_templateValidator.ValidateTemplate(templateName, pieceList, out var category, out var errMsg))
                    {
                        Console.WriteLine($"ERROR {errMsg}");
                        break;
                    }

                    _templateManager.AddTemplate(templateName, category, pieceList);
                    Console.WriteLine($"TEMPLATE_ADDED {templateName} ({category})");
                    break;

                case "TRANSFER":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("ERROR No arguments provided.");
                        break;
                    }

                    var splitTransfer = parts[1].Split(',', 3, StringSplitOptions.RemoveEmptyEntries);
                    if (splitTransfer.Length < 3)
                    {
                        Console.WriteLine("ERROR Format: TRANSFER Usine1, Usine2, ARGS");
                        break;
                    }

                    var fromFactoryName = splitTransfer[0].Trim();
                    var toFactoryName = splitTransfer[1].Trim();
                    var transferArgs = ParseArgs(splitTransfer[2]);

                    if (!_factoryManager.HasFactory(fromFactoryName) || !_factoryManager.HasFactory(toFactoryName))
                    {
                        Console.WriteLine("ERROR One of the factories is unknown.");
                        break;
                    }

                    var from = _factoryManager.GetFactory(fromFactoryName)!;
                    var to = _factoryManager.GetFactory(toFactoryName)!;

                    foreach (var kv in transferArgs)
                    {
                        var item = kv.Key;
                        var qty = kv.Value;

                        // Robot ou pièce ?
                        if (_templateManager.Contains(item))
                        {
                            if (from.Stock.GetRobotQuantity(item) < qty)
                            {
                                Console.WriteLine($"ERROR Not enough {item} in {fromFactoryName}");
                                break;
                            }
                            from.Stock.RobotStock[item] -= qty;
                            to.Stock.AddRobot(item, qty);
                        }
                        else
                        {
                            if (from.Stock.GetPieceQuantity(item) < qty)
                            {
                                Console.WriteLine($"ERROR Not enough {item} in {fromFactoryName}");
                                break;
                            }
                            from.Stock.PiecesStock[item] -= qty;
                            to.Stock.AddPiece(item, qty);
                        }

                        _logger.Log("TRANSFER", $"TRANSFER {qty} {item} from {fromFactoryName} to {toFactoryName}");
                    }

                    Console.WriteLine("TRANSFER_COMPLETED");
                    break;

                case "SAVE_STOCKS":
                    {
                        if (parts.Length < 2 || !parts[1].Contains("IN"))
                        {
                            Console.WriteLine("ERROR Usage: SAVE_STOCKS IN UsineX");
                            break;
                        }

                        var (_, factoryName) = ExtractFactoryTarget(parts[1]);

                        if (!_factoryManager.HasFactory(factoryName))
                        {
                            Console.WriteLine($"ERROR Unknown factory `{factoryName}`.");
                            break;
                        }

                        var targetFactoryObj = _factoryManager.GetFactory(factoryName)!;

                        var folder = "exports";
                        if (!Directory.Exists(folder))
                            Directory.CreateDirectory(folder);

                        var fileName = $"stock_{factoryName.ToLower()}.json";
                        var fullPath = Path.Combine(folder, fileName);

                        targetFactoryObj.Stock.SaveToJson(fullPath);

                        Console.WriteLine($"Stock saved to {fullPath}");
                        break;
                    }


                case "LOAD_STOCKS":
                    {
                        if (parts.Length < 2 || !parts[1].Contains("IN") || !parts[1].Contains("FROM"))
                        {
                            Console.WriteLine("ERROR Usage: LOAD_STOCKS IN UsineX FROM path/to/file.json");
                            break;
                        }

                        // Extraire "IN UsineX FROM file"
                        var split = parts[1].Split("FROM", 2, StringSplitOptions.RemoveEmptyEntries);
                        var factorySegment = split[0].Trim();
                        var fileSegment = split[1].Trim();

                        var (_, factoryName) = ExtractFactoryTarget(factorySegment);

                        if (!_factoryManager.HasFactory(factoryName))
                        {
                            Console.WriteLine($"ERROR Unknown factory `{factoryName}`.");
                            break;
                        }

                        var path = fileSegment;
                        if (!File.Exists(path))
                        {
                            Console.WriteLine($"ERROR File not found: {path}");
                            break;
                        }

                        var factoryStock = _factoryManager.GetFactory(factoryName)!;
                        try
                        {
                            factoryStock.Stock.LoadFromJson(path);
                            _logger.Log("LOAD", $"Loaded stock into {factoryName} from {path}");
                            Console.WriteLine($"Stock loaded from {path} into {factoryName}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"ERROR Loading failed: {ex.Message}");
                        }

                        break;
                    }

                case "SAVE_ALL_STOCKS":
                    {
                        var folder = "exports";
                        if (!Directory.Exists(folder))
                            Directory.CreateDirectory(folder);

                        foreach (var factoryName in _factoryManager.GetFactoryNames())
                        {
                            var factorySave = _factoryManager.GetFactory(factoryName)!;

                            var fileName = $"stock_{factoryName.ToLower()}.json";
                            var fullPath = Path.Combine(folder, fileName);

                            factorySave.Stock.SaveToJson(fullPath);

                            _logger.Log("SAVE_ALL", $"Saved {factoryName} stock to {fileName}");
                        }

                        Console.WriteLine("All factory stocks saved to 'exports/' folder.");
                        break;
                    }

                case "REPORT":
                    {
                        if (parts.Length < 2 || !parts[1].Contains("IN"))
                        {
                            Console.WriteLine("ERROR Usage: REPORT IN UsineX");
                            break;
                        }

                        var (_, factoryName) = ExtractFactoryTarget(parts[1]);

                        if (!_factoryManager.HasFactory(factoryName))
                        {
                            Console.WriteLine($"ERROR Unknown factory `{factoryName}`.");
                            break;
                        }

                        var factoryREPORT = _factoryManager.GetFactory(factoryName)!;

                        Console.WriteLine($"\n===== REPORT: {factoryName.ToUpper()} =====\n");

                        Console.WriteLine("🦾 Robots produits :");
                        if (factoryREPORT.Stock.RobotStock.Count == 0)
                            Console.WriteLine("  Aucun robot en stock.");
                        else
                            foreach (var robot in factoryREPORT.Stock.RobotStock)
                                Console.WriteLine($"  {robot.Key} : {robot.Value}");

                        Console.WriteLine("\n🧩 Pièces restantes :");
                        if (factoryREPORT.Stock.PiecesStock.Count == 0)
                            Console.WriteLine("  Aucune pièce en stock.");
                        else
                            foreach (var piece in factoryREPORT.Stock.PiecesStock)
                                Console.WriteLine($"  {piece.Key} : {piece.Value}");

                        Console.WriteLine("\n📜 Derniers mouvements :");
                        var logs = _logger.GetMatching(factoryName).Reverse().Take(10).ToList();

                        if (logs.Count == 0)
                            Console.WriteLine("  Aucun mouvement enregistré.");
                        else
                            foreach (var log in logs)
                                Console.WriteLine($"  {log}");

                        Console.WriteLine("\n====================================\n");
                        break;
                    }


                default:
                    Console.WriteLine("Commande inconnue.");
                    break;
            }
        }

        private Dictionary<string, int> ParseArgs(string argLine)
        {
            var result = new Dictionary<string, int>();
            var parts = argLine.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                var trimmed = part.Trim();
                var spaceIdx = trimmed.IndexOf(' ');
                if (spaceIdx <= 0) continue;

                var quantityStr = trimmed.Substring(0, spaceIdx);
                var robotName = trimmed.Substring(spaceIdx + 1);

                if (int.TryParse(quantityStr, out int qty))
                {
                    if (!result.ContainsKey(robotName))
                        result[robotName] = 0;
                    result[robotName] += qty;
                }
            }

            return result;
        }

        private (string, string) ExtractFactoryTarget(string input)
        {
            if (input.Contains("IN"))
            {
                var parts = input.Split("IN", 2, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
                {
                    return (parts[0].Trim(), "DEFAULT");
                }

                var main = parts[0].Trim();
                var factoryName = parts[1].Trim();
                return (main, factoryName);
            }

            return (input.Trim(), "DEFAULT");
        }
        
        private List<ModifiedCommand> ParseModifiedArgs(string argLine)
        {
            var commands = new List<ModifiedCommand>();

            var blocks = argLine.Split(';', StringSplitOptions.RemoveEmptyEntries);

            foreach (var block in blocks)
            {
                var trimmed = block.Trim();
                if (string.IsNullOrWhiteSpace(trimmed)) continue;

                var tokens = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length < 2) continue;

                if (!int.TryParse(tokens[0], out var qty)) continue;

                var robotName = tokens[1];
                var cmd = new ModifiedCommand(robotName, qty);

                var i = 2;
                while (i < tokens.Length)
                {
                    var keyword = tokens[i].ToUpper();
                    i++;

                    if (keyword == "WITH")
                    {
                        while (i + 1 < tokens.Length && int.TryParse(tokens[i], out var withQty))
                        {
                            cmd.With[tokens[i + 1]] = withQty;
                            i += 2;
                        }
                    }
                    else if (keyword == "WITHOUT")
                    {
                        while (i + 1 < tokens.Length && int.TryParse(tokens[i], out var withoutQty))
                        {
                            cmd.Without[tokens[i + 1]] = withoutQty;
                            i += 2;
                        }
                    }
                    else if (keyword == "REPLACE")
                    {
                        while (i + 3 < tokens.Length &&
                            int.TryParse(tokens[i], out var repQty1) &&
                            int.TryParse(tokens[i + 2], out var repQty2))
                        {
                            var piece1 = tokens[i + 1];
                            var piece2 = tokens[i + 3];
                            cmd.Replace[$"{piece1}->{piece2}"] = (repQty1, repQty2);
                            i += 4;
                        }
                    }
                    else
                    {
                        i++;
                    }
                }

                commands.Add(cmd);
            }

            return commands;
        }


    }
}
