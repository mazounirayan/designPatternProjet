using System.Collections.Generic;

namespace RobotFactory.Core
{
    public class Factory
    {
        public string Name { get; set; }
        public Stock Stock { get; set; }
        public Dictionary<string, Order> Orders { get; set; } = new();

        public Factory(string name)
        {
            Name = name;
            Stock = new Stock();
        }

        public void AddOrder(Order order)
        {
            Orders[order.Id] = order;
        }

        public override string ToString() => Name;
    }
}
