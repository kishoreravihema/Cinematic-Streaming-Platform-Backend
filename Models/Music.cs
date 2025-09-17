using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netflix_BackendAPI.Models
{
    public class Music
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public string? Lyrics { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? Language { get; set; }
        public int DurationInSeconds { get; set; }
        public bool IsExplicit { get; set; } = false;
        public bool IsPremium { get; set; } = false;
        public int PlayCount { get; set; } = 0;
        public DateTime ReleaseDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public int AlbumId { get; set; }
        public int GenreId { get; set; }
        public int? UserId { get; set; }

        // Navigation
        public Album Album { get; set; } = null!;
        public Genre? Genre { get; set; } = null!;
        public User? User { get; set; } = null!;
        public ICollection<Artist> Artists { get; set; } = new List<Artist>();
        public ICollection<PlaylistMusic>? PlaylistMusics { get; set; }
    }

}
