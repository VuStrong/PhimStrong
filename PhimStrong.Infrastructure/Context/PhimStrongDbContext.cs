using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PhimStrong.Domain.Models;

namespace PhimStrong.Infrastructure.Context
{
    public class PhimStrongDbContext : IdentityDbContext<User>
    {
#pragma warning disable
        public PhimStrongDbContext(DbContextOptions<PhimStrongDbContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Director> Directors { get; set; }
        public DbSet<Cast> Casts { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Tag> Tags { get; set; }

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

            builder.Entity<Movie>().Property(m => m.Id).ValueGeneratedNever();
            builder.Entity<Category>().Property(m => m.Id).ValueGeneratedNever();
            builder.Entity<Cast>().Property(m => m.Id).ValueGeneratedNever();
            builder.Entity<Director>().Property(m => m.Id).ValueGeneratedNever();
            builder.Entity<Country>().Property(m => m.Id).ValueGeneratedNever();
        }
    }
}
