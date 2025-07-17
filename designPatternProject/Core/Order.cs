using System.Collections.Generic;

namespace RobotFactory.Core
{
    public class Order
    {
        public string Id { get; set; }
        public Dictionary<string, int> RobotsRequested { get; set; } = new();

        public Order(string id, Dictionary<string, int> robotsRequested)
        {
            Id = id;
            RobotsRequested = robotsRequested;
        }

        public override string ToString()
        {
            var parts = new List<string>();
            foreach (var kv in RobotsRequested)
                parts.Add($"{kv.Value} {kv.Key}");
            return $"{Id}: {string.Join(", ", parts)}";
        }
    }
}
