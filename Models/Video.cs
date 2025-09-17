using System.ComponentModel.DataAnnotations.Schema;

namespace Netflix_BackendAPI.Models
{
    public class Video
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public int? PlaylistId { get; set; }
        public int? CategoryId { get; set; }
        public string? ThumbnailUrl { get; set; }
        public double Rating { get; set; }
        public int DurationInSeconds { get; set; }
        public string? Language { get; set; }
        public string? AgeRating { get; set; }
        public string? Genre { get; set; }
        public string? Director { get; set; }
        public bool IsPremium { get; set; } = false;
        public DateTime ReleaseDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int? UserId { get; set; }
        public User? User { get; set; }
        public Category? Category { get; set; }
        public Playlist? Playlist { get; set; }

        public ICollection<Actor> Actors { get; set; } = new List<Actor>();
        public ICollection<PlaylistVideo> PlaylistVideos { get; set; } = new List<PlaylistVideo>();
    }

}
