using RobotFactory.Core;
using RobotFactory.Enums;
using System.Collections.Generic;
using System.Linq;

namespace RobotFactory.Services
{
    public class TemplateValidator
    {
        private readonly Dictionary<string, Piece> _knownPieces;

        public TemplateValidator()
        {
            // Pièces connues en base (simplification)
            _knownPieces = new List<Piece>
            {
                new("Core_CD1", Category.Domestic),
                new("Core_CM1", Category.Military),
                new("Core_CI1", Category.Industrial),

                new("Generator_GD1", Category.Domestic),
                new("Generator_GM1", Category.Military),
                new("Generator_GI1", Category.Industrial),

                new("Arms_AD1", Category.Domestic),
                new("Arms_AM1", Category.Military),
                new("Arms_AI1", Category.Industrial),

                new("Legs_LD1", Category.Domestic),
                new("Legs_LM1", Category.Military),
                new("Legs_LI1", Category.Industrial),

                new("System_SB1", Category.Generalist),
                new("System_SD1", Category.Domestic),
                new("System_SM1", Category.Military),
                new("System_SI1", Category.Industrial),
            }.ToDictionary(p => p.Name);
        }

        public bool ValidateTemplate(string name, List<string> pieces, out Category category, out string error)
        {
            category = Category.Generalist;

            if (pieces.Count < 5)
            {
                error = "A robot must have at least 5 pieces.";
                return false;
            }

            // Check all pieces exist
            foreach (var piece in pieces)
            {
                if (!_knownPieces.ContainsKey(piece))
                {
                    error = $"Unknown piece `{piece}`.";
                    return false;
                }
            }

            // Déterminer la catégorie maximale autorisée
            var categories = pieces.Select(p => _knownPieces[p].Category).Distinct().ToList();

            if (categories.Contains(Category.Military))
                category = Category.Military;
            else if (categories.Contains(Category.Industrial))
                category = Category.Industrial;
            else if (categories.Contains(Category.Domestic))
                category = Category.Domestic;
            else
            {
                error = "A robot cannot be Generalist only.";
                return false;
            }

            // Vérification des contraintes
            if (category == Category.Domestic && categories.Any(c => c == Category.Military))
            {
                error = "Domestic robot cannot contain military pieces.";
                return false;
            }

            if (category == Category.Industrial && categories.Any(c => c == Category.Domestic || c == Category.Military))
            {
                error = "Industrial robot cannot contain domestic or military pieces.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        public bool IsValidPieceSetForCategory(Category robotCategory, List<Category> usedCategories, out string error)
        {
            error = string.Empty;

            if (robotCategory == Category.Domestic)
            {
                if (usedCategories.Any(c => c == Category.Military))
                {
                    error = "Domestique robots can't contain military pieces.";
                    return false;
                }
            }
            else if (robotCategory == Category.Industrial)
            {
                if (usedCategories.Any(c => c == Category.Domestic || c == Category.Military))
                {
                    error = "Industriel robots can't contain domestic or military pieces.";
                    return false;
                }
            }
            else if (robotCategory == Category.Military)
            {
                if (usedCategories.Any(c => c == Category.Domestic))
                {
                    error = "Militaire robots can't contain domestic pieces.";
                    return false;
                }
            }

            return true;
        }

        public Category GetPieceCategory(string pieceName)
        {
            if (_knownPieces.TryGetValue(pieceName, out var piece))
                return piece.Category;
            return Category.Generalist;
        }

    }
}
