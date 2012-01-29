using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace Efmap.Bootstrap
{
    public static class InitializerFactory
    {
        private static Dictionary<Type, DbInitializer> _dbInitializers = new Dictionary<Type, DbInitializer>();
        private static Dictionary<Type, EntityInitializer> _entityInitializers = new Dictionary<Type, EntityInitializer>();

        public static void SetDbInitializer<T>(DbInitializer dbInitializer ) where T : DbContext
        {
            if (_dbInitializers.ContainsKey(typeof(T)))
            {
                _dbInitializers.Remove(typeof(T));
            }
            _dbInitializers.Add(typeof(T), dbInitializer);
        }

        public static void SetEntityInitializer<T>(EntityInitializer entityInitializer) where T : DbContext
        {
            if (_entityInitializers.ContainsKey(typeof(T)))
            {
                _entityInitializers.Remove(typeof(T));
            }
            _entityInitializers.Add(typeof(T), entityInitializer);
        }

        public static DbInitializer GetDbInitializerFor<T>()
        {
            if (_dbInitializers.ContainsKey(typeof(T)))
            {
                return _dbInitializers[typeof(T)];
            }
            return new DbInitializer();
        }
        public static EntityInitializer<T> GetEntityInitializerFor<T>() where T:DbContext
        {
            if (_entityInitializers.ContainsKey(typeof(T)))
            {
                return (EntityInitializer<T>)_entityInitializers[typeof(T)];
            }
            return new EntityInitializer<T>();
        }

    }
}
