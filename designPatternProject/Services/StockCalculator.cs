using RobotFactory.Core;
using System;
using System.Collections.Generic;

namespace RobotFactory.Services
{
    public class StockCalculator
    {
        private readonly TemplateManager _templateManager;

        public StockCalculator(TemplateManager templateManager)
        {
            _templateManager = templateManager;
        }

        public void PrintNeededStock(Dictionary<string, int> robotArgs)
        {
            var total = new Dictionary<string, int>();

            foreach (var kv in robotArgs)
            {
                var template = _templateManager.GetTemplate(kv.Key);
                if (template == null) continue;

                Console.WriteLine($"{kv.Key} :");

                var local = new Dictionary<string, int>();

                foreach (var piece in template.RequiredPieces)
                {
                    if (!local.ContainsKey(piece))
                        local[piece] = 0;
                    local[piece] += kv.Value;

                    if (!total.ContainsKey(piece))
                        total[piece] = 0;
                    total[piece] += kv.Value;
                }

                foreach (var p in local)
                    Console.WriteLine($"{p.Value} {p.Key}");
            }

            Console.WriteLine("\nTotal :");
            foreach (var kv in total)
                Console.WriteLine($"{kv.Value} {kv.Key}");
        }
    }
}
