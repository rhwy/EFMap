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

        /// <summary>
        /// Helper to remove Metadataconventions from modelBuilder, this option is used for code using EF4.2 with EF4.3
        /// it's activated (and true by default) but you can call it explicitely
        /// </summary>
        /// <param name="status"></param>
        public static void RemoveMetadataConvention(bool status = true)
        {
            InitializerFactory.SetOption("RemoveMetaDataStatus", status);
        }
    }
}
