using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations;
using Efmap.Helpers;
using Efmap.Bootstrap;

namespace Integration.sample
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
