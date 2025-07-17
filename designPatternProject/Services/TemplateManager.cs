using System;
using System.Collections.Generic;
using RobotFactory.Core;
using RobotFactory.Data;
using RobotFactory.Enums;

namespace RobotFactory.Services
{
    public class TemplateManager
    {
        private readonly Dictionary<string, RobotTemplate> _templates;

        public TemplateManager()
        {
            _templates = RobotTemplates.LoadTemplates();
        }

        public bool Contains(string name) => _templates.ContainsKey(name);

        public RobotTemplate? GetTemplate(string name)
        {
            return _templates.TryGetValue(name, out var template) ? template : null;
        }

        public IEnumerable<RobotTemplate> GetAllTemplates() => _templates.Values;

        public bool AddTemplate(string name, Category category, List<string> pieces)
        {
            if (_templates.ContainsKey(name))
                return false;

            _templates[name] = new RobotTemplate(name, category, pieces);
            return true;
        }

        public void PrintTemplates()
        {
            Console.WriteLine("Templates disponibles :");
            foreach (var t in _templates)
                Console.WriteLine($"- {t.Key} ({t.Value.Category})");
        }
    }
}
