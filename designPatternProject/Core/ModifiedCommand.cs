using System.Collections.Generic;

namespace RobotFactory.Core
{
    public class ModifiedCommand
    {
        public string RobotName { get; set; }
        public int Quantity { get; set; }

        public Dictionary<string, int> With { get; set; } = new();
        public Dictionary<string, int> Without { get; set; } = new();
        public Dictionary<string, (int, int)> Replace { get; set; } = new(); // (qty1, qty2)

        public ModifiedCommand(string name, int qty)
        {
            RobotName = name;
            Quantity = qty;
        }

        public List<(string OldPiece, string NewPiece, int QtyFrom, int QtyTo)> Replaces { get; set; } = new(); // ✅

    }
}

