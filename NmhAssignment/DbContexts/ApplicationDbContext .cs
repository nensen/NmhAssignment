using Microsoft.EntityFrameworkCore;
using NmhAssignment.DbModels;

namespace NmhAssignment.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasKey(e => e.Id);
              
                entity.HasIndex(e => e.Name).IsUnique();

                entity.HasOne(e => e.Image)
                      .WithOne(e => e.Author)
                      .HasForeignKey<Image>(e => e.AuthorId);
            });

            modelBuilder.Entity<Article>(entity =>
            {
                entity.HasKey(e => e.Id);
               
                entity.HasIndex(e => e.Title);

                entity.HasMany(e => e.Authors)
                      .WithMany(e => e.Articles);

                entity.HasOne(e => e.Site)
                      .WithMany(e => e.Articles)
                      .HasForeignKey(e => e.SiteId);
            });

            modelBuilder.Entity<Site>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }
    }
}