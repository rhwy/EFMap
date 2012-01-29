using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace Efmap.Bootstrap
{
    public class DropCreateOnModelChanges<T> : IDatabaseInitializer<T> where T : DbContext
    {
        protected void Seed(T context)
        {

            EntityInitializer initializer = InitializerFactory.GetEntityInitializerFor<T>();
            initializer.Seed(context);
            context.SaveChanges();
        }

        public void InitializeDatabase(T context)
        {
            Database db = context.Database;
            if (!db.Exists() || !db.CompatibleWithModel(false))
            {
                db.Delete();
                db.Create();
            }
            DbInitializer dbinitializer = InitializerFactory.GetDbInitializerFor<T>();
            dbinitializer.InitializeDatabase(db);

            Seed(context);
        }
    }
}
