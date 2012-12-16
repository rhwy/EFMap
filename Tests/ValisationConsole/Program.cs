using System;
using System.Linq;
using Efmap;
using Integration.sample;
using Efmap.Bootstrap;
using Efmap.Helpers;

namespace IntegrationConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //define the data class to use to initialize database through entity
            EntityMapHelper.SetEntityInitializer<IdeaContext>(new IdeaEntityInitializer());

            //define the data class to use to add custom sql commands at 
            EntityMapHelper.SetDbInitializer<IdeaContext>(new IdeaDbInitializer());

            //define the pattern structure to initialize database
            EntityMapHelper.Initialize<IdeaContext>().With<DropCreateAlways<IdeaContext>>();
            
            //that's all, you can now use it and directly get the default data as in the exemple
            using (var context = new IdeaContext())
            {
                Console.WriteLine("default ideas : ");
                context.Ideas.ToList().ForEach(
                        idea => Console.WriteLine("- {0} ({1} comments)", idea.Title, idea.Comments.Count)
                    );
                Console.WriteLine("Press enter so print sql initialization script");
                Console.ReadLine();
                Console.Clear();

                string sql = context.GetSqlCreationScript();
                Console.WriteLine(sql);

                Console.WriteLine("Press enter so print generated Edmx script");
                Console.ReadLine();
                Console.Clear();

                string edmx = context.GetGeneratedEdmx();
                Console.WriteLine(edmx);
            }

            

            Console.Read();
        }
    }
}
