using RobotFactory.Core;
using System.Collections.Generic;

namespace RobotFactory.Services
{
    public class FactoryManager
    {
        private readonly Dictionary<string, Factory> _factories = new();

        public FactoryManager(IEnumerable<string> factoryNames)
        {
            foreach (var name in factoryNames)
                _factories[name] = new Factory(name);
        }

        public bool HasFactory(string name) => _factories.ContainsKey(name);

        public Factory? GetFactory(string name) =>
            _factories.TryGetValue(name, out var f) ? f : null;

        public IEnumerable<string> GetFactoryNames() => _factories.Keys;
    }
}
