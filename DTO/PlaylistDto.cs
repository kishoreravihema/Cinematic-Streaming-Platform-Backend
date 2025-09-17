namespace Netflix_BackendAPI.DTO
{
    public class PlaylistDto
    {
        public int Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public int UserId { get; set; }
        public bool IsPublic { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public List<string>? Tags { get; set; }
        public List<VideoDto> Videos { get; set; } = new();
        public List<MusicDto> Musics { get; set; } = new();
    }
}
