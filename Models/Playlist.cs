using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Netflix_BackendAPI.Models
{
    public class Playlist
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsPublic { get; set; } = true;
        public string? CoverImageUrl { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? TagsJson { get; set; } 
        [NotMapped]
        public List<string>? Tags
        {
            get => string.IsNullOrWhiteSpace(TagsJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(TagsJson);
            set => TagsJson = JsonSerializer.Serialize(value);
        }
            
        public User? User { get; set; }

        // Navigation
       public ICollection<PlaylistVideo> PlaylistVideos { get; set; } = new List<PlaylistVideo>();
       public ICollection<PlaylistMusic> PlaylistMusics { get; set; } = new List<PlaylistMusic>();
    }


}
