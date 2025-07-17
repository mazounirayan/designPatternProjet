using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace RobotFactory.Core
{

    public class Stock
    {
        public Dictionary<string, int> PiecesStock { get; set; } = new();
        public Dictionary<string, int> RobotStock { get; set; } = new();

        public void AddPiece(string pieceName, int quantity)
        {
            if (!PiecesStock.ContainsKey(pieceName))
                PiecesStock[pieceName] = 0;
            PiecesStock[pieceName] += quantity;
        }

        public void AddRobot(string robotName, int quantity)
        {
            if (!RobotStock.ContainsKey(robotName))
                RobotStock[robotName] = 0;
            RobotStock[robotName] += quantity;
        }

        public bool RemovePiece(string name, int qty)
        {
            if (!PiecesStock.ContainsKey(name) || PiecesStock[name] < qty)
                return false;

            PiecesStock[name] -= qty;
            return true;
        }


        public int GetPieceQuantity(string pieceName) =>
            PiecesStock.TryGetValue(pieceName, out var qty) ? qty : 0;

        public int GetRobotQuantity(string robotName) =>
            RobotStock.TryGetValue(robotName, out var qty) ? qty : 0;

        public void PrintInventory()
        {
            Console.WriteLine("Robots :");
            foreach (var kv in RobotStock)
                Console.WriteLine($"{kv.Value} {kv.Key}");

            Console.WriteLine("Pièces :");
            foreach (var kv in PiecesStock)
                Console.WriteLine($"{kv.Value} {kv.Key}");
        }

        public void SaveToJson(string filePath)
        {
            var export = new
            {
                Robots = this.RobotStock,
                Pieces = this.PiecesStock
            };

            var json = JsonSerializer.Serialize(export, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
        
        public void LoadFromJson(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            var json = File.ReadAllText(filePath);

            var data = JsonSerializer.Deserialize<StockData>(json);

            if (data != null)
            {
                PiecesStock = data.Pieces ?? new();
                RobotStock = data.Robots ?? new();
            }
        }

        private class StockData
        {
            public Dictionary<string, int>? Robots { get; set; }
            public Dictionary<string, int>? Pieces { get; set; }
        }

    }
}
