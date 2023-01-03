﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Models;

namespace PhimStrong.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
#pragma warning disable
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<People> Casts { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            builder.Entity<Movie>(entity =>
            {
                entity.HasMany(e => e.Directors)
                      .WithMany(u => u.DirectedMovies);

                entity.HasMany(e => e.Casts)
                      .WithMany(u => u.JoinedMovies);
            });
        }
    }
}
