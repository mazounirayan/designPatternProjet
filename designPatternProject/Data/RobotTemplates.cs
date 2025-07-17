using RobotFactory.Core;
using RobotFactory.Enums;
using System.Collections.Generic;

namespace RobotFactory.Data
{
    public static class RobotTemplates
    {
        public static Dictionary<string, RobotTemplate> LoadTemplates()
        {
            return new Dictionary<string, RobotTemplate>
            {
                {
                    "XM-1", new RobotTemplate(
                        name: "XM-1",
                        category: Category.Military,
                        requiredPieces: new List<string>
                        {
                            "Core_CM1", "Generator_GM1", "Arms_AM1", "Legs_LM1", "System_SB1"
                        }
                    )
                },
                {
                    "RD-1", new RobotTemplate(
                        name: "RD-1",
                        category: Category.Domestic,
                        requiredPieces: new List<string>
                        {
                            "Core_CD1", "Generator_GD1", "Arms_AD1", "Legs_LD1", "System_SB1"
                        }
                    )
                },
                {
                    "WI-1", new RobotTemplate(
                        name: "WI-1",
                        category: Category.Industrial,
                        requiredPieces: new List<string>
                        {
                            "Core_CI1", "Generator_GI1", "Arms_AI1", "Legs_LI1", "System_SB1"
                        }
                    )
                }
            };
        }
    }
}
