namespace Netflix_BackendAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? Password { get; set; }
        public string? SubscriptionType { get; set; } 
        public string? ProfileImageUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Playlist>? Playlists { get; set; }
    }


}
