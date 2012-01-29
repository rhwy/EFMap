using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace Efmap.Bootstrap
{
    public class DbInitializer
    {
        private SortedSet<string> _initializeCommands = new SortedSet<string>();
        public SortedSet<string> GetCommands(string separator = "")
        {
            return _initializeCommands;
        }

        public void AddCommand(string command)
        {
            _initializeCommands.Add(command);
        }

        public void AddCommands(IEnumerable<string> commands)
        {
            if(commands == null || commands.Count() == 0)
            {
                return;
            }

            _initializeCommands.OrderBy(value => value).ToList().ForEach(
                command => _initializeCommands.Add(command));
        }

        public void InitializeDatabase(Database db)
        {
            foreach (string command in _initializeCommands)
            {
                db.ExecuteSqlCommand(command);
            }
        }

    }
}
