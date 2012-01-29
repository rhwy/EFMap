using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;

namespace Efmap.Helpers
{
    public static class MappingExtensions
    {
        public static void WithRequiredGeneratedIdentity<T,U>(this EntityTypeConfiguration<T> mapping, Expression<Func<T, U>> keyExpression) where T:class where U:struct
        {
            mapping.HasKey(keyExpression);
            mapping.Property(keyExpression)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();
        }
    }
}
