using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // Tables
        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Scoreboard> Scoreboards { get; set; }
        public DbSet<GameEnvironment> Environments { get; set; }
        public DbSet<Fish> Fishes { get; set; }
        public DbSet<Obstacle> Obstacles { get; set; }
        public DbSet<SeaWeed> SeaWeeds { get; set; }
        public DbSet<Tool> Tools { get; set; }
        public DbSet<FishingRod> FishingRods { get; set; }
        public DbSet<Boat> Boats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Player relationships ---
            modelBuilder.Entity<Player>()
                .HasOne(p => p.Boat)
                .WithOne(b => b.Player)
                .HasForeignKey<Player>(p => p.BoatId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.FishingRod)
                .WithOne(fr => fr.Player)
                .HasForeignKey<Player>(p => p.FishingRodId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.Game)
                .WithMany(g => g.Players)
                .HasForeignKey(p => p.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- Game relationships ---
            modelBuilder.Entity<Game>()
                .HasOne(g => g.GameEnvironment)
                .WithMany()
                .HasForeignKey(g => g.GameEnvironmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.Scoreboard)
                .WithOne(s => s.Game)
                .HasForeignKey<Game>(g => g.ScoreboardId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.Session)
                .WithOne(s => s.Game)
                .HasForeignKey<Session>(s => s.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- Session ---
            modelBuilder.Entity<Session>()
                .HasOne(s => s.Game)
                .WithOne(g => g.Session)
                .HasForeignKey<Session>(s => s.GameId);

            // --- Environment relationships ---
            modelBuilder.Entity<GameEnvironment>()
                .HasMany(e => e.Fishes)
                .WithOne(f => f.GameEnvironment)
                .HasForeignKey(f => f.GameEnvironmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GameEnvironment>()
                .HasMany(e => e.Obstacles)
                .WithOne(o => o.GameEnvironment)
                .HasForeignKey(o => o.GameEnvironmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- Tool / FishingRod inheritance ---
            modelBuilder.Entity<Tool>()
                .HasDiscriminator<string>("ToolType")
                .HasValue<Tool>("Tool")
                .HasValue<FishingRod>("FishingRod");

            // --- Obstacle / SeaWeed inheritance ---
            modelBuilder.Entity<Obstacle>()
                .HasDiscriminator<string>("ObstacleType")
                .HasValue<Obstacle>("Obstacle")
                .HasValue<SeaWeed>("SeaWeed");
        }
    }
}
