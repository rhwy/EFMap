using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Efmap;
using System.Data.Entity;
using ValisationConsole.sample;
using Efmap.Bootstrap;

namespace ValisationConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            InitializerFactory.SetEntityInitializer<IdeaContext>(new IdeaEntityInitializer());
            EfMap.Initialize<IdeaContext>().With<DropCreateAlways<IdeaContext>>();
            
            using (IdeaContext context = new IdeaContext())
            {
                Console.WriteLine("default ideas : ");
                context.Ideas.ToList().ForEach(
                        idea => Console.WriteLine("{0} ({1} comments)", idea.Title, idea.Comments.Count)
                    );
            }

            Console.Read();

        }
    }
}
