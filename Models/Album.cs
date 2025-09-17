using System.ComponentModel.DataAnnotations.Schema;

namespace Netflix_BackendAPI.Models
{
    [Table("Albums")]
    public class Album
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int? UserId { get; set; }
        public User? User { get; set; }
        public string? CoverImagePath { get; set; }

        // Navigation
        public ICollection<Music> MusicTracks { get; set; } = new List<Music>();
    }

}
