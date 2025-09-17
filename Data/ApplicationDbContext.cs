using Microsoft.EntityFrameworkCore;
using Netflix_BackendAPI.Models;

namespace Netflix_BackendAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // 📦 DbSets
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Video> Videos { get; set; }
        public virtual DbSet<Playlist> Playlists { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Actor> Actors { get; set; }
        public virtual DbSet<Music> Music { get; set; }
        public virtual DbSet<Artist> Artists { get; set; }
        public virtual DbSet<Album> Albums { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<PlaylistVideo> PlaylistVideos { get; set; }
        public virtual DbSet<PlaylistMusic> PlaylistMusics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite Key: PlaylistVideo
            modelBuilder.Entity<PlaylistVideo>()
                .HasKey(pv => new { pv.PlaylistId, pv.VideoId });

            modelBuilder.Entity<PlaylistVideo>()
                .HasOne(pv => pv.Playlist)
                .WithMany(p => p.PlaylistVideos)
                .HasForeignKey(pv => pv.PlaylistId);

            modelBuilder.Entity<PlaylistVideo>()
                .HasOne(pv => pv.Video)
                .WithMany(v => v.PlaylistVideos)
                .HasForeignKey(pv => pv.VideoId);

            modelBuilder.Entity<PlaylistVideo>()
                .HasOne(pv => pv.User)
                .WithMany()
                .HasForeignKey(pv => pv.UserId)
                .IsRequired(false);

            // Composite Key: PlaylistMusic
            modelBuilder.Entity<PlaylistMusic>()
                .HasKey(pm => new { pm.PlaylistId, pm.MusicId });

            modelBuilder.Entity<PlaylistMusic>()
                .HasOne(pm => pm.Playlist)
                .WithMany(p => p.PlaylistMusics)
                .HasForeignKey(pm => pm.PlaylistId);

            modelBuilder.Entity<PlaylistMusic>()
                .HasOne(pm => pm.Music)
                .WithMany(m => m.PlaylistMusics)
                .HasForeignKey(pm => pm.MusicId);

            modelBuilder.Entity<PlaylistMusic>()
                .HasOne(pm => pm.User)
                .WithMany()
                .HasForeignKey(pm => pm.UserId)
                .IsRequired(false);

            // Many-to-Many: Music ↔ Artist
            modelBuilder.Entity<Music>()
                .HasMany(m => m.Artists)
                .WithMany(a => a.MusicTracks)
                .UsingEntity(j => j.ToTable("MusicArtists"));

            // Many-to-Many: Video ↔ Actor
            modelBuilder.Entity<Video>()
                .HasMany(v => v.Actors)
                .WithMany(a => a.Videos)
                .UsingEntity(j => j.ToTable("VideoActors"));

            // Optional Foreign Keys
            modelBuilder.Entity<Video>()
                .HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .IsRequired(false);

            modelBuilder.Entity<Video>()
                .HasOne(v => v.Category)
                .WithMany(c => c.Videos)
                .HasForeignKey(v => v.CategoryId)
                .IsRequired(false);

            modelBuilder.Entity<Video>()
                .HasOne(v => v.Playlist)
                .WithMany()
                .HasForeignKey(v => v.PlaylistId)
                .IsRequired(false);

            modelBuilder.Entity<Music>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .IsRequired(false);

            // Required Foreign Keys
            modelBuilder.Entity<Music>()
                .HasOne(m => m.Album)
                .WithMany(a => a.MusicTracks)
                .HasForeignKey(m => m.AlbumId);

            modelBuilder.Entity<Music>()
                .HasOne(m => m.Genre)
                .WithMany(g => g.MusicTracks)
                .HasForeignKey(m => m.GenreId);

            modelBuilder.Entity<Playlist>()
                .HasOne(p => p.User)
                .WithMany(u => u.Playlists)
                .HasForeignKey(p => p.UserId);
        }


    }
}
