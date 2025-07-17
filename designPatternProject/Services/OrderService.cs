using System;
using System.Collections.Generic;
using RobotFactory.Core;

namespace RobotFactory.Services
{
    public class OrderService
    {
        private int _nextId = 1;
        private readonly Dictionary<string, Order> _orders = new();

        public string CreateOrder(Dictionary<string, int> robots)
        {
            string id = $"ORDER{_nextId++}";
            var order = new Order(id, robots);
            _orders[id] = order;
            return id;
        }

        public bool HasOrder(string id) => _orders.ContainsKey(id);

        public Order? GetOrder(string id) => _orders.GetValueOrDefault(id);

        public IEnumerable<Order> GetPendingOrders() => _orders.Values;


        public string SendRobots(string orderId, Dictionary<string, int> toSend, Stock stock)
        {
            if (!_orders.TryGetValue(orderId, out var order))
                return $"ERROR Unknown order {orderId}";

            foreach (var kv in toSend)
            {
                var robot = kv.Key;
                var qty = kv.Value;

                // Si le robot n’est pas dans la commande ou déjà satisfait
                if (!order.RobotsRequested.ContainsKey(robot))
                    return $"ERROR {robot} not in order {orderId}";

                if (stock.GetRobotQuantity(robot) < qty)
                    return $"ERROR Not enough stock for {robot}";

                order.RobotsRequested[robot] -= qty;
                if (order.RobotsRequested[robot] <= 0)
                    order.RobotsRequested.Remove(robot);

                stock.RobotStock[robot] -= qty;
            }

            if (order.RobotsRequested.Count == 0)
            {
                _orders.Remove(orderId);
                return $"COMPLETED {orderId}";
            }

            var remaining = string.Join(", ",
                order.RobotsRequested.Select(kv => $"{kv.Value} {kv.Key}")
            );
            return $"Remaining for {orderId} : {remaining}";
        }

        public string GetOrderList()
        {
            if (_orders.Count == 0)
                return "No pending orders.";

            var lines = new List<string>();
            foreach (var order in _orders.Values)
            {
                var robots = string.Join(", ",
                    order.RobotsRequested.Select(kv => $"{kv.Value} {kv.Key}")
                );
                lines.Add($"{order.Id}: {robots}");
            }
            return string.Join(Environment.NewLine, lines);
        }

    }
}
