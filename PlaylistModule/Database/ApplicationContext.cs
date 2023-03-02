using Microsoft.EntityFrameworkCore;

namespace PlaylistModule
{
    public class ApplicationContext : DbContext
    {
        public DbSet<PlaylistSong> Songs { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options)
        {
        }

        //public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options)
        //   =>  Database.EnsureCreated();
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseNpgsql("Host=localhost;Port=5298;Database=playlistdb;Username=postgres;Password=пароль_от_postgres");
        //}
    }
}
