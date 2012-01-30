using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Efmap.Fluent;
using System.Data.Entity;
using Efmap.Bootstrap;

namespace Efmap
{
    public static class EntityMapHelper
    {
        public static ISetInitializerValue<T> Initialize<T>() where T : DbContext
        {
            SetInitializerValue<T> value = new SetInitializerValue<T>();
            return value; 
        }

        public static void SetEntityInitializer<T>(EntityInitializer entityInitializer) where T : DbContext
        {
            InitializerFactory.SetEntityInitializer<T>(entityInitializer);
        }

        public static void SetDbInitializer<T>(DbInitializer dbInitializer ) where T : DbContext
        {
            InitializerFactory.SetDbInitializer<T>(dbInitializer);
        }
    }
}
