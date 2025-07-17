using RobotFactory.Enums;

namespace RobotFactory.Core
{
    public class Piece
    {
        public string Name { get; set; }
        public Category Category { get; set; }
        public bool RequiresInstallation { get; set; } = false; // Ex : Core_CM1

        public Piece(string name, Category category, bool requiresInstallation = false)
        {
            Name = name;
            Category = category;
            RequiresInstallation = requiresInstallation;
        }

        public override string ToString() => Name;

    }
}
