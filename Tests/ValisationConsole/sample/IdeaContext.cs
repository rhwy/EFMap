using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations;
using Efmap.Helpers;
using Efmap.Bootstrap;

namespace ValisationConsole.sample
{
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

    public class IdeaMap : EntityTypeConfiguration<Idea>
    {
        public IdeaMap()
            : base()
        {
            HasKey(k => k.Id);
            Property(p => p.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();
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
                    context.SaveChanges();
                });
        }
    }

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
}
