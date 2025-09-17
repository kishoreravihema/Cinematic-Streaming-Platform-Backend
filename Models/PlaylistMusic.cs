using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Netflix_BackendAPI.Models
{
    public class PlaylistMusic
    {
        public int PlaylistId { get; set; }
        public int MusicId { get; set; }
        public Playlist Playlist { get; set; } = null!;
        public Music Music { get; set; } = null!;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string coverImagePath { get; set; } = string.Empty; // Path to the cover image
        public bool IsActive { get; set; } = true; // Indicates if the music is still part of the playlist
        public bool IsPremium { get; set; } = false; // Indicates if the music is premium content
        public int? UserId { get; set; } // Optional user who added the music
        public User? User { get; set; } // Navigation property for the user who added the music


    }

}
