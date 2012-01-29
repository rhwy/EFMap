using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Efmap.Fluent;
using System.Data.Entity;

namespace Efmap
{
    public static class EfMap
    {
        public static ISetInitializerValue<T> Initialize<T>() where T : DbContext
        {
            SetInitializerValue<T> value = new SetInitializerValue<T>();
            return value;
        }
    }
}
