# EF Map

## About

EF map provides easy additional helpers to work with Entity Framework Code First and make it more valuable.
It allows you to be focused on configuration and mapping in a clean way.

Functionalities:

*	Helps defining clean mappings with a single class mapping per entity and auto load them 
*	Helps defining database initialization with additional sql commands
*	Helps defining database initialization through default model data
*	Helps defining an identity column
*	Add validation helpers (for data anotations)
*	more to come ...

it helps you keeping focused with your data

## Sample usage

### Build your domain

Let's define a simple idea suggestion system, this how our domain looks like:

	public class Idea
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public DateTime Published { get; set; }
        public string Author { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual Vote Votes { get; set; }

        public Idea()
        {
            Comments = new HashSet<Comment>();
            Votes = new Vote();
            Published = DateTime.Now;
        }
    }

    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Published { get; set; }
        public string Author { get; set; }

        public Comment()
        {
            Published = DateTime.Now;
        }
    }

    [ComplexType]
    public class Vote
    {
        public int VoteUp { get; set; }
        public int VoteDown { get; set; }
    }

### Define your persistance

You have defined your domain in clean way until here, let's define now from your infrastructure perspective how you persist it with Entity Framework code first:

	public class IdeaMap : EntityTypeConfiguration<Idea>
    {
        public IdeaMap()
            : base()
        {
            this.WithRequiredGeneratedIdentity(p => p.Id);
            Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(100);
            Property(p=>p.Votes.VoteUp)
                .HasColumnType("int")
                .HasColumnName("VoteUp");
            Property(p => p.Votes.VoteDown)
                .HasColumnType("int")
                .HasColumnName("VoteDown");
            ToTable("Ideas");
        }
    }

    public class CommentMap : EntityTypeConfiguration<Comment>
    {
        public CommentMap() : base()
        {
            this.WithRequiredGeneratedIdentity(p => p.Id);
        }
    }


### Add some default data

There is usually two ways to complete the database initialisation process started by entity framework code first engine.
These options are complementary:

*	You can add some sql in order to add some specific commands that can not currently be generated automaticaly by 
Entity Framework engine during the initialization process : we call these DbInitializers.
For exemple, let's define that we want to ensure at database level that the title of our ideas is unique to avoid exact duplicates

        public class IdeaDbInitializer : DbInitializer
        {
            public IdeaDbInitializer()
            {
                //add constraint to avoid exact duplicate ideas
                this.AddCommand(
                    "IF NOT ISNULL(OBJECT_ID('Ideas','U'),0)=0 ALTER TABLE Ideas ADD CONSTRAINT UN_Ideass_Title UNIQUE(Title)"
                );
            }
        }

*	You can add some entity objects during the database initialisation to be used as default or starting values by
your application just like any other data : we call these EntityInitializers
For exemple, let's add a default idea with a comment to show users after the installation of our system an usage exemple:

	    public class IdeaEntityInitializer : EntityInitializer<IdeaContext>
        {
            public IdeaEntityInitializer()
            {
                this.AddCommand(
                    "add presentation idea",
                    (context) =>
                    {
                        Idea idea = new Idea();
                        idea.Title = "what about creating an ideas site?";
                        idea.Author = "admin";
                        idea.Comments.Add(new Comment { Author = "admin", Content = "vote and add your comments about the idea" });
                        idea.Votes.VoteUp++;
    
                        context.Ideas.Add(idea);
                    });
            }
        }

### Finish defining your context

You just have to define now your context with your DbSets. The interesting option now is the auto load. Define your sets available to your context, then, when defining autoload, it will search for each set of your context for a mapping. That's a cleaner way than handling everything in the "onModelcreating" method don't you think?

	    public class IdeaContext : DbContext
        {
            public DbSet<Comment> Comments { get; set; }
            public DbSet<Idea> Ideas { get; set; }
    
            public IdeaContext()
                : base("IdeasDb")
            {}
    
            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                this.AutoLoadForThisContext(modelBuilder);
                base.OnModelCreating(modelBuilder);
            }
        }

### create your app

You have now all your need, here is now how you link and use this in your application layer. Let's configure it, initialize it, and get print default values in a sample console app:

	    static void Main(string[] args)
            {
                //define the data class to use to initialize database through entity
                EntityMapHelper.SetEntityInitializer<IdeaContext>(new IdeaEntityInitializer());
    
                //define the pattern structure to initialize database
                EntityMapHelper.Initialize<IdeaContext>().With<DropCreateAlways<IdeaContext>>();
                
                //that's all, you can now use it and directly get the default data as in the exemple
                using (IdeaContext context = new IdeaContext())
                {
                    Console.WriteLine("default ideas : ");
                    context.Ideas.ToList().ForEach(
                            idea => Console.WriteLine("- {0} ({1} comments)", idea.Title, idea.Comments.Count)
                        );
                }
    
                Console.Read();
            }
          
## Foreword          
          
This is not supposed to be revolutionary, but these should help you working better with your EF code first projects by removing some plumbing practices and kept more focused on your business. 
This is aim to evolve with time, so share your thoughts and contribute with your additions.

Hope it helps ;-)