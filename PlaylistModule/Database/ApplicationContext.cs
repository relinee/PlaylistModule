using Microsoft.EntityFrameworkCore;

namespace PlaylistModule
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Song> Songs { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Song>().HasData(
                    new Song { Id = 1, Title = "Music1", Duration = 10 },
                    new Song { Id = 2, Title = "Music2", Duration = 10 },
                    new Song { Id = 3, Title = "Music3", Duration = 10 }
            );
        }
    }
}
