namespace Netflix_BackendAPI.Models
{
    public class PlaylistVideo
    {
        public int PlaylistId { get; set; }
        public Playlist Playlist { get; set; } = null!;
        public int VideoId { get; set; }
        public Video Video { get; set; } = null!;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public bool IsPremium { get; set; } = false;
        public int? UserId { get; set; }
        public User? User { get; set; }
    }


}
