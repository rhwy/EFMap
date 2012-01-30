using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace Efmap.Bootstrap
{
    public class EntityInitializer
    {
        public virtual DbContext Seed(DbContext context) { return null; }

    }
    public class EntityInitializer<T>: EntityInitializer where T : DbContext
    {
        private SortedList<string, Action<T>> _initializeCommands = new SortedList<string, Action<T>>();
        
        public IEnumerable<string> ListCommands(string separator = "")
        {
            return _initializeCommands.Select(cmd=>cmd.Key);
        }

        public void AddCommand(string commandName, Action<T> command)
        {
            _initializeCommands.Add(commandName,command);
        }

        public T Seed(T context)
        {
            if (_initializeCommands.Count > 0)
            {
                foreach (Action<T> command in _initializeCommands.Values)
                {
                    command(context);
                }
            }

            context.SaveChanges();
            return context;
        }
    }
}
