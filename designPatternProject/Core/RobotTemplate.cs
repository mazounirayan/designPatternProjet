using System.Collections.Generic;
using RobotFactory.Enums;

namespace RobotFactory.Core
{
    public class RobotTemplate
    {
        public string Name { get; set; }
        public Category Category { get; set; }

        public List<string> RequiredPieces { get; set; } = new();

        public RobotTemplate(string name, Category category, List<string> requiredPieces)
        {
            Name = name;
            Category = category;
            RequiredPieces = requiredPieces;
        }
    }
}
