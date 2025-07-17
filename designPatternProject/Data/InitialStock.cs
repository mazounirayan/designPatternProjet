using RobotFactory.Core;
using RobotFactory.Enums;
using System.Collections.Generic;

namespace RobotFactory.Data
{
    public static class InitialStock
    {
        public static void LoadInitialStock(Stock stock)
        {
            // Pièces principales (Core)
            stock.AddPiece("Core_CM1", 10); // Militaire
            stock.AddPiece("Core_CD1", 10); // Domestique
            stock.AddPiece("Core_CI1", 10); // Industriel

            // Générateurs
            stock.AddPiece("Generator_GM1", 10); // Militaire
            stock.AddPiece("Generator_GD1", 10); // Domestique
            stock.AddPiece("Generator_GI1", 10); // Industriel

            // Modules de préhension
            stock.AddPiece("Arms_AM1", 10); // Militaire
            stock.AddPiece("Arms_AD1", 10); // Domestique
            stock.AddPiece("Arms_AI1", 10); // Industriel

            // Modules de déplacement
            stock.AddPiece("Legs_LM1", 10); // Militaire
            stock.AddPiece("Legs_LD1", 10); // Domestique
            stock.AddPiece("Legs_LI1", 10); // Industriel

            // Systèmes principaux (nécessaires pour Core)
            stock.AddPiece("System_SB1", 10); // Généraliste
            stock.AddPiece("System_SM1", 10); // Militaire
            stock.AddPiece("System_SD1", 10); // Domestique
            stock.AddPiece("System_SI1", 10); // Industriel

            // Robots produits déjà en stock (optionnel)
            stock.AddRobot("XM-1", 2);
            stock.AddRobot("RD-1", 3);
            stock.AddRobot("WI-1", 1);
        }
    }
}
