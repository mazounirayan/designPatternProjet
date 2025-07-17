using System;
using System.Collections.Generic;
using RobotFactory.Core;

namespace RobotFactory.Services
{
    public class Validator
    {
        private readonly TemplateManager _templateManager;
        private readonly Stock _stock;

        public Validator(TemplateManager templateManager, Stock stock)
        {
            _templateManager = templateManager;
            _stock = stock;
        }

        public bool IsValidCommand(Dictionary<string, int> robotArgs, out string error)
        {
            foreach (var kv in robotArgs)
            {
                if (!_templateManager.Contains(kv.Key))
                {
                    error = $"`{kv.Key}` is not a recognized robot";
                    return false;
                }
            }

            error = string.Empty;
            return true;
        }

        public bool IsStockSufficient(Dictionary<string, int> robotArgs, out Dictionary<string, int> missing, Stock stock)

        {
            missing = new Dictionary<string, int>();

            var totalNeeded = new Dictionary<string, int>();

            // Accumuler les pièces nécessaires
            foreach (var kv in robotArgs)
            {
                var template = _templateManager.GetTemplate(kv.Key);
                if (template == null) continue;

                foreach (var piece in template.RequiredPieces)
                {
                    if (!totalNeeded.ContainsKey(piece))
                        totalNeeded[piece] = 0;

                    totalNeeded[piece] += kv.Value;
                }
            }

            // Comparer au stock
            foreach (var kv in totalNeeded)
            {
                var available = _stock.GetPieceQuantity(kv.Key);
                if (available < kv.Value)
                    missing[kv.Key] = kv.Value - available;
            }

            return missing.Count == 0;
        }

        public string Verify(Dictionary<string, int> robotArgs)
        {
            if (!IsValidCommand(robotArgs, out var error))
                return $"ERROR {error}";

            if (IsStockSufficient(robotArgs, out _, _stock))
                return "AVAILABLE";

            return "UNAVAILABLE";
        }
    }
}
