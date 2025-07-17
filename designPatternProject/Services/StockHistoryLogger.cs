using System;
using System.Collections.Generic;

namespace RobotFactory.Services
{
    public class StockHistoryLogger
    {
        private readonly List<string> _logs = new();

        public void Log(string context, string message)
        {
            _logs.Add($"[{context}] {message}");
        }

        public IEnumerable<string> GetAll() => _logs;

        public IEnumerable<string> GetMatching(params string[] elements)
        {
            var filters = elements.Select(e => e.Trim()).Where(e => !string.IsNullOrEmpty(e)).ToList();

            foreach (var log in _logs)
            {
                if (filters.Any(f => log.Contains(f, StringComparison.OrdinalIgnoreCase)))
                    yield return log;
            }
        }

    }
}
