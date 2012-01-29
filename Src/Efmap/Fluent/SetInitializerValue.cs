using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace Efmap.Fluent
{
    public interface ISetInitializerValue<T> where T : DbContext
    {
        void With<U>() where U : IDatabaseInitializer<T>, new();
    }

    public class SetInitializerValue<T> : ISetInitializerValue<T> where T:DbContext
    {
        public void With<U>() where U : IDatabaseInitializer<T>,new()
        {
            Database.SetInitializer<T>(new U());
        }
    }
}
